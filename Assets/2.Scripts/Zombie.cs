using System.Linq;
using Data;
using UnityEngine;

public class Zombie : MonoBehaviour, IDamageInterface
{
    #region Zombie References

    [SerializeField] private UnitType unitType;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Animator zombieAnimator;
    [SerializeField] private Collider2D zombieCollider;
    [SerializeField] private Rigidbody2D zombieRigidbody2D;
    [SerializeField] private UnityEngine.Rendering.SortingGroup sortingGroup;
    
    private bool m_IsAttacking;
    private bool m_PushBack;
    private bool m_IsClimbing;
    private int m_CurrentHealth;
    private float m_ElapsedTime;
    
    private IDamageInterface m_CachedDamageInterface;
    private GlobalBalanceSetting.StatData m_CacheStatData;

    #endregion
    
    public void SetZombie(ZombieSpawner.SpawnInfo spawnInfo)
    {
        var layer = Mathf.RoundToInt(Mathf.Log(spawnInfo.zombieLayer.value, 2));
        gameObject.layer = layer;
        sortingGroup.sortingOrder = spawnInfo.sortingOrder;
        
        var statData = Database.GlobalBalanceSetting.statDatas.FirstOrDefault(data => data.unitType == unitType);
        if (statData == null)
        {
            return;
        }
        
        m_CacheStatData = statData;
        m_CurrentHealth = statData.health;
        healthBar.SetHealth(statData.health);
    }
    
    #region MonoBehaviour Events
    
    private void Update()
    {
        zombieAnimator.SetBool("IsAttacking", m_IsAttacking);

        if (!m_IsClimbing)
        {
            return;
        }
        
        m_ElapsedTime += Time.deltaTime;
        if (Database.GlobalBalanceSetting.zombieClimbingCooldown <= m_ElapsedTime)
        {
            m_IsClimbing = false;
            m_ElapsedTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        // if (m_IsAttacking)
        // {
        //     return;
        // }
        //
        // if (zombieRigidbody2D.velocity.x > -moveSpeed)
        // {
        //     zombieRigidbody2D.AddForce(Vector2.left * moveSpeed);
        // }
        //
        // if (zombieRigidbody2D.velocity.x < 0)
        // {
        //     m_PushBack = false;
        // }
        if (m_IsAttacking)
        {
            return;
        }
        
        zombieRigidbody2D.velocity = new Vector2(
            -Database.GlobalBalanceSetting.zombieSpeed, 
            zombieRigidbody2D.velocity.y
        );
    }
    
    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.layer == LayerMask.NameToLayer("Tower"))
    //     {
    //         m_IsAttacking = true;
    //     }
    //     
    //     if (m_PushBack)
    //     {
    //         return;
    //     }
    //     
    //     if (other.gameObject.layer == gameObject.layer && !m_IsAttacking)
    //     {
    //         if (other.gameObject.TryGetComponent(out Zombie zombie))
    //         {
    //             var contact = other.contacts[0];
    //             var collisionY = contact.point.y;
    //             var otherTopY = zombie.zombieCollider.bounds.max.y;
    //             var otherBottomY = other.collider.bounds.min.y;
    //             
    //             if (collisionY <= otherTopY && zombieRigidbody2D.velocity.y != 0)
    //             {
    //                 if (zombie.m_IsAttacking)
    //                 {
    //                     zombie.zombieRigidbody2D.AddForce(Vector2.right * pushForce, ForceMode2D.Impulse);
    //                     zombie.m_PushBack = true;
    //                 }
    //             }
    //             
    //             var tolerance = 0.1f;
    //
    //             var isBodyCollision = collisionY > otherBottomY + tolerance && collisionY < otherTopY - tolerance;
    //             var relativeVelocityX = other.rigidbody.velocity.x - zombieRigidbody2D.velocity.x;
    //
    //             if (isBodyCollision && relativeVelocityX > 0 && !m_IsClimbing)
    //             {
    //                 Debug.Log($"{gameObject.name}이 {other.gameObject.name}의 **뒤쪽 몸통에 충돌함**");
    //                 zombieRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    //                 m_IsClimbing = true;
    //             }
    //         }
    //         
    //         // if (zombieRigidbody2D.velocity.y == 0)
    //         // {
    //         //     zombieRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    //         // }
    //
    //     }
    // }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tower") &&
            other.gameObject.TryGetComponent(out m_CachedDamageInterface))
        {
            m_IsAttacking = true;
            return;
        }

        if (m_PushBack || m_IsClimbing)
        {
            return;
        }

        if (other.gameObject.layer == gameObject.layer && !m_IsAttacking &&
            other.gameObject.TryGetComponent(out Zombie zombie))
        {

            var contact = other.contacts[0];
            var collisionY = contact.point.y;
            var otherTopY = zombie.zombieCollider.bounds.max.y;
            if (collisionY <= otherTopY && zombieRigidbody2D.velocity.y != 0)
            {
                if (zombie.m_IsAttacking)
                {
                    zombie.m_PushBack = true;
                    zombie.zombieRigidbody2D.AddForce(Vector2.right * Database.GlobalBalanceSetting.zombiePushForce,
                        ForceMode2D.Impulse);
                }
            }

            var otherBottomY = other.collider.bounds.min.y;
            var isBodyCollision = collisionY > otherBottomY + Database.GlobalBalanceSetting.zombieCollisionTolerance
                                  && collisionY < otherTopY - Database.GlobalBalanceSetting.zombieCollisionTolerance;
            var relativeVelocityX = other.rigidbody.velocity.x
                                    - zombieRigidbody2D.velocity.x;

            if (isBodyCollision && relativeVelocityX > 0 && !m_IsClimbing)
            {
                // zombieRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                // m_IsClimbing = true;
                // m_ElapsedTime = 0f;
                zombieRigidbody2D.velocity = new Vector2(zombieRigidbody2D.velocity.x,
                    Database.GlobalBalanceSetting.zombieClimbingForce);
                m_IsClimbing = true;
                m_ElapsedTime = 0f;
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tower"))
        {
            m_IsAttacking = false;
            m_CachedDamageInterface = null;
        }
    }

    #endregion

    public void OnAttack()
    {
        if (!m_IsAttacking || m_CachedDamageInterface == null)
        {
            return;
        }

        m_CachedDamageInterface.TakeDamage(m_CacheStatData.damage);
    }

    public void TakeDamage(int damage)
    {
        m_CurrentHealth -= damage;
        healthBar.UpdateHealth(m_CurrentHealth);
        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
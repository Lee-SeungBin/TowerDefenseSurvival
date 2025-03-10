using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Animator zombieAnimator;
    [SerializeField] private Rigidbody2D zombieRigidbody2D;
    [SerializeField] private UnityEngine.Rendering.SortingGroup sortingGroup;
    
    private bool m_IsAttacking;
    private bool m_IsClimbing;
    
    public void SetZombie(ZombieSpawner.SpawnInfo spawnInfo)
    {
        var layer = Mathf.RoundToInt(Mathf.Log(spawnInfo.zombieLayer.value, 2));
        gameObject.layer = layer;
        sortingGroup.sortingOrder = spawnInfo.sortingOrder;
    }
    
    #region MonoBehaviour Events
    
    private void Update()
    {
        zombieRigidbody2D.velocity = new Vector2(-moveSpeed, zombieRigidbody2D.velocity.y);
        zombieAnimator.SetBool("IsAttacking", m_IsAttacking);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tower"))
        {
            m_IsAttacking = true;
        }
        
        if (other.gameObject.layer == gameObject.layer && !m_IsAttacking && zombieRigidbody2D.velocity.y == 0)
        {
            zombieRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tower"))
        {
            m_IsAttacking = false;
        }
    }

    #endregion

    public void OnAttack()
    {
        
    }
}
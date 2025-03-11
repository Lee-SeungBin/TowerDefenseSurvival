using System.Linq;
using Data;
using UnityEngine;

public class Player : MonoBehaviour, IDamageInterface
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private UnitType unitType;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private LayerMask sensingLayerMask;
    [SerializeField] private float sensingInterval;
    
    private int m_CurrentHealth;
    private float m_SensingTimer;
    private float m_LastAttackTime;
    private GlobalBalanceSetting.StatData m_CacheStatData;
    private Transform m_CurrentTargetTransform;
    private readonly Collider2D[] m_SensingEnemyCache = new Collider2D[32];
    

    private void Start()
    {
        var statData = Database.GlobalBalanceSetting.statDatas.FirstOrDefault(data => data.unitType == unitType);
        if (statData == null)
        {
            return;
        }
        
        m_CacheStatData = statData;
        m_CurrentHealth = statData.health;
        healthBar.SetHealth(statData.health);
    }
    
    private void Update()
    {
        m_SensingTimer += Time.deltaTime;

        if (m_SensingTimer >= sensingInterval)
        {
            m_SensingTimer = 0f;
            SensingEnemy();
        }

        if (m_CurrentTargetTransform == null)
        {
            return;
        }
        
        m_LastAttackTime += Time.deltaTime;

        if (m_LastAttackTime >= 1 / Database.GlobalBalanceSetting.attackSpeed)
        {
            m_LastAttackTime = 0f;
            Attack(m_CurrentTargetTransform);
        }
    }
    
    private void SensingEnemy()
    {
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, Database.GlobalBalanceSetting.attackRange,
            m_SensingEnemyCache, sensingLayerMask);

        var closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        for (var i = 0; i < size; i++)
        {
            if (m_SensingEnemyCache[i] == null)
            {
                continue;
            }

            var distance = Vector2.Distance(transform.position, m_SensingEnemyCache[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = m_SensingEnemyCache[i].transform;
            }
        }

        if (m_CurrentTargetTransform != null)
        {
            var distanceToCurrentTarget = Vector2.Distance(transform.position, m_CurrentTargetTransform.position);
            if (distanceToCurrentTarget > Database.GlobalBalanceSetting.attackRange)
            {
                m_CurrentTargetTransform = null;
            }
        }

        if (closestTarget != null)
        {
            m_CurrentTargetTransform = closestTarget;
        }
    }

    private void Attack(Transform target)
    {
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.SetProjectile(target,m_CacheStatData.damage);
    }
    
    public void TakeDamage(int damage)
    {
        m_CurrentHealth -= damage;
        healthBar.UpdateHealth(m_CurrentHealth);
        
        if (m_CurrentHealth <= 0)
        {
            var currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
            Destroy(gameObject);
        }
    }
}
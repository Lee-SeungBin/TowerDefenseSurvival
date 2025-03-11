using System.Linq;
using Data;
using UnityEngine;

public class Tower : MonoBehaviour, IDamageInterface
{
    [SerializeField] private UnitType unitType;
    [SerializeField] private HealthBar healthBar;
    
    private int m_CurrentHealth;
    
    private void Start()
    {
        var statData = Database.GlobalBalanceSetting.statDatas.FirstOrDefault(data => data.unitType == unitType);
        if (statData == null)
        {
            return;
        }
        m_CurrentHealth = statData.health;
        healthBar.SetHealth(m_CurrentHealth);
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
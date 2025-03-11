using UnityEngine;

public class Projectile : MonoBehaviour
    {
        private int m_Damage;
        private Transform m_TargetTransform;

        public void SetProjectile(Transform targetTransform, int damage)
        {
            m_TargetTransform = targetTransform;
            m_Damage = damage;
        }
        
        private void Update()
        {
            if (m_TargetTransform == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector2 direction = (m_TargetTransform.position - transform.position).normalized;
            transform.Translate(direction * (Data.Database.GlobalBalanceSetting.projectileSpeed * Time.deltaTime));

            if (Vector2.Distance(transform.position, m_TargetTransform.position) < 0.1f)
            {
                OnHitTarget();
            }
        }

        private void OnHitTarget()
        {
            if (m_TargetTransform.TryGetComponent(out IDamageInterface eventReceiver))
            {
                Destroy(gameObject);
                eventReceiver.TakeDamage(m_Damage);
            }
        }
    }

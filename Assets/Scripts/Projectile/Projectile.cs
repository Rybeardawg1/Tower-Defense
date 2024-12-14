using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public int damage = 1; // Damage dealt to enemies
    private GameObject target;

    public void Initialize(GameObject enemyTarget)
    {
        target = enemyTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destroy projectile if no target
            return;
        }

        // Move toward the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Check if the projectile is close to the target
        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            ApplyDamage();
            Destroy(gameObject); // Destroy after hitting
        }
    }

    void ApplyDamage()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}

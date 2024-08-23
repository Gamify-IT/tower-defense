using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the behaviour of projectiles fired by towers
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private int projectileDamage = 1;

    private Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void FixedUpdate()
    {
        if (!target) return;
        //normalized = between 0 and 1
        Vector2 direction = (target.position - transform.position).normalized; 
        rb.velocity = direction * projectileSpeed;
    }

    /// <summary>
    ///  Hits an enemy with a projectile and reduces its health
    /// </summary>
    /// <param name="other"> the enemy </param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealth>().TakeDamage(projectileDamage);
            Destroy(gameObject);
        }
    }
}

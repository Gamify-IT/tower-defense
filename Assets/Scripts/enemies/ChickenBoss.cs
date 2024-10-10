using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Enemy class for the bosses that can destroy towers
/// </summary>
public class ChickenBoss : EnemyMovement
{
    [Header("Shooting Attributes")]
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float targetingRange = 3f;
    [SerializeField] private float shotCooldown = 5f; 
    [SerializeField] private Transform firingPoint; 

    private BaseTower currentTargetTower; 
    private float timeUntilFire;

    protected override void Update()
    {
        base.Update();
        if (currentTargetTower == null)
        {
            FindTargetTower();
            return;
        }
        if (CheckTargetIsInRange())
        {
            if (Time.time - timeUntilFire >= shotCooldown)
            {
                ShootAtTower();
                timeUntilFire = Time.time;
            }
        }
        else
        {
            
            currentTargetTower = null;
        }
    }



    /// <summary>
    /// Finds the nearest tower within range to target.
    /// </summary>
    private void FindTargetTower()
    {
        BaseTower[] allTowers = FindObjectsOfType<BaseTower>();
        float closestDistance = targetingRange;
        BaseTower closestTower = null;

        foreach (BaseTower tower in allTowers)
        {
            float distanceToTower = Vector2.Distance(transform.position, tower.transform.position);
            if (distanceToTower <= closestDistance)
            {
                closestTower = tower;
                closestDistance = distanceToTower;
            }
        }

        if (closestTower != null)
        {
            currentTargetTower = closestTower;
        }
    }

    /// <summary>
    /// Shoots a bullet towards the targeted tower.
    /// </summary>
    private void ShootAtTower()
    {

        if (currentTargetTower == null) return;

        GameObject projectileObj = Instantiate(Projectile, firingPoint.position, Quaternion.identity);
        EnemyProjectile projectileScript = projectileObj.GetComponent<EnemyProjectile>();
        projectileScript.SetTarget(currentTargetTower.transform);
    }

    /// <summary>
    ///  This method makes sure that the enemy is in range for the tower to attack
    /// </summary>
    /// <returns> True if the target is in range, false otherwise</returns>
    protected bool CheckTargetIsInRange()
    {
        
      return  Vector2.Distance(transform.position, currentTargetTower.transform.position) <= targetingRange;
    }

}

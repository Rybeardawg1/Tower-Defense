using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5f;             // Tower's attack range
    public float fireRate = 1f;          // Shots per second
    public GameObject projectilePrefab;  // Prefab for the projectile
    public int damage = 10;              // Damage this tower deals

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        // Find the closest enemy
        GameObject target = FindClosestEnemy();

        if (target != null && fireCooldown <= 0f)
        {
            Fire(target); // Fire at the enemy
            fireCooldown = 1f / fireRate; // Reset cooldown

            // Reduce health through GameManager
            ReduceHealthBasedOnTower();
        }
    }

    GameObject FindClosestEnemy()
    {
        // Find all objects in the scene
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Orc");
        GameObject closest = null;
        float closestDistance = range;

        foreach (GameObject obj in allObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closest = obj;
                closestDistance = distance;
            }
        }

        return closest;
    }

    void Fire(GameObject target)
    {
        // Instantiate the projectile and set its target
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Initialize(target);

        Vector3 newPosition = projectile.transform.position;
        newPosition.y = 1; // Update the Y coordinate
        projectile.transform.position = newPosition;
    }

    void ReduceHealthBasedOnTower()
    {
        // Reduce health using GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ReduceGameHealth(damage);
            Debug.Log($"Tower fired! Reduced health by {damage}.");
        }
    }
}

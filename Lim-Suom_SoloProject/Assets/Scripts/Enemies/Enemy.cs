using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{

    public bool isFollowing = false;
    public bool hasAttacked = false;

    public int health = 25;
    public int maxHealth = 25;
    public int damageDealt = 15;

    public float detectionRange = 5;
    public float attackCooldown = 2;

    public PlayerController player;
    public NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        if(health <= 0)
            // set enemy count -- towards game manager
            Destroy(gameObject);

        // Find difference between player position and enemy position
        float targetDistance = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));

        isFollowing = targetDistance <= detectionRange;

        // Follow player 
        if(health > 0)
            if(isFollowing)
            {
                agent.destination = player.transform.position;
            }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && hasAttacked == false)
        {
            hasAttacked = true;
            StartCoroutine("enemyAttackCooldown");
        }
        else
        {
            // back away?
        }

        // Take damage equal to player's current weapon's damage
        if (collision.gameObject.tag == "Projectile")
            health -= player.currentWeapon.damage;

        // taking dmg from other enemies projectiles prototype (obviously not gonna work yet)
        //if (collision.gameObject.tag == "enemyProjectile")
        //    health -= enemy.currentWeapon.damage - 5;
    }

    // If the enemy has attacked, wait for enemy's attack cooldown. Then, allow them to attack again.
    IEnumerator enemyAttackCooldown()
    {
        if(hasAttacked == true)
        {
            yield return new WaitForSeconds(attackCooldown);
            hasAttacked = false;
        }
    }
}

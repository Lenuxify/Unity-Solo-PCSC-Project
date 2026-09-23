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
        // Find difference between player position and enemy position
        float targetDistance = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));
        // float trackDistance = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));

        isFollowing = targetDistance <= detectionRange;

        // Follow player 
        if(isFollowing)
        {
            agent.destination = player.transform.position;
        }
    }

    // If collides with player, deal damage. Then, start the attack cooldown right after.
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && hasAttacked == false)
        {
            player.health -= damageDealt;
            hasAttacked = true;

            StartCoroutine("enemyAttackCooldown");
        }
        else
        {
            // back away?
        }
    }

    // If the enemy has attacked, wait for enemy's attack cooldown. Then, allow them to attack again.
    IEnumerator enemyAttackCooldown()
    {
        if(hasAttacked == true)
        {
            yield return new WaitForSeconds(attackCooldown);

            //player.health -= damageDealt;

            hasAttacked = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{

    bool facingRight = true;
    public Transform target;
    public Transform enemyGfx;
    public float speed = 200;
    // PATHFINDING 
    public float nextWaypointDistance = 3;
    Path path;
    Seeker seeker;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    Animator animator;
    Rigidbody2D rb;

    // COMBAT 
    public int maxHealth = 100;
    public Transform attackPoint;
    float nextAttackTime = 0f;
    public float attackRange = 0.5f;
    public int attackDamage = 5;
    public float attackRate = 0.5f;
    public LayerMask enemyLayers;
    bool targetAlive = true;
    int currentHealth;


    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = enemyGfx.GetComponent<Animator>();
        currentHealth = maxHealth;
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath(){
        if (seeker.IsDone()){
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    
        }
    }
    void OnPathComplete(Path p){
        if(!p.error){
            path = p;
            currentWayPoint = 0;

        }
    }

    void FixedUpdate()
    {
        if (path == null){
            return;
        }
        if (currentWayPoint >= path.vectorPath.Count){
            reachedEndOfPath = true;
            if(Time.time >= nextAttackTime && targetAlive){
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
            }
            else if (!targetAlive){
               CancelInvoke(); 
            }
            return;
        }
        else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        
        rb.AddForce(force);
        
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWaypointDistance){
            currentWayPoint++;
        }
        // if(rb.velocity.x >= 0.01f){
        //     enemyGfx.localScale = new Vector3(1f, 1f, 1f);
        // }
        // else if(rb.velocity.x <= -0.01f){
        //     enemyGfx.localScale = new Vector3(-1f, 1f, 1f);
        // }
        if(rb.velocity.x <= -0.01f && facingRight){
            transform.Rotate(0f, 180f, 0f);
            facingRight = false;
        }
        else if(rb.velocity.x >= 0.01f && !facingRight){
            transform.Rotate(0f, 180f, 0f);
            facingRight = true;
        } 

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        if(animator.GetBool("IsDead")){
            speed = 0;
        }
    }
    
    void Attack(){
        animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies){
            enemy.GetComponent<PlayerCombat>().TakeDamage(attackDamage);
            targetAlive = enemy.GetComponent<PlayerCombat>().isAlive;
        }
    }

    public void TakeDamage(int damage){
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if(currentHealth <= 0){
            Die();
        }
    }
    void OnDrawGizmosSelected(){
        if(attackPoint == null){
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    void Die(){
        animator.SetBool("IsDead", true);
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        CancelInvoke();
        this.enabled = false;
        
    }
}

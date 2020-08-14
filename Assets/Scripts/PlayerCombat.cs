using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // Update is called once per frame
    public Animator animator;
    public float attackRange = 0.5f;
    public float attackRate = 1f;
    float nextAttackTime = 0f;
    public int attackDamage = 40;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    int currentHealth;
    public int maxHealth = 300;

    void Start() {
        currentHealth = maxHealth;
        
    }
    void Update()
    {
        if(Time.time >= nextAttackTime){
            if (Input.GetKeyDown(KeyCode.Space)){
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        
        }
    }

    void Attack(){
        animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies){
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
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
        this.enabled = false;
    }
}

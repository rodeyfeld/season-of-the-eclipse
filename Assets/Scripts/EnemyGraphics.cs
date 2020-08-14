using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyGraphics : MonoBehaviour
{
    // Start is called before the first frame update
    public AIPath aIPath;
    public Animator animator;
    // Update is called once per frame
    float horizontalMove = 0f;
    void Update()
    {
        if(aIPath.desiredVelocity.x >= 0.01f){
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if(aIPath.desiredVelocity.x <= -0.01f){
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        animator.SetFloat("Speed", Mathf.Abs(aIPath.desiredVelocity.x));
        if(animator.GetBool("IsDead")){
            this.aIPath.enabled = false;
        }
    }
}

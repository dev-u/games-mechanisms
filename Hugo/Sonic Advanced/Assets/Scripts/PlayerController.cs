using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool walking;

    // Use this for initialization
    void Awake() {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal");
        
        if(horizontal != 0 ) {
            animator.SetBool( "walking", true );
            if ( horizontal > 0 ) {
                spriteRenderer.flipX = true;
            } else {
                spriteRenderer.flipX = false;
            }
        } else {
            animator.SetBool( "walking", false );
        }

	}
}

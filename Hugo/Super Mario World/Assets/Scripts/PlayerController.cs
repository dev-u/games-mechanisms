using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Variables

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool walking;
    public bool Walking {
        get { return walking; }
        set {
            walking = value;
            animator.SetBool( "walking", value );
        }
    }

    #endregion Variables

    void Awake () {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        float horz = Input.GetAxis("Horizontal");

        if (horz != 0f ) {
            Walking = true;

            if(horz > 0 ) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }

        } else {
            Walking = false;
        }
    }
}

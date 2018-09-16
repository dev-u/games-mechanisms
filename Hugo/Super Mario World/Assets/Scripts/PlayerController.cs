using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Variables

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;

    [SerializeField]
    private int jumpForce = 150;

    // Verifies the ground
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public bool grounded;

    private bool jumping;
    public bool Jumping {
        get { return jumping; }
        set {
            jumping = value;
            animator.SetBool( "jumping", value );
        }
    }

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
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
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

        grounded = Physics2D.OverlapCircle( groundCheck.position, 0.2f, whatIsGround );
        Jumping = !grounded;

        if ( Input.GetButtonDown( "Jump" ) ) {
            rigidbody2D.AddForce( Vector2.up * jumpForce );
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Variables

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    [SerializeField]
    private int jumpForce = 150;
    [SerializeField]
    private float walkSpeed = 0.4f;

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

    private bool falling;
    public bool Falling {
        get { return falling; }
        set {
            falling = value;
            animator.SetBool( "falling", falling );
        }
    }

    #endregion Variables

    void Awake () {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float horz = Input.GetAxis("Horizontal");

        if (horz != 0f ) {
            Walking = true;
            transform.position += Vector3.right * horz * walkSpeed * Time.deltaTime;

            if (horz > 0 ) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }

        } else {
            Walking = false;
        }

        grounded = Physics2D.OverlapBox( groundCheck.position, new Vector2( boxCollider2D.size.x, 0.001f ), 0f, whatIsGround );
        Jumping = !grounded;

        if ( grounded && Input.GetButtonDown( "Jump" ) ) {
            rigidbody2D.AddForce( Vector2.up * jumpForce );
        }

        Falling = ( rigidbody2D.velocity.y < -0.2f );
    }
}

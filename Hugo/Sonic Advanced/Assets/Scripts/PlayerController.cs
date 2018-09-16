using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    private bool walking;

    // Checks the ground collision
    public Transform groundCheck;
    private bool grounded;
    public LayerMask whatIsGround;

    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float jumpImpulse = 100f;

    // Use this for initialization
    void Awake() {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        float horizontal = Input.GetAxis("Horizontal");
        
        if(horizontal != 0 ) {
            animator.SetBool( "walking", true );
            transform.position += Vector3.right * speed * Time.deltaTime * horizontal;

            spriteRenderer.flipX = horizontal > 0 ? true : false;

        } else {
            animator.SetBool( "walking", false );
        }

        grounded = Physics2D.OverlapCircle( groundCheck.position, 0.001f, whatIsGround );

        animator.SetBool( "jumping", !grounded );

        if ( Input.GetButtonDown( "Jump" ) && grounded ) {
            rigidbody2D.AddForce( Vector2.up * jumpImpulse );
        }

	}
}

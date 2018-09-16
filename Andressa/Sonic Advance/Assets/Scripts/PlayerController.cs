using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool walking;
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float jumpImpulse = 100f;

    public Transform groundCheck;
    public LayerMask whatIsGround; 
    
    private bool reachedGround;

    

    // Use this for initialization
    private void Awake(){
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Start () {
    

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float horizontal = Input.GetAxis("Horizontal");

        walking = horizontal != 0 ? true : false;
        animator.SetBool("walking", walking);

        if(horizontal != 0){
            animator.SetBool("walking", true);
            transform.position += Vector3.right * speed * Time.deltaTime * horizontal;
            spriteRenderer.flipX = horizontal > 0 ? true : false;
        }else{
            animator.SetBool("walking", false);
        }

        reachedGround = Physics2D.OverlapCircle(groundCheck.position, 0.001f, whatIsGround);

        animator.SetBool("jumping", !reachedGround);

        if(Input.GetButtonDown("Jump") && reachedGround){
            rigidbody2D.AddForce(Vector2.up * jumpImpulse);
            animator.SetBool("jumping", true);
        }
	}
}

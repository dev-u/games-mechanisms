using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Variables
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;

    [SerializeField]
    private int forceJump = 200;

    // Slide
    private bool slide;
    public float slideDuration = 1f;
    private float slidingTime;

    // Verifies the ground
    [SerializeField]
    private Transform groundCheck;
    public bool grounded;
    [SerializeField]
    private LayerMask whatIsGround;
    #endregion

    void Awake() {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if ( Input.GetButtonDown( "Jump" ) && grounded ) {
            rigidbody2D.AddForce( Vector2.up * forceJump );
            slide = false;
        }

        if ( Input.GetButtonDown( "Slide" ) && grounded ) {
            slide = true;
            slidingTime = 0;
        }

        if( slide == true ) {
            slidingTime += Time.deltaTime;
            if(slidingTime >= slideDuration ) {
                slide = false;
            }
        }

        grounded = Physics2D.OverlapCircle( groundCheck.position, 0.2f, whatIsGround );

        animator.SetBool( "jump", !grounded );
        animator.SetBool( "slide", slide );
    }
}

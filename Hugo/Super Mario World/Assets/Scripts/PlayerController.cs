using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Variables

    // GameObject Components
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    // PlayerController Variables
    [SerializeField]
    private int jumpForce = 165;
    private float currentSpeed;
    [SerializeField]
    private float walkSpeed = 0.65f;
    [SerializeField]
    private float fastWalkSpeed = 1.3f;
    [SerializeField]
    private float runSpeed = 2f;
    [SerializeField]
    private float timeUntilMaxSpeed = 0.6f;
    private float currentTime = 0f;

    // Verifies the ground
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public bool grounded;


    // Animation Controller Variables
    private bool lookingUp;
    public bool LookingUp {
        get { return lookingUp; }
        set {
            lookingUp = value;
            animator.SetBool( "lookingup", lookingUp );
        }
    }

    private bool crouched;
    public bool Crouched {
        get { return crouched; }
        set {
            crouched = value;
            animator.SetBool( "crouched", crouched );
        }
    }

    private bool jumping;
    public bool Jumping {
        get { return jumping; }
        set {
            jumping = value;
            animator.SetBool( "jumping", value );
        }
    }

    private bool falling;
    public bool Falling {
        get { return falling; }
        set {
            falling = value;
            animator.SetBool( "falling", falling );
            if ( falling ) {
                if ( !Jumping ) {
                    currentTimePenalty( timeUntilMaxSpeed / 3 );
                } else if ( !Running ) {
                    currentTimePenalty( timeUntilMaxSpeed / 2 );
                }

            }
        }
    }

    private bool walking;
    public bool Walking {
        get { return walking; }
        set {
            walking = value;
            animator.SetBool( "walking", walking );
        }
    }

    private bool fastWalking;
    public bool FastWalking {
        get { return fastWalking; }
        set {
            fastWalking = value;
            animator.SetBool( "fastwalking", fastWalking );

            if ( fastWalking ) {
                currentSpeed = fastWalkSpeed;
            } else {
                currentTime = 0;
                currentSpeed = walkSpeed;
            }
        }
    }

    private bool running;
    public bool Running {
        get { return running; }
        set {
            running = value;
            animator.SetBool( "running", running );
            if ( running ) {
                currentSpeed = runSpeed;
            } else {
                currentSpeed = walkSpeed;
            }
        }
    }

    #endregion Variables

    void currentTimePenalty( float penalty ) {
        currentTime = Mathf.Max( 0f, currentTime - penalty );
    }

    void Awake() {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();

        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void FixedUpdate() {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        grounded = Physics2D.OverlapBox( groundCheck.position, new Vector2( boxCollider2D.size.x, 0.001f ), 0f, whatIsGround );

        if ( vert > 0 ) {
            LookingUp = true;
        } else if ( vert < 0 ) {
            Crouched = true;
        } else {
            LookingUp = false;
            Crouched = false;
        }

        bool isLookingUp = animator.GetCurrentAnimatorStateInfo( 0 ).IsName( "playerLookingUp" );
        bool isCrouched = animator.GetCurrentAnimatorStateInfo( 0 ).IsName( "playerCrouched" );
        if ( isLookingUp || isCrouched ) {
            return;
        }

        if ( horz != 0f ) {
            if ( !Running && !FastWalking )
                Walking = true;

            if ( Input.GetButton( "Run" ) && !Running ) {
                FastWalking = true;

                if ( !Jumping && !Falling ) {
                    currentTime += Time.deltaTime;
                    if ( currentTime > timeUntilMaxSpeed ) {
                        Running = true;
                    }
                }

            }

            if ( Input.GetButtonUp( "Run" ) ) {
                FastWalking = Running = false;
            }

            transform.position += Vector3.right * horz * currentSpeed * Time.deltaTime;

            if ( horz > 0 ) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }

        } else {
            Walking = false;
            FastWalking = false;
            Running = false;
        }

        if ( grounded ) {
            Jumping = false;

            if ( Input.GetButtonDown( "Jump" ) ) {
                Jumping = true;
                rigidbody2D.AddForce( Vector2.up * jumpForce );
            }
        }

        Falling = ( rigidbody2D.velocity.y < -0.2f );
    }
}

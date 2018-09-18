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
    private float standSpeedCheck = 0.05f;
    private float fallingSpeedCheck = -0.2f;

    // PlayerController Variables
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private float currentSpeed;
    private float currentJumpImpulseValue;
    private float currentTime = 0f;
    private float currentSmoothStopValue;

    // Jumping
    [SerializeField]
    private int jumpImpulseStandValue = 120;
    [SerializeField]
    private int jumpImpulseWalkingValue = 125;
    [SerializeField]
    private int jumpImpulseFastWalkingValue = 130;
    [SerializeField]
    private int jumpImpulseRunningValue = 140;
    [SerializeField]
    [Range(0, 100)]
    private int gravityPercOnHold = 60;

    // Running
    [SerializeField]
    private float walkSpeed = 0.65f;
    [SerializeField]
    private float fastWalkSpeed = 1.3f;
    [SerializeField]
    private float runSpeed = 2f;
    [SerializeField]
    private float timeUntilMaxSpeed = 1f;
    [SerializeField]
    private float smoothWalkingStopValue = 0.5f;
    [SerializeField]
    private float smoothFastWalkingStopValue = 1f;
    [SerializeField]
    private float smoothRunningStopValue = 1.2f;
    [SerializeField]
    private float smoothAccelerationValue = 1f;

    // Verifies the ground
    public Transform groundCheck;
    public LayerMask whatIsGround;
    private bool grounded;

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

    private bool spinJumping;
    public bool SpinJumping {
        get { return spinJumping; }
        set {
            spinJumping = value;
            animator.SetBool( "spinjump", value );
        }
    }

    private bool runJumping;
    public bool RunJumping {
        get { return runJumping; }
        set {
            runJumping = value;
            animator.SetBool( "runjump", value );
        }
    }

    private bool falling;
    public bool Falling {
        get { return falling; }
        set {
            falling = value;
            if ( !Crouched && !SpinJumping && !RunJumping )
                animator.SetBool( "falling", falling );
            if ( falling && !Running ) {
                if ( !Jumping && !SpinJumping ) {
                    currentTimePenalty( currentTime / 2 );
                } else {
                    currentTimePenalty( currentTime / 4 );
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
            if ( walking ) {
                currentJumpImpulseValue = jumpImpulseWalkingValue;
            }
        }
    }

    private bool fastWalking;
    public bool FastWalking {
        get { return fastWalking; }
        set {
            fastWalking = value;
            animator.SetBool( "fastwalking", fastWalking );

            if ( fastWalking ) {
                currentJumpImpulseValue = jumpImpulseFastWalkingValue;
                currentSpeed = fastWalkSpeed;
                currentSmoothStopValue = smoothFastWalkingStopValue;
            } else {
                currentTimePenalty( currentTime / 3 );
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
                currentJumpImpulseValue = jumpImpulseRunningValue;
                currentSpeed = runSpeed;
                currentSmoothStopValue = smoothRunningStopValue;
            } else {
                currentTimePenalty( currentTime / 8 );
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
        currentSmoothStopValue = smoothWalkingStopValue;
        currentJumpImpulseValue = jumpImpulseStandValue;
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

        if ( grounded ) {
            Jumping = false;
            SpinJumping = false;
            RunJumping = false;

            if ( Input.GetButtonDown( "Jump" ) ) {
                if ( Running ) {
                    RunJumping = true;
                } else {
                    Jumping = true;
                }
                rigidbody2D.AddForce( Vector2.up * currentJumpImpulseValue );
            }

            if ( Input.GetButtonDown( "SpinJump" ) ) {
                SpinJumping = true;
                rigidbody2D.AddForce( Vector2.up * currentJumpImpulseValue );
            }
        }

        if ( Input.GetButton( "Jump" ) || Input.GetButton( "SpinJump" ) ) {
            rigidbody2D.gravityScale = gravityPercOnHold / 100f;
        } else {
            rigidbody2D.gravityScale = 1f;
        }

        bool isLookingUp = animator.GetCurrentAnimatorStateInfo( 0 ).IsName( "playerLookingUp" );
        bool isCrouched = animator.GetCurrentAnimatorStateInfo( 0 ).IsName( "playerCrouched" );
        if ( ( !Jumping && !SpinJumping && !RunJumping && !Falling ) && ( isLookingUp || isCrouched ) ) {
            return;
        }

        if ( horz != 0f ) {
            if ( !Running && !FastWalking )
                Walking = true;

            if ( Input.GetButton( "Run" ) && !Running ) {
                FastWalking = true;

                if ( !Jumping && !Falling && !SpinJumping ) {
                    currentTime += Time.deltaTime;
                    if ( currentTime > timeUntilMaxSpeed ) {
                        Running = true;
                    }
                }
            }

            if ( Input.GetButtonUp( "Run" ) ) {
                FastWalking = Running = false;
            }

            targetPosition = transform.position + Vector3.right * horz * 10;
            transform.position = Vector3.SmoothDamp( transform.position, targetPosition, ref velocity, smoothAccelerationValue, currentSpeed, Time.deltaTime );

            if ( horz > 0 ) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }

        } else {
            targetPosition = transform.position + Vector3.right * horz;
            transform.position = Vector3.SmoothDamp( transform.position, targetPosition, ref velocity, currentSmoothStopValue, currentSpeed, Time.deltaTime );
            if ( Mathf.Abs( velocity.x ) < standSpeedCheck ) {
                Walking = false;
                currentJumpImpulseValue = jumpImpulseStandValue;
                if ( grounded )
                    currentTime = 0f;
                currentSmoothStopValue = smoothWalkingStopValue;
            }
            if ( !RunJumping ) {
                FastWalking = false;
                Running = false;
            }
        }

        Falling = ( !grounded && rigidbody2D.velocity.y < fallingSpeedCheck );
    }
}

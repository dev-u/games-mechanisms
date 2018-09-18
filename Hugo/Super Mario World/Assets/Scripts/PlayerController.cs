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

    // Audio Variables
    private AudioSource audioSource;
    public enum ENUM_AUDIO {
        player_jumping,
        player_spin_jumping
    }
    public static Dictionary<ENUM_AUDIO, AudioClip> audio_player;

    // PlayerController Variables
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private float currentSpeed;
    private float currentJumpImpulseValue;
    private float currentTime = 0f;
    private float currentSmoothStopValue;
    private float currentAccelerationSmoothValue;
    private Vector3 defaultPosition;

    // Jumping
    [SerializeField]
    private int jumpImpulseStandValue = 120;
    [SerializeField]
    private int jumpImpulseWalkingValue = 130;
    [SerializeField]
    private int jumpImpulseFastWalkingValue = 140;
    [SerializeField]
    private int jumpImpulseRunningValue = 150;
    [SerializeField]
    [Range(0, 100)]
    private int gravityPercOnHold = 60;
    [SerializeField]
    [Range(0, 100)]
    private int spinJumpPercImpulse = 90;

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
    private float smoothJumpingStopValue = 20f;
    [SerializeField]
    private float smoothAccelerationValue = 1f;
    [SerializeField]
    private float smoothWalkTurnValue = 1.5f;

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

    private bool walkTurn;
    public bool WalkTurn {
        get { return walkTurn; }
        set {
            walkTurn = value;
            animator.SetBool( "walkturn", walkTurn );
            if ( walkTurn ) {
                currentAccelerationSmoothValue = smoothWalkTurnValue;
            } else {
                currentAccelerationSmoothValue = smoothAccelerationValue;
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

    void Play( AudioClip audio ) {
        audioSource.clip = audio;
        audioSource.Play();
    }

    void Awake() {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();

        currentSpeed = walkSpeed;
        currentSmoothStopValue = smoothWalkingStopValue;
        currentJumpImpulseValue = jumpImpulseStandValue;
        currentAccelerationSmoothValue = smoothAccelerationValue;
        defaultPosition = transform.position;

        audioSource = gameObject.GetComponent<AudioSource>();
        audio_player = new Dictionary<ENUM_AUDIO, AudioClip>()
        {
            { ENUM_AUDIO.player_jumping , Resources.Load<AudioClip>("Audios/smw_jump")},
            { ENUM_AUDIO.player_spin_jumping , Resources.Load<AudioClip>("Audios/smw_spin_jump")},
        };
    }

    void ResetPosition() {
        transform.position = defaultPosition;
        rigidbody2D.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate() {

        if ( transform.position.y < -0.5 )
            ResetPosition();

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
                Play( audio_player[ENUM_AUDIO.player_jumping] );
                if ( Running ) {
                    RunJumping = true;
                } else {
                    Jumping = true;
                }
                rigidbody2D.AddForce( Vector2.up * currentJumpImpulseValue );
            }

            if ( Input.GetButtonDown( "SpinJump" ) ) {
                Play( audio_player[ENUM_AUDIO.player_spin_jumping] );
                SpinJumping = true;
                rigidbody2D.AddForce( Vector2.up * currentJumpImpulseValue * spinJumpPercImpulse / 100f );
            }
        }

        if ( Input.GetButton( "Jump" ) || Input.GetButton( "SpinJump" ) ) {
            rigidbody2D.gravityScale = gravityPercOnHold / 100f;
        } else {
            rigidbody2D.gravityScale = 1f;
        }

        bool isLookingUp = animator.GetCurrentAnimatorStateInfo( 0 ).IsName( "playerLookingUp" );
        bool isCrouched = animator.GetCurrentAnimatorStateInfo( 0 ).IsName( "playerCrouched" );

        bool turn = ( velocity.x > 0 && horz < 0 ) || ( velocity.x < 0 && horz > 0 );
        WalkTurn = turn;

        if ( horz != 0f && !( Crouched && grounded ) ) {
            if ( !Running && !FastWalking )
                Walking = true;

            if ( Input.GetButton( "Run" ) && !Running ) {
                FastWalking = true;

                if ( !Jumping && !Falling && !SpinJumping && !Crouched ) {
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
            transform.position = Vector3.SmoothDamp( transform.position, targetPosition, ref velocity, currentAccelerationSmoothValue, currentSpeed, Time.deltaTime );


            if ( horz > 0 ) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }

        } else {
            float smooth = currentSmoothStopValue;
            if ( !grounded )
                smooth = smoothJumpingStopValue;

            targetPosition = transform.position + Vector3.right * horz;
            transform.position = Vector3.SmoothDamp( transform.position, targetPosition, ref velocity, smooth, currentSpeed, Time.deltaTime );
            if ( Mathf.Abs( velocity.x ) < standSpeedCheck ) {
                Walking = false;
                currentJumpImpulseValue = jumpImpulseStandValue;
                if ( grounded )
                    currentTime = 0f;
                currentSmoothStopValue = smoothWalkingStopValue;
            }
            if ( !RunJumping && !SpinJumping ) {
                FastWalking = false;
                Running = false;
            }
        }

        Falling = ( !grounded && rigidbody2D.velocity.y < fallingSpeedCheck );
    }
}

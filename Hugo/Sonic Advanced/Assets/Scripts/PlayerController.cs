using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool walking;

    [SerializeField]
    private float speed = 2f;

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
            transform.position += Vector3.right * speed * Time.deltaTime * horizontal;

            spriteRenderer.flipX = horizontal > 0 ? true : false;

        } else {
            animator.SetBool( "walking", false );
        }

        if ( Input.GetButtonDown( "Jump" ) ) {
            animator.SetBool( "jumping", true );
        }

	}
}

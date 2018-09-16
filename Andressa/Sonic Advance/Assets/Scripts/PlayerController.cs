﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool walking;

    // Use this for initialization
    private void Awake(){
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Start () {
    

	}
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal");

        walking = horizontal != 0 ? true : false;
        animator.SetBool("walking", walking);

        if(horizontal != 0){
            animator.SetBool("walking", true);
            if(horizontal >0){
                spriteRenderer.flipX = true;
            }else{
                spriteRenderer.flipX = false;
            }
        }else{
            animator.SetBool("walking", false);
        }
	}
}

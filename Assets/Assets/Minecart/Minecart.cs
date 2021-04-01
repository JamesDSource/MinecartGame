using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Controller2D), typeof(Interactable))]
public class Minecart : MonoBehaviour {
    SpriteRenderer sprite;
    Controller2D controller;
    Interactable interactable;

    public Vector2 velocity = new Vector2();
    const float gravity = -14;
    float acceleration = 2.5f;
    float maxMovementSpeed = 6;
    float jumpVelocity = 8;
    float targetMovementSpeed = 0;
    float jump = 0;

    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        interactable = GetComponent<Interactable>();
        interactable.interactedWith = InteractedWith;
    }
    
    void Update() {
        velocity.y += gravity*Time.deltaTime;

        if(controller.collisions.below) {
            if(jump > 0) {
                velocity.y = jumpVelocity;
                jump = 0;
            }
            else {
                velocity.x = Numbers.Approach(velocity.x, targetMovementSpeed, acceleration*Time.deltaTime);
            }
        }
        if(jump > 0) {
            jump -= Time.deltaTime;
        }

        controller.Move(velocity*Time.deltaTime);

        if(controller.collisions.below) {
            velocity.y = 0;
        }

        targetMovementSpeed = 0;
    }

    public void Movement() {
        targetMovementSpeed = 0;
        if(Input.GetKey(KeyCode.D)) {
            targetMovementSpeed += 1;
        }
        if(Input.GetKey(KeyCode.A)) {
            targetMovementSpeed -= 1;
        }

        targetMovementSpeed *= maxMovementSpeed;
    
        if(Input.GetKeyDown(KeyCode.W)) {
            jump = 0.5f;
        }
    }

    void InteractedWith(Player player) {
        player.minecart = this;
    }

}
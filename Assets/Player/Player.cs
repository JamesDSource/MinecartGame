using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {
    
    const float gravity = -14;
    const float jumpVelocity = 8;
    Vector2 velocity;
    float moveSpeed = 3;

    Controller2D controller;

    void Start() {
        controller = GetComponent<Controller2D>();
    }

    void Update() {
        velocity.x = 0;
        if(Input.GetKey(KeyCode.D)) {
            velocity.x += 1;
        }
        if(Input.GetKey(KeyCode.A)) {
            velocity.x -= 1;
        }

        velocity.x *= moveSpeed;
        velocity.y += gravity*Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.W) && controller.collisions.below) {
            velocity.y = jumpVelocity;
        }

        controller.Move(velocity*Time.deltaTime);
        
        if(controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
    }
}
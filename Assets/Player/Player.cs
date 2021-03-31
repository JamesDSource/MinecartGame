using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {
    public enum PlayerState {
        Free,
        Minecart
    }
    public PlayerState state = PlayerState.Free;
    
    const float gravity = -14;
    const float jumpVelocity = 8;
    float jump = 0;
    Vector2 velocity;
    float momentum = 0;
    float moveSpeed = 3;

    Controller2D controller;

    Interactable closestInteractable;
    float interactableRadius = 2;

    public Minecart minecart;

    void Start() {
        controller = GetComponent<Controller2D>();
    }

    void Update() {
        switch(state) {
            case PlayerState.Free:
                Movement();
                FindInteractables(); 
                if(minecart) {
                    state =  PlayerState.Minecart;
                }
                break;
            case PlayerState.Minecart:
                if(!minecart) {
                    state = PlayerState.Free;
                    break;
                }

                minecart.Movement();
                Vector3 minecartPos = minecart.transform.position;
                transform.position = new Vector3(minecartPos.x, minecartPos.y + 0.5f, minecartPos.z);

                if(Input.GetKeyDown(KeyCode.Space)) {
                    velocity.y = jumpVelocity + Mathf.Max(minecart.velocity.y, 0);
                    momentum = minecart.velocity.x;
                    minecart = null;
                }
                break;
        }     
    }

    void Movement() {
        if(controller.collisions.below) {
            momentum = Numbers.Approach(momentum, 0, 5f*Time.deltaTime);
        }

        velocity.x = 0;
        if(Input.GetKey(KeyCode.D)) {
            velocity.x += 1;
        }
        if(Input.GetKey(KeyCode.A)) {
            velocity.x -= 1;
        }

        velocity.x *= moveSpeed;
        velocity.x += momentum;
        velocity.y += gravity*Time.deltaTime;

        if(Mathf.Sign(velocity.x) != Mathf.Sign(momentum)) {
            momentum = 0;
        }

        if(Input.GetKeyDown(KeyCode.W)) {
            jump = 0.5f;
        }

        if(jump > 0 && controller.collisions.below) {
            velocity.y = jumpVelocity;
            jump = 0;
            momentum = 0;
        }
        else if(jump > 0) {
            jump -= Time.deltaTime;
        }

        controller.Move(velocity*Time.deltaTime);
        
        if(controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
    }

    void FindInteractables() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactableRadius);
        closestInteractable = null;

        float closestDistance = Mathf.Infinity;
        foreach(Collider2D collider in  colliders) {
            Interactable interactable = collider.GetComponent<Interactable>();
            
            if(interactable) {
                float dist = Vector2.Distance(collider.transform.position, transform.position);
                
                if(dist < closestDistance) {
                    closestInteractable = interactable;
                    closestDistance = dist;
                }
            }
        }
        if(closestInteractable && Input.GetKeyDown(KeyCode.Space)) {
            closestInteractable.interactedWith(this);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Controller2D), typeof(Interactable))]
public class Minecart : MonoBehaviour {
    SpriteRenderer sprite;
    Controller2D controller;
    Interactable interactable;

    Vector2 velocity = new Vector2();
    const float gravity = -14;

    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        interactable = GetComponent<Interactable>();
        interactable.interactedWith = InteractedWith;
    }
    
    void Update() {
        velocity.y += gravity*Time.deltaTime;
        controller.Move(velocity*Time.deltaTime);

        if(controller.collisions.below) {
            velocity.y = 0;
        }
    }

    void InteractedWith() {
        print("Interacted");
    }
}

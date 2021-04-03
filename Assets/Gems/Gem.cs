using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Controller2D))]
public class Gem : MonoBehaviour {
    [SerializeField] Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    Interactable interactable;

    Controller2D controller;
    const float gravity = -14;
    Vector2 velocity;

    public bool collected = false;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length-1)];
        
        interactable = GetComponent<Interactable>();
        interactable.interactedWith = InteractedWith;

        controller = GetComponent<Controller2D>();
    }

    void Update() {
        velocity.y += gravity*Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(controller.collisions.below) {
            velocity.y = 0;
        }
    }

    void InteractedWith(Player player) {
        player.gemHolding = this;
    }
}

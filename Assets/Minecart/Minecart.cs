using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Controller2D), typeof(Interactable))]
public class Minecart : MonoBehaviour {
    [SerializeField] LayerMask railMask;
    RaycastHit2D onRail;
    SpriteRenderer sprite;
    Controller2D controller;
    Interactable interactable;

    public Vector2 velocity = new Vector2();
    const float gravity = -14;
    float acceleration = 2.5f;
    float offRailsAcceleration = 0.5f;
    float offRailsDeceleration = 4f;
    float maxMovementSpeed = 6;
    float offRailsMaxMovementSpeed = 2;
    float jumpVelocity = 8;
    float offRailsJumpVelocity = 4;
    float targetMovementSpeed = 0;
    float jump = 0;

    AudioSource audioSource;
    [SerializeField] AudioClip moveOnTracks;
    [SerializeField] AudioClip moveOffTracks;
    [SerializeField] AudioClip gemCollected;
    AudioClip lastClip = null;

    public int gems = 0;

    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        interactable = GetComponent<Interactable>();
        audioSource = GetComponent<AudioSource>();

        interactable.interactedWith = InteractedWith;
        interactable.action = "enter mine cart";
    }
    
    void Update() {
        onRail = Physics2D.Raycast(transform.position, new Vector2(0, -1), 1, railMask);
        velocity.y += gravity*Time.deltaTime;

        if(controller.collisions.below) {
            if(jump > 0) {
                velocity.y = onRail ? jumpVelocity : offRailsJumpVelocity;
                jump = 0;
            }
            else {
                float acc;
                if(Mathf.Abs(targetMovementSpeed) < Mathf.Abs(velocity.x) || Mathf.Sign(targetMovementSpeed) != Mathf.Sign(velocity.x)) {
                    acc = onRail ? acceleration : offRailsDeceleration;
                }
                else {
                    acc = onRail ? acceleration : offRailsAcceleration;
                }

                velocity.x = Numbers.Approach(velocity.x, targetMovementSpeed, acc*Time.deltaTime);
            }
        }
        if(jump > 0) {
            jump -= Time.deltaTime;
        }

        controller.Move(velocity*Time.deltaTime);

        if(controller.collisions.below || controller.collisions.above) {
            velocity.y = 0;
        }

        if(controller.collisions.left || controller.collisions.right) {
            velocity.x = 0;
        }

        targetMovementSpeed = 0;

        if(velocity.x != 0 && controller.collisions.below) {
            if(onRail) {
                audioSource.clip = moveOnTracks;
            }
            else {
                audioSource.clip = moveOffTracks;
            }
            audioSource.volume = Mathf.Abs(velocity.x) / (onRail ? maxMovementSpeed : offRailsMaxMovementSpeed);
        }
        else {
            audioSource.clip = null;
        }

        if(audioSource.clip != lastClip) {
            if(audioSource.clip != null) {
                audioSource.Play();
            }
            lastClip = audioSource.clip;
        }
    }

    public void Movement() {
        targetMovementSpeed = 0;
        if(Input.GetKey(KeyCode.D)) {
            targetMovementSpeed += 1;
        }
        if(Input.GetKey(KeyCode.A)) {
            targetMovementSpeed -= 1;
        }

        targetMovementSpeed *= onRail ? maxMovementSpeed : offRailsMaxMovementSpeed;
    
        if(Input.GetKeyDown(KeyCode.W)) {
            jump = 0.5f;
        }
    }

    void InteractedWith(Player player) {
        player.minecart = this;
    }

    void OnTriggerEnter2D(Collider2D other) {
        Gem gem = other.GetComponent<Gem>();
        if(gem) {
            gems++;
            gem.collected = true;
            audioSource.PlayOneShot(gemCollected);
            Destroy(other.gameObject);
        }
    }
}
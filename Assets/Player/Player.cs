using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D), typeof(SpriteRenderer), typeof(Animator))]
public class Player : MonoBehaviour {
    [SerializeField] Camera cam;

    public enum PlayerState {
        Free,
        Minecart,
        Carry,
        Dead
    }
    public PlayerState state = PlayerState.Free;
    bool building = false;

    const float gravity = -14;
    const float jumpVelocity = 8;
    float jump = 0;
    Vector2 velocity;
    float moveSpeed = 3;
    float carryMoveSpeed = 1.5f;

    Controller2D controller;

    Interactable closestInteractable;
    float interactableRadius = 1f;
    bool displayInterAction = false;

    [SerializeField] RailController railController;
    public int tracksHeld = 0;

    public Minecart minecart;
    public Gem gemHolding;

    [SerializeField] GameObject pickAxe;
    float pickDelay = 0.5f;
    float pickTimer = 0;

    [SerializeField] UIManager UIObject;

    Animator animator;
    SpriteRenderer spriteRenderer;
    
    AudioSource audioSource;
    [SerializeField] AudioClip railPurchased;

    void Start() {
        controller = GetComponent<Controller2D>();
        UIObject = Object.FindObjectOfType<UIManager>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        // State machine
        switch(state) {
            case PlayerState.Free:
                Movement(moveSpeed);
                FindInteractables();
                BuildingTracks();
                ThrowPickAxe();
                if(minecart) {
                    state =  PlayerState.Minecart;
                }
                else if(gemHolding) {
                    state = PlayerState.Carry;
                }
                break;
            case PlayerState.Minecart:
                BuildingTracks();
                ThrowPickAxe();
                if(minecart && (minecart.transform.position - transform.position).magnitude > 5f) {
                    minecart = null;
                }

                if(!minecart) {
                    state = PlayerState.Free;
                    break;
                }

                if(minecart.gems >= 3) {
                    audioSource.PlayOneShot(railPurchased);
                }
                while(minecart.gems >= 3) {
                    minecart.gems -= 3;
                    tracksHeld += 4;
                }

                animator.SetBool("isJumping", false);
                animator.SetBool("isRunning", false);
                if(minecart.velocity.x != 0) {
                    spriteRenderer.flipX = minecart.velocity.x < 0;
                }

                minecart.Movement();
                Vector3 minecartPos = minecart.transform.position;
                transform.position = new Vector3(minecartPos.x, minecartPos.y + 0.5f, minecartPos.z);

                if(Input.GetKeyDown(KeyCode.Space)) {
                    velocity.y = jumpVelocity + Mathf.Clamp(minecart.velocity.y, 0, 3);
                    minecart = null;
                }
                break;
            case PlayerState.Carry:
                Movement(carryMoveSpeed);
                BuildingTracks();
                if(gemHolding.collected || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) {
                    gemHolding = null;
                }
                if(!gemHolding) {
                    state = PlayerState.Free;
                    break;
                }

                gemHolding.transform.position = transform.position;
                gemHolding.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                break;
            case PlayerState.Dead:
                Movement(0f, false);
                break;
        }

        if(displayInterAction) {
            UIObject.actionStr = closestInteractable.action;
            displayInterAction = false;
        }
        else {
            UIObject.actionStr = "";
        }
        
    }

    void Movement(float speed, bool takeInputs = true) {

        velocity.x = 0;
        if(takeInputs) {
            if(Input.GetKey(KeyCode.D)) {
                velocity.x += 1;
            }
            if(Input.GetKey(KeyCode.A)) {
                velocity.x -= 1;
            }
        }

        animator.SetBool("isRunning", velocity.x != 0);
        if(velocity.x != 0) {
            spriteRenderer.flipX = velocity.x == -1;
        }


        velocity.x *= speed;
        velocity.y += gravity*Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.W)) {
            jump = 0.5f;
        }

        if(jump > 0 && controller.collisions.below && takeInputs) {
            velocity.y = jumpVelocity;
            jump = 0;
            animator.SetBool("isJumping", true);
        }
        else if(jump > 0) {
            jump -= Time.deltaTime;
        }

        controller.Move(velocity*Time.deltaTime);
        
        if(controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
            animator.SetBool("isJumping", false);
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
                
                if(dist < closestDistance && interactable.isInteractable) {
                    closestInteractable = interactable;
                    closestDistance = dist;
                }
            }
        }
        if(closestInteractable) {
            displayInterAction = true;
            if(Input.GetKeyDown(KeyCode.Space)) {
                closestInteractable.interactedWith(this);
            }
        }
    }

    void BuildingTracks() {
        if(!railController) {
            print("Rail controller not set");
        }
        else {
            if(Input.GetKeyDown(KeyCode.Tab)) {
                building = !building;
            }

            if(building) {
                railController.GetInputs();
            }
        }
    }

    void ThrowPickAxe() {
        if(pickTimer > 0) {
            pickTimer -= Time.deltaTime;
        }

        if(!building && Input.GetMouseButtonUp(0) && pickTimer <= 0) {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.25f, 0);
            GameObject newGO = Instantiate(pickAxe, spawnPos, new Quaternion());
            PickAxe pickComp = newGO.GetComponent<PickAxe>();
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            pickComp.Launch((mousePos - spawnPos).normalized);
            pickTimer = pickDelay;
            animator.SetBool("hasThrown", true);
        }
    }

    void ThrowAnimFinish() {
        animator.SetBool("hasThrown", false);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            Damage();
        }
        //assuming the health pickups have collider and health tag
        else if (collision.gameObject.tag == "Health") {
            //heal to full
            UIObject.GetComponent<UIManager>().SetHealth(5);
        }
    }

    public void Damage() {
        UIObject.GetComponent<UIManager>().TakeDamage(1);
        if(UIObject.slider.value <= 0) {
            state = PlayerState.Dead;
            animator.SetBool("dead", true);
        }
    }

}
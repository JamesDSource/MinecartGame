using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {
    [SerializeField] Camera cam;

    public enum PlayerState {
        Free,
        Minecart,
        Carry,
        Dead
    }
    PlayerState state = PlayerState.Free;
    bool building = false;

    const float gravity = -14;
    const float jumpVelocity = 8;
    float jump = 0;
    Vector2 velocity;
    float momentum = 0;
    float moveSpeed = 3;
    float carryMoveSpeed = 1.5f;

    Controller2D controller;

    Interactable closestInteractable;
    float interactableRadius = 2;

    [SerializeField] RailController railController;
    public int tracksHeld = 0;

    public Minecart minecart;
    public Gem gemHolding;

    [SerializeField] GameObject pickAxe;

    [SerializeField] UIManager UIObject;

    void Start() {
        controller = GetComponent<Controller2D>();
        UIObject = Object.FindObjectOfType<UIManager>();
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
            case PlayerState.Carry:
                if(gemHolding.collected || velocity.y > 0 || Input.GetKeyDown(KeyCode.Space)) {
                    gemHolding = null;
                }
                if(!gemHolding) {
                    state = PlayerState.Free;
                    break;
                }
                Movement(carryMoveSpeed);
                BuildingTracks();
                gemHolding.transform.position = transform.position;
                break;
        }     
    }

    void Movement(float speed) {
        if(controller.collisions.below) {
            momentum = Numbers.Approach(momentum, 0, 8f*Time.deltaTime);
        }

        velocity.x = 0;
        if(Input.GetKey(KeyCode.D)) {
            velocity.x += 1;
        }
        if(Input.GetKey(KeyCode.A)) {
            velocity.x -= 1;
        }

        velocity.x *= speed;
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

    void BuildingTracks() {
        if(!railController) {
            print("Rail controller not set");
        }
        else {
            if(Input.GetKeyDown(KeyCode.Tab)) {
                building = !building;
            }

            if(building) {
                railController.GetInputs(ref tracksHeld);
            }
        }
    }

    void ThrowPickAxe() {
        if(!building) {
            if(Input.GetMouseButtonUp(0)) {
                Vector3 spawnPos = transform.position + new Vector3(0, 0.25f, 0);
                GameObject newGO = Instantiate(pickAxe, spawnPos, new Quaternion());
                PickAxe pickComp = newGO.GetComponent<PickAxe>();
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                pickComp.Launch((mousePos - spawnPos).normalized);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            UIObject.GetComponent<UIManager>().TakeDamage(1);
        }
        //assuming the health pickups have collider and health tag
        else if (collision.gameObject.tag == "Health")
        {
            //heal to full
            UIObject.GetComponent<UIManager>().SetHealth(5);
        }
    }
    

}
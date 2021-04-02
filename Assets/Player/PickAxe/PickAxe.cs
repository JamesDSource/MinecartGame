using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PickAxe : MonoBehaviour {

    [SerializeField] LayerMask collidingLayers;
    float launchSpeed = 40;
    float rotationStep = 500;
    float rotation = 0;
    float gravity = -10;
    Vector3 velocity;

    void Start() {
        
    }

    void Update() {
        velocity.y += gravity*Time.deltaTime;
        transform.position += velocity*Time.deltaTime;

        rotation += rotationStep * Time.deltaTime * (velocity.x > 0 ? -1 : 1);
        transform.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other) {
        Destroy(this.gameObject);
    }

    public void Launch(Vector2 direction) {
        velocity = direction*launchSpeed;
    }
}

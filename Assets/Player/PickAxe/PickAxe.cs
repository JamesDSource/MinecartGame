using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PickAxe : MonoBehaviour {

    [SerializeField] LayerMask collidingLayers;
    float gravity = -14;
    Vector2 velocity;

    void Start() {
        
    }

    void Update() {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        Destroy(this.gameObject);
    }
}

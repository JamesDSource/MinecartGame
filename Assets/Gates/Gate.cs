using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable), typeof(Animator))]
public class Gate : MonoBehaviour {
    [SerializeField] RailController railController;
    Interactable interactable;
    Animator animator;

    void Start() {
        interactable = GetComponent<Interactable>();
        interactable.interactedWith = LevelEnd;

        animator = GetComponent<Animator>();
    }
    void Update() {
        if(railController.GatesConnected()) {
            interactable.isInteractable = true;
            animator.SetBool("complete", true);
        }
        else {
            interactable.isInteractable = false;
            animator.SetBool("complete", false);
        }
    }

    void LevelEnd(Player player) {
        print("Level ended");
    }
}

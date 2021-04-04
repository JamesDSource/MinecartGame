using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        switch(other.gameObject.tag) {
            case "Player":
                Player player = other.GetComponent<Player>();
                while(player.state != Player.PlayerState.Dead) {
                    player.Damage();
                }
                break;
            case "Mine cart":
                GameObject[] respawn = GameObject.FindGameObjectsWithTag("Cart respawn");
                if(respawn.Length > 0) {
                    other.transform.position = respawn[0].transform.position;
                }
                break;
        }
    }

}

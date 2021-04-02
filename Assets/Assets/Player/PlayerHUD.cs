using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {
    [SerializeField] Player player;

    [SerializeField] Text railText;

    void Update() {
        railText.text = "Rails:" + player.tracksHeld;
    }
}

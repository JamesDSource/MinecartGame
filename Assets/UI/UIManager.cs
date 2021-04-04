using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] Player player;
    [SerializeField] Minecart minecart;
    int _tracks = 0;
    int _gems = 0;
    [SerializeField] Text gemsText;
    [SerializeField] Text tracksText;
    [SerializeField] Text actionText;
    [SerializeField] Text deathText;

    public Slider slider;

    public string actionStr = "";

    void Start()
    {
        slider.maxValue = 5;   
    }
    void Update()
    {
        _tracks = player.tracksHeld;
        _gems = minecart.gems;
        gemsText.text = ("" + _gems);
        tracksText.text = ("" + _tracks);
        
        if(actionStr != "") {
            actionText.text = "Press SPACE to " + actionStr;
        }
        else {
            actionText.text = "";
        }

        if(player.state == Player.PlayerState.Dead) {
            deathText.text = "You Died\nPress \"R\" to restart";
        }

    }

    public void SetHealth(int health) {
        slider.value = health;
    }

    public void TakeDamage(int damage) {
        slider.value -= damage;
    }
}

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

    public Slider slider;

    void Start()
    {
        slider.maxValue = 5;   
    }
    void Update()
    {
        //_tracks = player.tracksHeld;
        _gems = minecart.gems;
        gemsText.text = ("" + _gems);
        tracksText.text = ("" + _tracks);

    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    public void TakeDamage(int damage)
    {
        slider.value -= damage;
    }
}

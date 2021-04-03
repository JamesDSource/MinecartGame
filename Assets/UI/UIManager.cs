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

    void Update()
    {
        _tracks = player.tracksHeld;
        _gems = minecart.gems;
        gemsText.text = ("" + _gems);
        tracksText.text = ("" + _tracks);
    }
}

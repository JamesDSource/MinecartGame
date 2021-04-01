using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    int _tracks;
    int _gems;

    public Text gemsText;
    public Text tracksText;

    // Start is called before the first frame update
    void Start()
    {
        _gems = 0;
        _tracks = 0;
    }

    // Update is called once per frame
    void Update()
    {
        gemsText.text = ("" + _gems);
        tracksText.text = ("" + _tracks);
        
        if(Input.GetKeyDown("g"))
        {
            IncreaseCollection();
        }
        
        if(Input.GetKeyDown("f"))
        {
            IncreaseTracks();
        }
    }

    void IncreaseCollection()
    {
        _gems += 1;
    }

    void IncreaseTracks()
    {
        _tracks += 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManaging : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //only works in build
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            //our only scene is SampleScene
            //pressing R loads the scene again
            //adding more scenes involves working with build settings
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

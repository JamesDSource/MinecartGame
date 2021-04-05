using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManaging : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            //only works in build
            Quit();
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            //our only scene is SampleScene
            //pressing R loads the scene again
            //adding more scenes involves working with build settings
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void GotoNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoPlayScene()
    {
        SceneManager.LoadScene("Level1");
    }

    public void CreditScreen()
    {
        SceneManager.LoadScene("Credit Scene");
    }

    public void TitleScreen()
    {
        SceneManager.LoadScene("TitleScene");
    }


    public void Quit()
    {
        Application.Quit();
    }
}

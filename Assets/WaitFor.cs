using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitFor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LoadScene());
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GlobalVariables.Trial_counter < 3)
                SceneManager.LoadScene(0);
            else
                Application.Quit();
        }
    }


        IEnumerator LoadScene()
    {
        yield return new WaitForSecondsRealtime(2);
        if (GlobalVariables.Trial_counter < 3)
            SceneManager.LoadScene(0);
        else
            Application.Quit();
    }
}

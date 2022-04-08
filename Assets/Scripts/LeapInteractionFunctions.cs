using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LeapInteractionFunctions : MonoBehaviour
{
    public GameObject Video;
    public VideoPlayer VideoPlayer;
    public GameObject Nav;
    // Start is called before the first frame update
    void Start()
    {
        Video.SetActive(false);
        VideoPlayer.Stop();
        Nav.SetActive(false);

    }

    public void NavigationScreen() {

       Debug.Log("NavigationButtoPressed");
        Nav.SetActive(true);
        Video.SetActive(false);
    }
    public void MusicScreen()
    {
        Video.SetActive(true);
        Nav.SetActive(false);
        VideoPlayer.Play();
        Debug.Log("MusicPlaying");

    }
    public void CarScreen()
    {

        Debug.Log("CarScreen");

    }
    public void OptionsScreen()
    {

        Debug.Log("OptionScreem");

    }
}

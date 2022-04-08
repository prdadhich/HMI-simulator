using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
//iVJRZD_swzQHVHYL8ZhWJGCDivN2qSXuYL3eTBNlrvj6
//https://api.eu-gb.assistant.watson.cloud.ibm.com/instances/0784f3b6-04c5-4044-821e-a6c7c508de84
//74cfa2f6-93ad-4810-8b38-2106a2fd062d
public class CallFunctionsWithAudio : MonoBehaviour
{
    private string _speechText;
    public GameObject Video;
    public VideoPlayer VideoPlayer;

    int count = 1;
    FollowPath follow = new FollowPath();
    // Start is called before the first frame update
    void Start()
    {
        Video.SetActive(false);
        VideoPlayer.Stop();

    }

        // Update is called once per frame
        void LateUpdate()
        {
            _speechText = WatsonAPI.SavedText.ToLower();
            if (_speechText.Contains("milan") && _speechText.Contains("location") && count == 1)
            {
                GotoMilan();
            }
            if (_speechText.Contains("sugar") && count == 1)
            {
                PlaySugar();
            }
            if (_speechText.Contains("stop") && _speechText.Contains("music") && count == 1)
            {
                StopMusic();
            }
            if ( _speechText.Contains("restaurant") )
            {
                GotoRestaurant();
            }


        }
        void GotoMilan()
        {
            Debug.Log("Watson Working");
            count = 0;
        }

        void PlaySugar()
        {
            Video.SetActive(true);
            VideoPlayer.Play();
            count = 0;
            Debug.Log("MusicPlaying");
        }
        void StopMusic()
        {
            Video.SetActive(false);
            VideoPlayer.Stop();
            count = 0;
        }
        void GotoRestaurant()
        {
            Debug.Log("Going to Restaurant");
            follow.GoToRestaurant();
            
        }



    
}

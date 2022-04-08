using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts
{
    public class ManageGame : MonoBehaviour
    {


        public static bool is5G = false;
        public static bool isAudioOn = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                if (is5G == false)
                    StartCoroutine(Turn5GOn());
                else
                 if (is5G == true)
                    StartCoroutine(Turn5Goff());

            if (Input.GetKeyDown(KeyCode.Y))
                if (isAudioOn == false)
                    StartCoroutine(TurnAudioOn());
                else
                 if (isAudioOn == true)
                    StartCoroutine(TurnAudiooff());

        }

        IEnumerator TurnAudiooff()
        {
            isAudioOn = false;
            Debug.Log("Audio turned off ");
            yield return new WaitForSecondsRealtime(2);
        }

        IEnumerator TurnAudioOn()
        {
            isAudioOn = true;
            Debug.Log("Audio turned On");
            yield return new WaitForSecondsRealtime(2);
        }

        IEnumerator Turn5GOn()
        {
           

            is5G = true;
            Debug.Log("5G turned On");
            yield return new WaitForSecondsRealtime(2);

        }

        IEnumerator Turn5Goff()
        {
            
            is5G = false;
            Debug.Log("5g turned off ");
            yield return new WaitForSecondsRealtime(2);


        }



    }
        


    
}
        
            

          
        
        
   
        
    


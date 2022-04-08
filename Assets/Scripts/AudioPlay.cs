using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts 
{
    public class AudioPlay : MonoBehaviour
    {
        private AudioSource _audio;
        private AudioClip audioClip;
        private bool _playAudio = false;
        AudioClip audioClipHuman;
        AudioClip audioClipCar;
        AudioClip audioDestination;


        // Start is called before the first frame update
        void Start()
        {
            _audio = GetComponent<AudioSource>();
            
             audioClipHuman = Resources.Load<AudioClip>("Audios/HumanApproaching") as AudioClip;
             audioClipCar = Resources.Load<AudioClip>("Audios/CarApproaching") as  AudioClip;
             audioDestination = Resources.Load<AudioClip>("Audios/Destination") as AudioClip;

        }
        
        // Update is called once per frame
        void Update()
        {
            
            if (ManageGame.isAudioOn) 
            {
                if (CollisionWithPlayer.isCollision)
                    StartCoroutine(PlayAudio());
                
            }
            if (CollisionWithPlayer.whichGameObject.CompareTag("Destination") && CollisionWithPlayer.isCollision)
            {

                _playAudio = true;
                StartCoroutine(PlayDestinationAudio());
                
               


            }

        }
        IEnumerator PlayAudio()
        {
            if (CollisionWithPlayer.whichGameObject.CompareTag("HumanNotify"))
            {
                
                _playAudio = true;
                PlayHumanAudio();
               
               
                yield return new WaitForSecondsRealtime(1);

                _playAudio = false;


            }

            
                if (CollisionWithPlayer.whichGameObject.CompareTag("CarNotify"))
            {
                
                _playAudio = true;
                PlayCarAudio();
                yield return new WaitForSecondsRealtime(2);
                _playAudio = false;
            }
               




        }

        private void PlayHumanAudio()

        {
            if (_playAudio == true) 
            {


                _audio.clip = audioClipHuman;  
                _audio.Play();
                
               
            }
        }

        private void PlayCarAudio() 
        {
            _audio.clip = audioClipCar;

            _audio.Play();
            

        }

        IEnumerator PlayDestinationAudio()
        {
            _audio.clip = audioDestination;

            _audio.Play();
           yield  return new WaitForSecondsRealtime(2);

            _playAudio = false;

        }

    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CarAnimation : MonoBehaviour
    {
        private Image carImage;
        private Animation carFade;
        private Animator carFadeAnimator;
        private bool _isEnabled = false;
        // Start is called before the first frame update
        void Start()
        {
            carFade = GetComponent<Animation>();
            carFadeAnimator = GetComponent<Animator>();
            carImage = GetComponent<Image>();
            if (_isEnabled == false)
            {

                carFade.enabled = false;
                carFadeAnimator.enabled = false;
                carImage.enabled = false;

            }
        }

        // Update is called once per frame
        void Update()
        {
            if (CollisionWithPlayer.isCar)
            {
                _isEnabled = true;
                StartCoroutine("ShowCar");

            }
            HideCar();


        }
        IEnumerator ShowCar()
        {
            if (_isEnabled == true)
            {

                carFade.enabled = true;
                carFadeAnimator.enabled = true;
                carImage.enabled = true;

                yield return new WaitForSecondsRealtime(1f);
                _isEnabled = false;
            }


        }

        private void HideCar()
        {
            if (_isEnabled == false)
            {

                carFade.enabled = false;
                carFadeAnimator.enabled = false;
                carImage.enabled = false;

            }

        }

    }





}

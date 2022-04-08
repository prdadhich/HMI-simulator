using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HumanAnimation : MonoBehaviour
    {
        private Image humanImage;
        private Animation humanFade;
        private Animator humanFadeAnimator;
        private bool _isEnabled = false;
        Renderer mTexture;
        public Texture humanTextureu;
        // Start is called before the first frame update
        void Start()
        {
            humanFade = GetComponent<Animation>();
            humanFadeAnimator = GetComponent<Animator>();
            humanImage = GetComponent<Image>();
            mTexture = gameObject.GetComponent<Renderer>();
            humanTextureu = Resources.Load<Texture>("Images/walking") as Texture;
            
            if (_isEnabled == false) 
            {

                humanFade.enabled = false;
                humanFadeAnimator.enabled = false;
                humanImage.enabled = false;
                mTexture.material.mainTexture = humanTextureu;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (CollisionWithPlayer.isHuman)
            {
                _isEnabled = true;
                StartCoroutine("ShowHuman");

            }
            HideHuman();
               

        }
        IEnumerator ShowHuman()
        {
            if (_isEnabled == true)
            {

                humanFade.enabled = true;
                humanFadeAnimator.enabled = true;
                humanImage.enabled = true;
              
                yield return new WaitForSecondsRealtime(2f);
                _isEnabled = false;
            }

          
        }

        private void HideHuman() 
        {
            if (_isEnabled == false)
            {

                humanFade.enabled = false;
                humanFadeAnimator.enabled = false;
                humanImage.enabled = false;
                
            }
      
        }
        
    }





}

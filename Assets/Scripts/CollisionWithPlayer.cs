using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
    public class CollisionWithPlayer : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> notify = new List<GameObject>();
        [SerializeField]
        private List<GameObject> obstacle = new List<GameObject>();
        public Text text1;
        private bool moveObstacle = false;
        public static GameObject whichGameObject;
        public static bool isCollision = false;
        public static bool isHuman = false;
        public static bool isCar = false;
        public static bool isDestination = false;
        [SerializeField]
        private Animator _humanObstacleAnimation;


        public static int indexofCollider;

        private void Start()
        {
            _humanObstacleAnimation.enabled = false;
     
        }

        // Use this for initialization

        void LateUpdate()
        {
  
            if ( isCar)                     // checking the condition if the player hits the car collider 
                obstacle[0].transform.Translate(0, 0, 0.09f); // if above condition is true move the car obstacle
            if (moveObstacle && whichGameObject.CompareTag("HumanNotify"))  //checking if the player hits human obstacle collider
            {
                _humanObstacleAnimation.enabled = true;
            
            }
 
        }
        public static int SendIndex(int i)
        {
            indexofCollider = i;
            return indexofCollider;

        }

        private void OnTriggerEnter(Collider other)
        {
            whichGameObject = other.gameObject; 
            //check player hits which obstacle 
            if (other.gameObject.CompareTag("CarNotify")|| other.gameObject.CompareTag("HumanNotify") || other.gameObject.CompareTag("Destination")) 
            
            {
                isCollision = true;
                Debug.Log(" Collison happened ");
                CollisonDetected(other);
            
            }
            if (other.gameObject.CompareTag("HumanNotify"))
            {
                isHuman = true;
            
            }
            if (other.gameObject.CompareTag("CarNotify"))
            {
                isCar = true;

            }
            if (other.gameObject.CompareTag("Destination"))
            {
                isDestination = true;

            }
        }

        private void CollisonDetected(Collider collider) 
        {
            
            if ((notify.Contains(collider.gameObject)) || collider.gameObject.CompareTag("CarNotify")  ) 
            {
                int i = notify.IndexOf(collider.gameObject);
                Debug.Log(i);
                SendIndex(i);
                StartCoroutine(MoveObstacle());
                
                if (ManageGame.is5G)
                {
                    Invoke(nameof(SendMessage), .3f);
                    Invoke(nameof(DestroyMessage), 5f);


                }
                

            }
            
        }

 

        IEnumerator MoveObstacle() 
        {

            moveObstacle = true; 
            yield return new WaitForSecondsRealtime(20f);
            moveObstacle = false; 
        }

        private void SendMessage() 
        {   
            text1.text= "Obstacle Detected";
        }

        private void DestroyMessage() 
        {
            text1.text = "";
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("CarNotify") || other.gameObject.CompareTag("HumanNotify") || other.gameObject.CompareTag("Destination"))

            {
                isCollision = false;


            }
            if (other.gameObject.CompareTag("HumanNotify"))
            {
                isHuman = false;

            }
            if (other.gameObject.CompareTag("CarNotify"))
            {
                isCar = false;

            }
            if (other.gameObject.CompareTag("Destination"))
            {
                isDestination = false;

            }
        }



    }


}
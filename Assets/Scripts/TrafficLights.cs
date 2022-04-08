using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    public GameObject Red;
    public GameObject Yellow;
    public GameObject Green;
    private bool _isRed = true;
    private bool _isYellow = false;
    private bool _isGreen = false;

    private void Start()
    {
        
        Red.SetActive( false);
        Green.SetActive (false);
        Yellow.SetActive  (false);
    }

    private void Update()
    {
       StartCoroutine( TurnOnGreen());
       StartCoroutine( TurnOnYellow());
       StartCoroutine( TurnOnRed());
        if (_isGreen == true)
        {
            Green.SetActive(true);
        }
        if (_isGreen == false)
        {
            Green.SetActive ( false);
        }
        if (_isRed == true)
        {
            Red.SetActive (true);
        }
        if (_isRed == false)
        {
            Red.SetActive ( false);
        }
        if (_isYellow == true)
        {
            Yellow.SetActive ( true);
        }
        if (_isYellow == false)
        {
            Yellow.SetActive ( false);
        }

    }


    IEnumerator TurnOnRed()
    {
        if (_isRed)
        {
            yield return new WaitForSeconds(10);
            _isRed = false;
            _isYellow = true;

        }


    }
    IEnumerator TurnOnYellow()
    {
        if (_isYellow) {

            yield return new WaitForSeconds(5);
            _isYellow = false;
            _isGreen = true;
        }
       

    }
    IEnumerator TurnOnGreen()
    {
        if (_isGreen)
        {
            _isYellow = false;
            yield return new WaitForSeconds(10);
            _isGreen = false;
            _isRed = true;
        }
       
    }




}

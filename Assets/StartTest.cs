using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

public class StartTest : MonoBehaviour

{
    public GameObject AI_cars, TileManager, Player, Logger;
    private BoxCollider Collider;
    private AICarEngine[] AICarEngines;
    float currCountdownValue;
    private AudioSource Audio;


    // Start is called before the first frame update
    void Start()
    {
        if (GlobalVariables.Trial_type != GlobalVariables.TrialType.ADAPTATION)
        {
            AI_cars.SetActive(true);
            AICarEngines = AI_cars.GetComponentsInChildren<AICarEngine>();
        }
        Collider = GetComponent<BoxCollider>();
        //StartCoroutine(StartCountdown());
        Audio = GetComponent<AudioSource>();
        GameObject.Find("AlertCanvas").GetComponent<Text>().text = GlobalVariables.Label[GlobalVariables.Trial_counter];


    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnTriggerEnter(Collider other)
    {
        //Logger.GetComponent<Logger>().enabled = true;
        //Logger.GetComponent<Logger>().StartLogger();
        if (other.name == "ColliderFront")
        {
            TileManager.SetActive(true);
            if (GlobalVariables.Trial_type != GlobalVariables.TrialType.ADAPTATION)
            {
                foreach (AICarEngine AIce in AICarEngines)
                { AIce.enabled = true; }
            }
            GlobalVariables.isTestRunning = true;
            StartCoroutine(StartCountdown());

        }
    }


    public IEnumerator StartCountdown(float countdownValue = GlobalVariables.Trial_Time)
    {
        currCountdownValue = countdownValue;

        while (currCountdownValue > 0)
        {
            //Debug.Log("Countdown: " + currCountdownValue);
            GameObject.Find("AlertCanvas").GetComponent<Text>().text = "Countdown: " + currCountdownValue;
            yield return new WaitForSecondsRealtime(1.0f);
            currCountdownValue--;
        }

        if (GlobalVariables.Trial_counter++ < 2)
        {
            //GlobalVariables.Trial_type = GlobalVariables.Trial_sequence[GlobalVariables.Trial_counter];

            /*Player.GetComponent<SteeringWheel>().enabled = false;
            Player.GetComponent<CarController>().enabled = false;
            Player.GetComponent<CarAudio>().enabled = false;
            Player.GetComponent<PlayerSensors>().enabled = false;
            */
            GlobalVariables.Trial_type = GlobalVariables.Trial_sequence[GlobalVariables.Trial_counter];
        }
        GlobalVariables.isTestRunning = false;
        Logger.GetComponent<Logger>().StopLogger();
        SceneManager.LoadScene(1);

    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button10))
        {
            Audio.Play();
            Player.GetComponent<SteeringWheel>().enabled = true;
            Player.GetComponent<CarController>().enabled = true;
            Player.GetComponent<CarAudio>().enabled = true;
            Player.GetComponent<PlayerSensors>().enabled = true;
        }
    }


}

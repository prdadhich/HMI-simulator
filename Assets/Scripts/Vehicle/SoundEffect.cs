using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class SoundEffect : MonoBehaviour {

    public AudioClip rearendcoll;
    public AudioClip bumpyroad;
    public AudioClip StopInst;
    


    private bool m_StartedSound; // flag for knowing if we have started sounds
    private CarController m_CarController; // Reference to car we are controlling
    private AudioSource playRearEnd;
    private AudioSource playBumpyRoad;
    private AudioSource stopInstruction;

    // Use this for initialization
    void Start () {
        // get the carcontroller ( this will not be null as we have require component)
        m_CarController = GetComponent<CarController>();


        playRearEnd = SetUpEngineAudioSource(rearendcoll);
        playBumpyRoad = SetUpEngineAudioSource(bumpyroad);
        stopInstruction = SetUpEngineAudioSource(StopInst);
        //StartSound();

    }

    private void StartSound()
    {
        // setup the  audio source
        playBumpyRoad = gameObject.AddComponent<AudioSource>();
        playRearEnd = gameObject.AddComponent<AudioSource>();

        // flag that we have started the sounds playing
        m_StartedSound = true;
    }

    private void StopSound()
    {
        //Destroy all audio sources on this object:
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }

        m_StartedSound = false;
    }

    public void PlayRearEndCollisionSound()
    {

        //playRearEnd.PlayOneShot(rearendcoll);
        if(!playRearEnd.isPlaying)
            playRearEnd.Play();

    }

    public void PlayStopInstruction(float d)
    {
        stopInstruction.PlayDelayed(d);
        StartCoroutine(quitfromApp(d));
        
        //Application.Quit();
    }

    public void PlayBumpyRoadSound()
    {
        /*layBumpyRoad.PlayOneShot(bumpyroad);*/
       // if(!playBumpyRoad.isPlaying)
            playBumpyRoad.Play();
        
    }

    // sets up and adds new audio source to the gane object
    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        // create the new audio source component on the game object and set up its properties
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 1;
        source.loop = false;
        source.playOnAwake = false;

        // start the clip from a random point
        //source.time = Random.Range(0f, clip.length);
     
        source.minDistance = 0f;
        source.maxDistance = 100f;
        source.dopplerLevel = 0;
        return source;
    }

    IEnumerator quitfromApp(float interval)
    {

        yield return new WaitForSeconds(interval + 5.0f );
       
        Application.Quit();

    }

}

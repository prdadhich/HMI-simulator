using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedLimit : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] SpeedLimitBoard30;
    public GameObject[] SpeedLimitBoard50;
    public GameObject[] SpeedLimitBoard80;
    private Sprite SpeedWarning30;
    private Sprite SpeedWarning50;
    private Sprite SpeedWarning80;
    public Image SpeedWarningImage;
    private bool _isSpeed30 = false;
    private bool _isSpeed50 = false;
    private bool _isSpeed80 = false;

    // Start is called before the first frame update
    void Start()
    {
        SpeedWarning30 = Resources.Load<Sprite>("Images/speedlimit30")as Sprite;
        SpeedWarning50 = Resources.Load<Sprite>("Images/speedlimit50")as Sprite;
        SpeedWarning80 = Resources.Load<Sprite>("Images/speedlimit80")as Sprite;
        SpeedWarningImage.GetComponent<Sprite>();
        SpeedWarningImage.enabled = false; 
         
    }

    // Update is called once per frame
    void Update()
    {
        SpeedLimitBoard30 = GameObject.FindGameObjectsWithTag("SpeedLimit30");
        SpeedLimitBoard50 = GameObject.FindGameObjectsWithTag("SpeedLimit50");
        SpeedLimitBoard80 = GameObject.FindGameObjectsWithTag("SpeedLimit80");
        Vector3 PlayerPosition = Player.transform.position;
        if (_isSpeed30 || _isSpeed50 || _isSpeed80)
        {
            SpeedWarningImage.enabled = true;
        }
        else
            SpeedWarningImage.enabled = false;
        foreach (GameObject slb in SpeedLimitBoard30)
        {
            float distance30 = Vector3.Distance(slb.transform.position, PlayerPosition);
            float angle30 = Vector3.Angle(Player.transform.forward,slb.transform.forward);



            if (distance30 < 30f && angle30 > 150 && angle30 < 210 & PlayerControl.speedkmph > 30)
            {

                SpeedLimit30();
            }
            else 
            {
                _isSpeed30 = false;
                
            }
            

        }
        foreach (GameObject slb in SpeedLimitBoard50)
        {
            float distance30 = Vector3.Distance(slb.transform.position, PlayerPosition);
            float angle30 = Vector3.Angle(Player.transform.forward, slb.transform.forward);


            
            if (distance30 < 30f && angle30 > 150 && angle30 < 210 && PlayerControl.speedkmph > 50)
            {
               
                SpeedLimit50();
            }
            else
                _isSpeed50 = false;


        }
        foreach (GameObject slb in SpeedLimitBoard80)
        {
            float distance30 = Vector3.Distance(slb.transform.position, PlayerPosition);
            float angle30 = Vector3.Angle(Player.transform.forward, slb.transform.forward);


           
            if (distance30 < 30f && angle30 > 150 && angle30 < 210 && PlayerControl.speedkmph > 80)
            {
              
                SpeedLimit80();
            }
            else 
                _isSpeed80 = false;


        }

    }


    void SpeedLimit30()
    {
        SpeedWarningImage.sprite = SpeedWarning30;
        _isSpeed30 = true;
        

    }
    void SpeedLimit50()
    {
        SpeedWarningImage.sprite = SpeedWarning50;
        
        _isSpeed80 = true;
    }
    void SpeedLimit80()
    {
        SpeedWarningImage.sprite = SpeedWarning80;
        
        _isSpeed80 = true;

    }
}



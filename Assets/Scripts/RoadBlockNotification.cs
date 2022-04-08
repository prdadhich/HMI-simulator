using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadBlockNotification : MonoBehaviour
{
    public GameObject[] RoadBlock;
    public GameObject Car;
    public Image RoadBlockWarning;
    private Sprite _roadBlockWarning;
    // Start is called before the first frame update
    void Start()
    {
        RoadBlock = GameObject.FindGameObjectsWithTag("RoadBlockSign");
        _roadBlockWarning= Resources.Load<Sprite>("Images/RoadBlock") as Sprite;
        RoadBlockWarning.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < RoadBlock.Length; i++) 
        {
            float distance = Vector3.Distance(Car.transform.position, RoadBlock[i].transform.position);

            if (distance < 30f)
            {
                RoadBlockWarning.sprite = _roadBlockWarning;
                RoadBlockWarning.enabled = true;

            }
            else {

                RoadBlockWarning.enabled = false;
            
            }
        }
        
        
    }
}

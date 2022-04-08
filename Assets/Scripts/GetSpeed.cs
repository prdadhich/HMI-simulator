using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GetSpeed : MonoBehaviour
{
    
    private float _carspeed;
    [SerializeField]
    private TMP_Text  _speedText;
    public TMP_Text TMText;
    Vector3 lastposition;
    public int speed;
    public static int SendSpeed;
    // Start is called before the first frame update

    private void Start()
    {
        InvokeRepeating("SetSpeed", 1f, 0.5f);
        TMText.GetComponent<TextMesh>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 currentposition = transform.position;
        if( currentposition != lastposition){
            _carspeed = Vector3.Magnitude(lastposition - currentposition) / Time.deltaTime;
            speed = (int)((int)_carspeed * 3.6); 

        }
        lastposition = currentposition;
        SendSpeed = speed;

    }
    void SetSpeed()
    {
        _speedText.text = speed.ToString();
        TMText.text = speed.ToString();
        
    }
}

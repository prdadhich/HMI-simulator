using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanRandomMove : MonoBehaviour
{
    private bool _isgrounded = true;
    [SerializeField]
    //private LayerMask _humanLayer;
    // Start is called before the first frame update

    private Animator _anim;
    void Start()
    {
        _anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((_anim.enabled == false))
        {
            float random = Random.value;
            this.transform.rotation = Quaternion.Euler(0, random*10, 0);
            _anim.enabled = true; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("HumanWalk"))
        {
            _anim.enabled = true;
            Debug.Log("onlayer");
        }
        else 
        {
            Debug.Log("Noton layer");

            _anim.enabled = false;

        }
       

    }
    

}

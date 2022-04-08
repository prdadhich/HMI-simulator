using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationLineTwo : MonoBehaviour
{
    private LineRenderer _navigationLine;
    public Transform _car;
    public Transform _followpath;
    Vector3 _startposition;


    // Start is called before the first frame update
    void Start()
    {
        _navigationLine = GetComponent<LineRenderer>();


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _startposition = _car.position;

        }
        _navigationLine.SetPosition(0, FollowPath.goal.position);
        _navigationLine.SetPosition(1, FollowPath.nextgoal.position);
       // _navigationLine.SetPosition(2, FollowPath.nextgoal.position);
        //_navigationLine.SetPosition(3, FollowPath.nextnextgoal.position);

    }
}

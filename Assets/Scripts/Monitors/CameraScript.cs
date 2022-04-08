using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float VfieldOfview;
    public float Hfieldofview;
    public float aspectRatio;
    public float frustrumHeight;
    public float frustumWidth;
    public float distance;

    public float editHFOV;
    public float editVFOV;
    public float HorizObl;
    public float VertObl;
    public Matrix4x4 projectMat;

    protected Camera c;

    // Use this for initialization
    void Start()
    {
        c = GetComponent<Camera>();
        //editValue = c.aspect;
        projectMat = c.projectionMatrix;

        editHFOV = 0.0f;
        HorizObl = 0.0f;
        VertObl = 0.0f;
        if (c.gameObject.name == "LeftCAM")       //if this scrit is applyed to object named "LeftCAM" deform the camera this specific way
        {
            HorizObl = -0.55f;
            VertObl = 0.04f;
        }
        else if (c.gameObject.name == "RightCAM") //if this scrit is applyed to object named "RightCAM" deform the camera this specific way
        {
            HorizObl = 0.1f;
        }
        else if (c.gameObject.name == "CenterCAM") //if this scrit is applyed to object named "CenterCAM" deform the camera this specific way
        {
            //do nothing
        }

    }

    // Update is called once per frame
    void Update()
    {
        VfieldOfview = c.fieldOfView;    // show the vertical FOV of camera

        //c.aspect = editValue;          // edit the aspect ratio of camera

        aspectRatio = c.aspect;          // store the current a.r of camera

        frustrumHeight = 2.0f * c.farClipPlane * Mathf.Tan(0.5f * c.fieldOfView * Mathf.Deg2Rad);        // calculate the height of far clipping plane


        frustumWidth = aspectRatio * frustrumHeight;        // calculate the width of far clipping plane

        // calculate the horizontal FOV of camera
        Hfieldofview = 2.0f * Mathf.Atan(0.5f * aspectRatio * frustrumHeight / c.farClipPlane) * Mathf.Rad2Deg;

        projectMat[0, 2] = HorizObl;
        projectMat[1, 2] = VertObl;
        c.projectionMatrix = projectMat;

        float w = 2.0f * c.farClipPlane * Mathf.Tan(Mathf.Deg2Rad * editHFOV * 0.5f);

        editVFOV = 2.0f * Mathf.Atan(0.5f * (w / aspectRatio) / c.farClipPlane) * Mathf.Rad2Deg;
    }

    protected void doNothing()
    {

    }


}

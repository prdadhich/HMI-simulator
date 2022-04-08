using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetTime : MonoBehaviour
{

    public TMP_Text largeText;

    void Start()
    {
        largeText.GetComponent<TextMesh>();
        string time = System.DateTime.UtcNow.ToLocalTime().ToString("HH:mm");
        string timeUS = System.DateTime.UtcNow.ToLocalTime().ToString("M/d/yy   hh:mm tt");
        largeText.text = time;
    }
}
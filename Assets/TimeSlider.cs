using System;   
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
    private Slider timeSlider;
    void Start()
    {
        timeSlider = GetComponent<Slider>();



    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeSlider.value;
    }
}

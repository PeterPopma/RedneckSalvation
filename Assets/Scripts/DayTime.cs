using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayTime : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDayTime;
    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] Volume volumeSkyFog;
    [SerializeField] GameObject groundSmoke;
    [SerializeField] Transform lightRoot;

    const int MINUTES_PER_DAY = 1440;
    float minuteOfDay;
    bool isNight;
    bool lightsOn;
    float elevation;

    public bool IsNight { get => isNight; set => isNight = value; }
    public float Elevation { get => elevation; set => elevation = value; }

    private void Start()
    {
        minuteOfDay = MINUTES_PER_DAY / 2;
        SetLights(false);
    }

    void FixedUpdate()
    {
        minuteOfDay += 0.2f;
        if(minuteOfDay >= MINUTES_PER_DAY)
        {
            minuteOfDay = 0;
        }
        // we add one hour because otherwise it already starts to get light at 2:00
        int hours = (int)((minuteOfDay + 60) / 60);
        if (hours == 24)
        {
            hours = 0;
        }
        textDayTime.text = hours.ToString("00") + ":" + (minuteOfDay % 60).ToString("00");
        sun.transform.rotation = Quaternion.Euler(0, minuteOfDay / (float)MINUTES_PER_DAY * 360, 0);
        elevation = ((720 - Mathf.Abs((MINUTES_PER_DAY / 2) - minuteOfDay)) * 0.083f) - 5; // 720..0..720  -> 0..720..0  -> 0..60..0  -> -5..55..-5
        sun.transform.Rotate(new Vector3(elevation, 0, 0));
        if (elevation < 0 && !isNight)
        {
            SetNight(true);
        }
        if (elevation > 0 && isNight)
        {
            SetNight(false);
        }
        if (elevation < 0 && !lightsOn && minuteOfDay > MINUTES_PER_DAY / 2)
        {
            SetLights(true);
        }
        if (elevation > 20 && lightsOn && minuteOfDay < MINUTES_PER_DAY / 2)
        {
            SetLights(false);
        }
        moon.enabled = elevation < 10;
        groundSmoke.SetActive(elevation < 35 && minuteOfDay<MINUTES_PER_DAY/2);
        sun.colorTemperature = (elevation + 10) * 128 + 500; // 1000..8000
    }
    private void SetLights(bool lightsOn)
    {
        this.lightsOn = lightsOn;
        foreach (Transform light in lightRoot)
        {
            light.Find("Light").gameObject.SetActive(lightsOn);
        }
    }

    private void SetNight(bool isNight)
    {
        this.isNight = isNight;
        VisualEnvironment env;
        if (volumeSkyFog.profile.TryGet<VisualEnvironment>(out env))
        {
            if (isNight)
            {
                env.skyType.value = Convert.ToInt32(SkyType.HDRI);
                sun.intensity = 0;
            }
            else
            {
                env.skyType.value = Convert.ToInt32(SkyType.PhysicallyBased);
                sun.intensity = 450000;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class DayCycle : MonoBehaviour {
    public float CycleInSeconds = 60;
    public float SunlightLerpSpeed = 1;
    [Range(1, 3)] public int AxisOfRotation = 1;

    public int CyclePercent { get; private set; }
    public bool IsDay { get; private set; }

    public event System.EventHandler OnNewDay;
    public event System.EventHandler OnNewNight;

    private Light sunlight;
    private float intensity;

    private float step;
    private Vector3 rotation;
    private float axis {
        get => rotation[AxisOfRotation - 1];
        set => rotation[AxisOfRotation - 1] = value;
    }

    void Start() {
        sunlight = GetComponent<Light>();
        intensity = sunlight.intensity;
        rotation = transform.localEulerAngles;
        step = (360 / CycleInSeconds) * Time.deltaTime;
    }

    void Update() {
        axis += step;
        WrapRotation();
        ApplyRotation();
        CyclePercent = Mathf.FloorToInt((axis / 360) * 100);

        if (axis >= 180 && IsDay) SetNight();
        else if (axis < 180 && !IsDay) SetDay();
    }

    public void SetTime(float percent) {
        axis = (360 / CycleInSeconds) * percent;
        // SetNight will update this to the proper value
        IsDay = !(axis < 180);
    }

    void WrapRotation() {
        if (axis >= 360) axis -= 360;
        else if (axis < 0) axis += 360;
    }

    // Update the axis of rotation for the light
    void ApplyRotation() {
        transform.localEulerAngles = rotation;
    }

    void SetNight() {
        IsDay = false;
        OnNewNight?.Invoke(null, System.EventArgs.Empty);
        StartCoroutine(LerpIntensity(0));
    }

    void SetDay() {
        IsDay = true;
        OnNewDay?.Invoke(null, System.EventArgs.Empty);
        StartCoroutine(LerpIntensity(intensity));
    }

    IEnumerator LerpIntensity(float target) {
        float lerp = 0;
        while (lerp < 1) {
            lerp += Time.deltaTime * SunlightLerpSpeed;
            sunlight.intensity = Mathf.Lerp(sunlight.intensity, target, lerp);
            yield return null;
        }
        sunlight.intensity = target;
    }
}
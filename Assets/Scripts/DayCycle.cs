using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour {
    public float CycleInSeconds = 60;
    public float SunlightLerpSpeed = 1;
    public float DayExposure, NightExposure;
    [Range(1, 3)] public int AxisOfRotation = 1;

    public int CyclePercent { get; private set; }
    public bool IsDay { get; private set; }

    public Light Sunlight;
    public Light Moonlight;
    public Material SkyboxMaterial;

    public event System.EventHandler OnNewDay;
    public event System.EventHandler OnNewNight;

    private float sunIntens;
    private float moonIntens;
    private float exposure;

    private float step;
    private Vector3 rotation;
    private float axis {
        get => rotation[AxisOfRotation - 1];
        set => rotation[AxisOfRotation - 1] = value;
    }

    void Start() {
        sunIntens = Sunlight.intensity;
        moonIntens = Moonlight.intensity;
        rotation = Sunlight.transform.localEulerAngles;
        step = (360 / CycleInSeconds) * Time.deltaTime;
        exposure = SkyboxMaterial.GetFloat("_Exposure");
        axis = 0;
    }

    void Update() {
        axis += step;
        WrapRotation();
        ApplyRotation();
        CyclePercent = Mathf.FloorToInt((axis / 360) * 100);

        if (axis >= 180 && IsDay) StartCoroutine(SetNight());
        else if (axis < 180 && !IsDay) StartCoroutine(SetDay());
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
        Sunlight.transform.localEulerAngles = rotation;
        Vector3 moonOffset = Vector3.zero;
        moonOffset[AxisOfRotation - 1] = 180;
        Moonlight.transform.localEulerAngles = rotation + moonOffset;
    }

    IEnumerator SetNight() {
        IsDay = false;
        OnNewNight?.Invoke(null, System.EventArgs.Empty);
        StartCoroutine(LerpExposure(DayExposure, NightExposure));
        var sun = StartCoroutine(LerpIntensity(0, moon : false));
        var moon = StartCoroutine(LerpIntensity(moonIntens, moon : true));
        yield return sun;
        yield return moon;
        RenderSettings.sun = Moonlight;
    }

    IEnumerator SetDay() {
        IsDay = true;
        OnNewDay?.Invoke(null, System.EventArgs.Empty);
        StartCoroutine(LerpExposure(NightExposure, DayExposure));
        var sun = StartCoroutine(LerpIntensity(sunIntens, moon : false));
        var moon = StartCoroutine(LerpIntensity(0, moon : true));
        yield return sun;
        yield return moon;
        RenderSettings.sun = Sunlight;
    }

    IEnumerator LerpIntensity(float target, bool moon = false) {
        float lerp = 0;
        while (lerp <= 1) {
            lerp += Time.deltaTime * SunlightLerpSpeed;
            if (moon) Moonlight.intensity = Mathf.Lerp(Moonlight.intensity, target, lerp);
            else Sunlight.intensity = Mathf.Lerp(Sunlight.intensity, target, lerp);
            yield return null;
        }
        if (moon) Moonlight.intensity = target;
        else Sunlight.intensity = target;
    }

    IEnumerator LerpExposure(float start, float target) {
        float lerp = 0;
        while (lerp < 1) {
            lerp += Time.deltaTime / 2;
            exposure = Mathf.Lerp(start, target, lerp);
            SkyboxMaterial.SetFloat("_Exposure", exposure);
            yield return null;
        }
        SkyboxMaterial.SetFloat("_Exposure", exposure);
    }
}
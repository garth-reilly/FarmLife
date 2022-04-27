// GR: Add this script to any Direction Light to add a Day Night cycle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    // GR: Configurable variables
    [SerializeField] float fullDayNightCycleSeconds = 256f;
    [SerializeField] bool staticShadows = false;
    [SerializeField] float dawn = 6;
    [SerializeField] float dusk = 18f;
    [SerializeField] float lightDarkTransitionSpeed = 0.5f;

    // GR: State variables
    float halfDayNightCycleSeconds;
    float secondsElapsed = 0f;
    private float timeOfDay = 0f;
    public float TimeOfDay{get {return timeOfDay;}}
    Color currentColor;

    // GR: Referenced variables
    Light directionalLight;
    Color originalLightColor;

    void Start()
    {
        secondsElapsed = fullDayNightCycleSeconds / 2f;
        if (!staticShadows)
        {
            this.gameObject.AddComponent<Rigidbody>();
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.angularDrag = 0f;
            rigidbody.angularVelocity = new Vector3(6.28f / fullDayNightCycleSeconds, 0f, 0f); // GR: 6.28f would rotate fully in 1 second. 2 x pi = 6.28f.
            this.transform.eulerAngles = new Vector3(90f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            halfDayNightCycleSeconds = fullDayNightCycleSeconds / 2f;
            timeOfDay = fullDayNightCycleSeconds * 0.04166f;
            directionalLight = GetComponent<Light>();
            originalLightColor = directionalLight.color;
            currentColor = originalLightColor;
        }
    }

    void Update()
    {
        secondsElapsed += Time.deltaTime;
        timeOfDay = secondsElapsed / fullDayNightCycleSeconds * 24f;
        if (secondsElapsed > fullDayNightCycleSeconds)
        {
            secondsElapsed = 0;
        }
        if (staticShadows)
        {
            if ((timeOfDay >= dusk) || (timeOfDay < dawn))
            {
                currentColor = new Color(Mathf.Lerp(currentColor.r, 0f, lightDarkTransitionSpeed * Time.deltaTime), Mathf.Lerp(currentColor.g, 0f, lightDarkTransitionSpeed * Time.deltaTime), Mathf.Lerp(currentColor.b, 0f, lightDarkTransitionSpeed * Time.deltaTime));
                directionalLight.color = currentColor;
            }
            if ((timeOfDay >= dawn) && (timeOfDay < dusk))
            {
                currentColor = new Color(Mathf.Lerp(currentColor.r, originalLightColor.r, lightDarkTransitionSpeed * Time.deltaTime), Mathf.Lerp(currentColor.g, originalLightColor.g, lightDarkTransitionSpeed * Time.deltaTime), Mathf.Lerp(currentColor.b, originalLightColor.b, lightDarkTransitionSpeed * Time.deltaTime));
                directionalLight.color = currentColor;
            }
        }
    }
}

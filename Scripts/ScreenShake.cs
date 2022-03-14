using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private float shakeDuration = 0f;
    private float shakeIntensity = 0.7f;
    private float dampSpeed = 1f;
    private Vector3 origPos;

    private static ScreenShake instance;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        if (shakeDuration > 0)
        {
            Vector2 randomOffset = Random.insideUnitCircle;
            transform.localPosition = origPos + new Vector3(randomOffset.x, randomOffset.y) * shakeIntensity;

            shakeDuration -= dampSpeed * Time.fixedDeltaTime;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = origPos;
        }
    }

    public static void SetOriginalPosition()
    {
        instance.origPos = instance.transform.localPosition;
    }

    public static void TriggerShake(float duration, float intensity)
    {
        instance.shakeDuration = duration;
        instance.shakeIntensity = intensity;
    }
}

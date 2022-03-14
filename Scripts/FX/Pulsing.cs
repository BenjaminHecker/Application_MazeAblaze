using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsing : MonoBehaviour
{
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private float pulseSpeed;

    private float startTime;
    private float alphaFactor = 1;
    private SpriteRenderer sRender;

    void Awake()
    {
        startTime = Time.fixedTime;
        sRender = GetComponent<SpriteRenderer>();

        sRender.color = color1;
    }

    void FixedUpdate()
    {
        float ratio = Mathf.Sqrt((Mathf.Sin(pulseSpeed * (Time.fixedTime - startTime)) + 1) / 2);
        Color res = Color.Lerp(color1, color2, ratio);
        res.a *= alphaFactor;
        sRender.color = res;
    }

    public void SetAlpha(float alpha)
    {
        alphaFactor = alpha;
    }
}

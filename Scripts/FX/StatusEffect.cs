using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusEffect : MonoBehaviour
{
    private SpriteRenderer effectRender;
    private ParticleSystem effectParticles;
    private IEnumerator effectRoutine;

    private void Awake()
    {
        effectRender = transform.GetComponent<SpriteRenderer>();
        effectParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    public void TriggerEffect(TextMeshProUGUI counterTxt)
    {
        if (effectRoutine != null)
            StopCoroutine(effectRoutine);

        effectRoutine = EffectRoutine(counterTxt);
        StartCoroutine(effectRoutine);

        effectParticles.Play();
    }

    private IEnumerator EffectRoutine(TextMeshProUGUI counterTxt)
    {
        CanvasGroup txtGroup = counterTxt.transform.GetComponent<CanvasGroup>();
        
        for (int i = 0; i < 20; i++)
        {
            effectRender.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, i / 20f));
            txtGroup.alpha = Mathf.Lerp(1, 0, i / 20f);

            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 20; i++)
        {
            effectRender.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, i / 20f));
            txtGroup.alpha = Mathf.Lerp(0, 1, i / 20f);

            yield return new WaitForSeconds(0.01f);
        }
    }
}

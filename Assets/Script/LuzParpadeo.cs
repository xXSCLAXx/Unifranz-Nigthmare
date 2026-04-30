using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LuzParpadeo : MonoBehaviour
{
    public Image panelOscuro;
    public float minTiempo = 0.05f;
    public float maxTiempo = 0.2f;
    public float minAlpha = 180f;
    public float maxAlpha = 255f;

    void OnEnable()
    {
        StartCoroutine(Parpadear());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Parpadear()
    {
        while (true)
        {
            // oscurece
            float alpha = Random.Range(minAlpha, maxAlpha);
            Color c = panelOscuro.color;
            c.a = alpha / 255f;
            panelOscuro.color = c;

            yield return new WaitForSeconds(Random.Range(minTiempo, maxTiempo));

            // aclara
            c.a = 0f;
            panelOscuro.color = c;

            yield return new WaitForSeconds(Random.Range(minTiempo, maxTiempo));
        }
    }
}
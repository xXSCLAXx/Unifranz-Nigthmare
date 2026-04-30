using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PCTimer : MonoBehaviour
{
    [Header("Refs")]
    public GameObject jumpScareImage;
    public Text timerText;

    [Header("Config")]
    public float tiempoLimite = 20f;

    private float tiempoRestante;
    private bool corriendo = false;

    void OnEnable()
    {
        tiempoRestante = tiempoLimite;
        corriendo = true;
        StartCoroutine(Contar());
    }

    void OnDisable()
    {
        corriendo = false;
    }

    IEnumerator Contar()
    {
        while (tiempoRestante > 0 && corriendo)
        {
            tiempoRestante -= Time.deltaTime;

            if (timerText != null)
            {
                if (tiempoRestante <= 10f)
                    timerText.color = Color.red;
                else
                    timerText.color = Color.white;

                timerText.text = "⚠ " + Mathf.CeilToInt(tiempoRestante) + "s";
            }

            yield return null;
        }

        if (corriendo)
            MostrarJumpScare();
    }

    void MostrarJumpScare()
    {
        corriendo = false;
        if (jumpScareImage != null)
            jumpScareImage.SetActive(true);
        gameObject.SetActive(false);
    }

    public void DetenerTimer()
    {
        corriendo = false;
    }
}
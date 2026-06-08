using UnityEngine;
using UnityEngine.UI;

public class PCTimer : MonoBehaviour
{
    [Header("Refs")]
    public GameObject jumpScareImage;
    public Text timerText;

    private static int consecutiveScares = 0;
    public static float bonusTime = 0f;

    private float tiempoRestante;
    private bool corriendo = false;

    void Awake()
    {
        tiempoRestante = GetTimeLimit();
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        corriendo = false;
    }

    float GetTimeLimit()
    {
        float penalty = 0f;
        for (int i = 0; i < consecutiveScares; i++)
            penalty += 1.0f + i * 0.7f;
        return Mathf.Max(1f, 10f - penalty + bonusTime);
    }

    void Update()
    {
        if (!corriendo) return;
        if (tiempoRestante <= 0f) return;

        tiempoRestante -= Time.deltaTime;

        if (timerText != null)
        {
            if (tiempoRestante <= 5f)
                timerText.color = Color.red;
            else
                timerText.color = Color.white;

            timerText.text = "⚠ " + Mathf.CeilToInt(tiempoRestante) + "s";
        }

        if (tiempoRestante <= 0f)
        {
            corriendo = false;
            MostrarJumpScare();
        }
    }

    public void IniciarTimer()
    {
        if (corriendo) return;
        if (tiempoRestante <= 0f) return;

        corriendo = true;
        if (timerText != null)
            timerText.gameObject.SetActive(true);
    }

    public void PausarTimer()
    {
        corriendo = false;
    }

    public void ResetTimer()
    {
        corriendo = false;
        tiempoRestante = GetTimeLimit();
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    void MostrarJumpScare()
    {
        GameOverController.LoseLife();
        corriendo = false;
        consecutiveScares++;
        if (jumpScareImage != null)
            jumpScareImage.SetActive(true);
        gameObject.SetActive(false);
    }

    public void DetenerTimer()
    {
        corriendo = false;
    }

    public static bool IsExpired()
    {
        return false;
    }
}

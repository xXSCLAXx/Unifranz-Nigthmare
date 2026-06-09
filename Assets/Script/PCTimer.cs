using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PCTimer : MonoBehaviour
{
    [Header("Refs")]
    public GameObject jumpScareImage;
    public Text timerText;

    private static int consecutiveScares = 0;
    public static float bonusTime = 0f;

    private float tiempoRestante;
    private bool corriendo = false;
    private bool ultimateTriggered = false;

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
            else if (tiempoRestante <= 40f)
                timerText.color = new Color(1f, 0.5f, 0f);
            else
                timerText.color = Color.white;

            timerText.text = "" + Mathf.CeilToInt(tiempoRestante) + "s";
        }

        if (tiempoRestante <= 0f && !ultimateTriggered)
        {
            ultimateTriggered = true;
            corriendo = false;
            StartCoroutine(UltimateScreamerSequence());
        }
    }

    IEnumerator UltimateScreamerSequence()
    {
        if (timerText != null)
            timerText.text = "";

        for (int i = 0; i < 3; i++)
        {
            GameOverController.LoseLife();
            if (GameOverController.isGameOver) yield break;
            yield return new WaitForSeconds(0.3f);
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
        ultimateTriggered = false;
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    public static void AddBonusTime(float seconds)
    {
        bonusTime += seconds;
        PCTimer instance = FindObjectOfType<PCTimer>();
        if (instance != null)
            instance.ResetTimer();
    }

    public void AddPenalty(float seconds)
    {
        tiempoRestante += seconds;
        if (tiempoRestante < 1f) tiempoRestante = 1f;
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
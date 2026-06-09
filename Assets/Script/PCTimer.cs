using UnityEngine;
using System.Collections;

public class PCTimer : MonoBehaviour
{
    [Header("Refs")]
    public GameObject jumpScareImage;

    private static int consecutiveScares = 0;
    public static float bonusTime = 0f;

    private float tiempoRestante;
    private bool corriendo = false;
    private bool ultimateTriggered = false;
    private bool pendingDeactivate = false;

    void Awake()
    {
        tiempoRestante = GetTimeLimit();
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
        if (pendingDeactivate)
        {
            pendingDeactivate = false;
            gameObject.SetActive(false);
            return;
        }

        if (!corriendo) return;
        if (tiempoRestante <= 0f) return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f && !ultimateTriggered)
        {
            ultimateTriggered = true;
            corriendo = false;
            StartCoroutine(UltimateScreamerSequence());
        }
    }

    IEnumerator UltimateScreamerSequence()
    {
        PCWindowController pcw = GetComponent<PCWindowController>();
        if (pcw != null)
            pcw.CancelTask();

        if (jumpScareImage != null)
        {
            jumpScareImage.transform.SetAsLastSibling();
            jumpScareImage.SetActive(true);
        }

        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.PlayScream();

        yield return new WaitForSeconds(0.5f);

        GameOverController.LoseLife();

        yield return new WaitForSeconds(0.3f);
        if (jumpScareImage != null) jumpScareImage.SetActive(false);
        GameObject bloodFilter = GameObject.Find("BloodFilter");
        if (bloodFilter != null) bloodFilter.SetActive(false);
        pendingDeactivate = true;
    }

    public void IniciarTimer()
    {
        if (corriendo) return;
        if (tiempoRestante <= 0f) return;

        corriendo = true;
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
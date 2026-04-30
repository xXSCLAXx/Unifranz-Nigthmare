using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class JumpScare : MonoBehaviour
{
    public float duracion = 3f;
    public GameObject bloodFilter;
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void OnEnable()
    {
        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.PlayScream();

        if (bloodFilter != null)
            bloodFilter.SetActive(true);

        if (videoPlayer != null)
            videoPlayer.Play();

        StartCoroutine(Esperar());
    }

    IEnumerator Esperar()
    {
        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null) am.PlayPostScare();

        yield return new WaitForSeconds(duracion);

        if (videoPlayer != null)
            videoPlayer.Stop();

        if (bloodFilter != null)
            bloodFilter.SetActive(false);

        gameObject.SetActive(false);
    }
}
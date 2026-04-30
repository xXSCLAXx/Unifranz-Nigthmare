using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip musica;
    public AudioClip clickPC;
    public AudioClip scream;
    public AudioClip earRinging;
    public AudioClip breathing;

    [Header("Sources")]
    public AudioSource sourceMusica;
    public AudioSource sourceFX;

    void Start()
    {
        // Música de ambiente en loop
        sourceMusica.clip = musica;
        sourceMusica.loop = true;
        sourceMusica.Play();
    }
    public void PlayPostScare()
    {
    sourceFX.PlayOneShot(earRinging);
    sourceFX.PlayOneShot(breathing);
    }
    public void PlayClickPC()
    {
      sourceFX.clip = clickPC;
      sourceFX.Play();
    }

    public void PlayScream()
    {
        sourceFX.PlayOneShot(scream);
    }
}
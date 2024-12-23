using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    public AudioClip[] musicTracks;
    private AudioSource audioSource;
    public float fadeDuration = 2.0f;
    public float targetVolume = 1.0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        PlayRandomTrack();
    }

    void PlayRandomTrack()
    {
        if (musicTracks.Length == 0)
        {
            Debug.LogWarning("Track list are empty");
            return;
        }

        int randomIndex = Random.Range(0, musicTracks.Length);
        AudioClip selectedTrack = musicTracks[randomIndex];
        StartCoroutine(FadeOutAndIn(selectedTrack));
    }

    IEnumerator FadeOutAndIn(AudioClip newTrack)
    {
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut());
        }

        audioSource.clip = newTrack;
        yield return StartCoroutine(FadeIn());

        Invoke(nameof(PlayRandomTrack), audioSource.clip.length - fadeDuration);
    }

    IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0f;
    }

    IEnumerator FadeIn()
    {
        audioSource.Play();
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance { get; private set; }

    private AudioSource audioSource;
    private Queue<AudioClip> sfxQueue = new Queue<AudioClip>(); 
    private float lastSFXTime = 0f;
    [SerializeField] private float minIntervalBetweenSFX = 0.1f;
    [SerializeField] private int maxQueueSize = 5;
    [SerializeField] private float targetVolume = 1;


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
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
    }

    void Update()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (sfxQueue.Count > 0 && CanPlaySFX())
        {
            AudioClip clipToPlay = sfxQueue.Dequeue();
            PlaySFXDirectly(clipToPlay);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("missing audio clip");
            return;
        }

        if (CanPlaySFX())
        {
            PlaySFXDirectly(clip, targetVolume);
        }
        else
        {
            if (sfxQueue.Count < maxQueueSize)
            {
                sfxQueue.Enqueue(clip);
            }
        }
    }

    private void PlaySFXDirectly(AudioClip clip, float volume = 1.0f)
    {
        audioSource.PlayOneShot(clip, volume);
        lastSFXTime = Time.time;
    }

    private bool CanPlaySFX()
    {
        return Time.time - lastSFXTime >= minIntervalBetweenSFX;
    }
}
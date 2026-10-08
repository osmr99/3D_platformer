using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private MusicTrack musicTrack;
    public AudioSource musicSource;
    [SerializeField] private float musicVolume;

    private void Awake()
    {
        if(Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void PlayMusic(string trackName, float fadeDuration)
    {
        StartCoroutine(AnimateMusicCrossFade(musicTrack.GetClipFromName(trackName), fadeDuration));
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(musicVolume, 0f, percent);
            yield return null;
        }
        musicSource.clip = nextTrack;
        musicSource.Play();
        percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, musicVolume, percent);
            yield return null;
        }
    }

    public void StopMusic()
    {
        StartCoroutine(StopMusicCrossFade(0.5f));
    }

    IEnumerator StopMusicCrossFade(float fadeDuration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(musicVolume, 0, percent);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = musicVolume;
    }
}

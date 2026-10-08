using UnityEngine;

[System.Serializable]
public struct MusicClip
{
    public string clipName;
    public AudioClip clip;
}

public class MusicTrack : MonoBehaviour
{
    public MusicClip[] clips;

    public AudioClip GetClipFromName (string musicName)
    {
        foreach (var track in clips)
        {
            if (track.clipName == musicName)
                return track.clip;
        }

        return null;
    }
}

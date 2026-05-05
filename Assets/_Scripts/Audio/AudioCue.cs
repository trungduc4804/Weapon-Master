using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class AudioCue
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private Vector2 pitchRange = Vector2.one;
    [SerializeField] [Range(0f, 1f)] private float spatialBlend = 0f;
    [SerializeField] private bool loop;
    [SerializeField] private AudioMixerGroup outputGroup;

    public float Volume => volume;
    public float SpatialBlend => spatialBlend;
    public bool Loop => loop;
    public AudioMixerGroup OutputGroup => outputGroup;
    public bool HasClip => clips != null && clips.Length > 0;

    public AudioClip GetRandomClip()
    {
        if (!HasClip)
        {
            return null;
        }

        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

    public float GetRandomPitch()
    {
        float minPitch = Mathf.Min(pitchRange.x, pitchRange.y);
        float maxPitch = Mathf.Max(pitchRange.x, pitchRange.y);
        return UnityEngine.Random.Range(minPitch, maxPitch);
    }
}

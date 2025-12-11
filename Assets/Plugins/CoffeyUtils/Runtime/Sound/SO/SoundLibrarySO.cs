using UnityEngine;
using CoffeyUtils.Sound;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Sound System/Sound Library")]
public class SoundLibrarySO : ScriptableObject
{
    [SerializeField] private List<SFXEventClip> _sfxEventClips = new List<SFXEventClip>();
    [SerializeField] private List<MusicTrackClip> _musicTrackClips = new List<MusicTrackClip>();

    
    public SfxBase GetSFXClip(SFXEventsEnum sfxEvent)
    {
        foreach (var sfxEventClip in _sfxEventClips)
        {
            if (sfxEventClip.Event == sfxEvent)
            {
                return sfxEventClip.Clip;
            }
        }
        Debug.LogWarning($"[SoundLibrarySO] SFX Event '{sfxEvent}' not found in Sound Library '{name}'.");
        return null;
    }

    public MusicTrack GetMusicTrack(MusicTracksEnum musicTrack)
    {
        foreach (var musicTrackClip in _musicTrackClips)
        {
            if (musicTrackClip.Track == musicTrack)
            {
                return musicTrackClip.Clip;
            }
        }
        Debug.LogWarning($"[SoundLibrarySO] Music Track '{musicTrack}' not found in Sound Library '{name}'.");
        return null;
    }

    [Button, Tooltip("EDITOR ONLY: Removes duplicate SFXEvents and MusicTracks, keeping the first occurrence for each.")]
    public void RemoveDuplicates()
    {
        HashSet<SFXEventsEnum> seenEvents = new HashSet<SFXEventsEnum>();
        for (int i = _sfxEventClips.Count - 1; i >= 0; i--)
        {
            if (seenEvents.Contains(_sfxEventClips[i].Event))
            {
                _sfxEventClips.RemoveAt(i);
            }
            else
            {
                seenEvents.Add(_sfxEventClips[i].Event);
            }
        }

        HashSet<MusicTracksEnum> seenTracks = new HashSet<MusicTracksEnum>();
        for (int i = _musicTrackClips.Count - 1; i >= 0; i--)
        {
            if (seenTracks.Contains(_musicTrackClips[i].Track))
            {
                _musicTrackClips.RemoveAt(i);
            }
            else
            {
                seenTracks.Add(_musicTrackClips[i].Track);
            }
        }
    }
}

[System.Serializable]
public struct SFXEventClip
{
    public SFXEventsEnum Event;
    public SfxBase Clip;
}

[System.Serializable]
public struct MusicTrackClip
{
    public MusicTracksEnum Track;
    public MusicTrack Clip;
}
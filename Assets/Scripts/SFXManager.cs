using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    #region Enums

    public enum GameplaySFXType
    {
        None,
        LayoutOpen,
        CardOpen,
        CardClose,
        CorrectMatch,
        IncorrectMatch,
        GameWin
    }
    public enum UiSFXType
    {
        None,
        Click,
        ScoreIncrease,
    }
    #endregion

    [System.Serializable]
    public class GenericSfxClips<T> where T : Enum
    {
        public T key;
        public AudioClip clip;
        public List<AudioClip> altClips = new List<AudioClip>();
        [Range(0f, 1f)] public float volume = 1f;
        public bool isLoop = false;
    }

    [Header("SFXs")]
    private Dictionary<Type, object> sfxClipsDictionary = new Dictionary<Type, object>();
    public List<GenericSfxClips<GameplaySFXType>> gameplaySfxClips = new List<GenericSfxClips<GameplaySFXType>>();
    public List<GenericSfxClips<UiSFXType>> uiSfxClips = new List<GenericSfxClips<UiSFXType>>();

    public static SFXManager Instance;
    public GameObject sfxSourcePrefab;
    protected List<AudioSource> sfxSources = new List<AudioSource>();

    protected bool sfxMuted = false;
    public bool IsSFXMuted => sfxMuted;

    protected Dictionary<AudioSource, float> volumesForAudioSources = new Dictionary<AudioSource, float>();

    protected List<AudioSource> addedAudioSources = new List<AudioSource>();
    protected Dictionary<string, AudioSource> sfxPlayed = new Dictionary<string, AudioSource>();

    protected float sfxMasterVolume = 1;

    protected virtual void Awake()
    {
        Instance = this;
        SetSfxClipsDictionary();
    }

    private void SetSfxClipsDictionary()
    {
        sfxClipsDictionary[typeof(GameplaySFXType)] = gameplaySfxClips;
        sfxClipsDictionary[typeof(UiSFXType)] = uiSfxClips;
    }

    #region SFX related Methods

    public virtual void PlaySFX<T>(T type) where T : Enum
    {
        AudioSource audioSource = PlayCommonSFX(type);
    }

    public virtual void PlaySFXOnce<T>(T type) where T : Enum
    {
        AudioSource audioSource = PlayCommonSFXOnce(type);
    }

    AudioSource PlayCommonSFX<T>(T type) where T : Enum
    {
        if (sfxClipsDictionary.TryGetValue(typeof(T), out var list))
        {
            var sfxClips = list as List<GenericSfxClips<T>>;

            if (sfxClips == null)
            {
                Debug.Log("null audio clip or list not found");
                return null;
            }

            foreach (var x in sfxClips)
            {
                if (Enum.Equals(x.key, type))
                {
                    if (x.clip != null)
                    {
                        List<AudioClip> randClips = new List<AudioClip>();
                        randClips.Add(x.clip);
                        foreach (var z in x.altClips)
                        {
                            if (z != null)
                            {
                                randClips.Add(z);
                            }
                        }

                        AudioSource newAS = PlayAudioClip(randClips[UnityEngine.Random.Range(0, randClips.Count)], x.volume, x.isLoop);

                        if (newAS != null)
                        {
                            UpdateSFXPlayed(type, newAS);
                        }

                        return newAS;
                    }
                }

            }
        }

        Debug.Log("null audio clip " + type.ToString());
        return null;
    }

    public AudioSource PlayCommonSFXOnce<T>(T type) where T : Enum
    {
        if (sfxClipsDictionary.TryGetValue(typeof(T), out var list))
        {
            var sfxClips = list as List<GenericSfxClips<T>>;

            if (sfxClips == null)
            {
                Debug.Log("null audio clip or list not found");
                return null;
            }

            foreach (var x in sfxClips)
            {
                if (Enum.Equals(x.key, type))
                {
                    if (x.clip != null)
                    {
                        foreach (var r in sfxPlayed)
                        {
                            if (r.Key == type.ToString())
                            {
                                if (r.Value != null && r.Value.isPlaying)
                                {
                                    return null;
                                }
                            }
                        }

                        List<AudioClip> randClips = new List<AudioClip> { x.clip };
                        randClips.AddRange(x.altClips.FindAll(z => z != null));

                        AudioSource newAS = PlayAudioClip(randClips[UnityEngine.Random.Range(0, randClips.Count)], x.volume, x.isLoop);

                        UpdateSFXPlayed(type, newAS);

                        return newAS;
                    }
                }
            }
        }

        Debug.Log("null audio clip or list not found");
        return null;
    }

    public virtual AudioSource PlayAudioClip(AudioClip clip, float volume = 1f, bool isLoop = false)
    {
        if (clip == null)
        {
            return null;
        }

        foreach (var x in sfxSources)
        {
            if (x.isPlaying == false)
            {
                RemoveAudioSourceFromListAsNotPlaying(x);

                x.Stop();
                x.clip = clip;
                x.loop = isLoop;
                float finalVolume = volume * sfxMasterVolume;
                x.volume = finalVolume;
                x.mute = sfxMuted;

                x.Play();

                if (volumesForAudioSources.ContainsKey(x))
                {
                    volumesForAudioSources[x] = volume;
                }
                else
                {
                    volumesForAudioSources.Add(x, volume);
                }

                return x;
            }
        }

        GameObject newSFX = Instantiate(sfxSourcePrefab, this.transform);
        newSFX.SetActive(true);

        AudioSource a = newSFX.GetComponent<AudioSource>();
        a.volume = volume;
        sfxSources.Insert(0, a);

        return PlayAudioClip(clip, volume, isLoop);
    }

    public AudioSource PlayAudioClipLoop(AudioClip clip, float volume = 1f)
    {
        return PlayAudioClip(clip, volume, true);
    }

    public virtual void StopSFX<T>(T type) where T : Enum
    {
        StopSFX(type.ToString());
    }

    public virtual void StopSFX(string type)
    {
        foreach (var r in sfxPlayed)
        {
            if (r.Key == type.ToString())
            {
                if (r.Value != null)
                {
                    if (r.Value.isPlaying)
                    {
                        r.Value.Stop();
                        if (volumesForAudioSources.ContainsKey(r.Value))
                        {
                            volumesForAudioSources.Remove(r.Value);
                        }
                    }
                }
            }
        }
    }

    public void StopAllSFX()
    {
        foreach (var x in sfxSources)
        {
            if (x != null)
            {
                x.Stop();

                if (volumesForAudioSources.ContainsKey(x))
                {
                    volumesForAudioSources.Remove(x);
                }
            }
        }
    }

    private void UpdateSFXPlayed<T>(T type, AudioSource newAS) where T : Enum
    {
        if (sfxPlayed.ContainsKey(type.ToString()))
        {
            sfxPlayed[type.ToString()] = newAS;
        }
        else
        {
            sfxPlayed.Add(type.ToString(), newAS);
        }
    }

    #endregion

    public void SetSFXMuteTo(bool target)
    {
        sfxMuted = target;
        foreach (var x in sfxSources)
        {
            if (x != null)
            {
                x.mute = sfxMuted;
            }
        }
    }

    public void SFXMasterVolumeChanged(float val)
    {
        val = Mathf.Clamp(val, 0f, 1f);
        sfxMasterVolume = val;

        foreach (var x in volumesForAudioSources)
        {
            x.Key.volume = x.Value * sfxMasterVolume;
        }
    }

    protected virtual void RemoveAudioSourceFromListAsNotPlaying(AudioSource x)
    {
        List<string> listToRemoveFromOncePlayed = new List<string>();
        foreach (var r in sfxPlayed)
        {
            if (r.Value != null)
            {
                if (r.Value == x)
                {
                    listToRemoveFromOncePlayed.Add(r.Key);
                }
            }
        }

        foreach (var r in listToRemoveFromOncePlayed)
        {
            if (sfxPlayed.ContainsKey(r))
            {
                sfxPlayed.Remove(r);
            }
        }
    }

    public AudioSource GetAudioSource<T>(T type) where T : Enum
    {
        if (sfxPlayed.ContainsKey(type.ToString()))
        {
            if (sfxPlayed.TryGetValue(type.ToString(), out AudioSource newAs))
            {
                return newAs;
            }
        }

        return null;
    }
}
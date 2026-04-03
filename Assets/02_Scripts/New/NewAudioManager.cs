using System;
using UnityEngine;
using UnityEngine.Audio;

public enum Sfx { Dead, Hit, LevelUp, Lose, Melee, Range, Select, Win, GetItem, OpenBox }

[Serializable]
public class SfxEntry
{  
    public Sfx Sfx;
    public AudioClip[] clips;
}
public class NewAudioManager : MonoBehaviour
{

    [Header("AudioMixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("BGM")]
    [SerializeField] private AudioClip bgmClip;

    [Header("SFX")]
    //[SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private SfxEntry[] sfxEntries;
    [SerializeField] private int channels = 8;

    private AudioSource bgmPlayer;
    private AudioHighPassFilter bgmEffect;
    private AudioSource[] sfxPlayers;
    private int channelIndex;

    private void Awake()
    {
        Init();
        LoadVolumes();
    }

    private void Init()
    {
        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = this.transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.clip = bgmClip;
        bgmPlayer.outputAudioMixerGroup = bgmGroup;
        bgmEffect = Camera.main != null ? Camera.main.GetComponent<AudioHighPassFilter>() : null;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < channels; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassEffects = true;
            sfxPlayers[i].outputAudioMixerGroup = sfxGroup;
        } 
    }

    private void LoadVolumes()
    {
        SetBGMVolume(1);
        SetSFXVolume(1);
        //SetBGMVolume(PlayerPrefs.GetFloat("BGM", 1f));
        //SetSFXVolume(PlayerPrefs.GetFloat("SFX", 1f));
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay) bgmPlayer.Play();
        else bgmPlayer.Stop();
    }
    public void EffectBgm(bool isPlay)
    {
        if(bgmEffect != null) bgmEffect.enabled = isPlay;
    }
    public void PlaySfx(Sfx sfx)
    {
        AudioClip clip = GetSfxClip(sfx);
        if (clip == null) return;

        for (int i = 0; i < sfxPlayers.Length; i++)
        {      
           
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying) continue;

            //int ranIndex = (sfx == Sfx.Hit || sfx == Sfx.Melee) ? UnityEngine.Random.Range(0, 2) : 0;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = clip; 
            sfxPlayers[loopIndex].Play();
                      
            break;
        }
    }

    private AudioClip GetSfxClip(Sfx sfx)
    {
        foreach(SfxEntry entry in sfxEntries)
        {
            if(entry.Sfx == sfx && entry.clips != null && entry.clips.Length > 0)
            {
                return entry.clips[UnityEngine.Random.Range(0, entry.clips.Length)];
            }           
        }
        return null;
    }
    public void SetBGMVolume(float value) //0~1 사이 값 받음, -80~20으로 적용
    {
        float db = value <= 0 ? -80 : Mathf.Log10(value) * 20f;
        
        if (mixer != null) mixer.SetFloat("BGM", db);
       
       
    }
    public void SetSFXVolume(float value)
    {
        float db = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
        if (mixer != null) mixer.SetFloat("SFX", db);
       
    }
   
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Net.NetworkInformation;
public class AudioManager : MonoBehaviour
{
    [Header("#BGM")]
    [SerializeField]private AudioClip bgmClip;
    [SerializeField] AudioSource bgmPlayer;
    [SerializeField] AudioHighPassFilter bgmEffect;

    [Header("#SFX")]
    [SerializeField] private AudioClip[] sfxClip;
    [SerializeField] private int channels;
    [SerializeField] AudioSource[] sfxPlayers;
    [SerializeField] int channelIndex;
   
    private AudioMixer audioMixer;
    private float masterVolume;
    private float bgmVolume;
    private float sfxVolume;

    private void Awake()
    {
        SetInit();
    }
    private void SetInit()
    {
        audioMixer.GetFloat("Master", out masterVolume);
        audioMixer.GetFloat("SFX", out sfxVolume);
        audioMixer.GetFloat("BGM", out bgmVolume);
    }
    //오디오 믹서 볼륨값 -80 ~ 0
    public void SetMasterVolume(float v)
    {
        audioMixer.GetFloat("Master", out v);
    }

    public void SetBgmVolume(float v)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(v) * 20);
    }

    public void SetSfxVolume(float v)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(v) * 20);
    }
   /* [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("#SFX")]
    public AudioClip[] sfxClip;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx {  Dead, Hit, LevelUp=3, Lose, Melee, Range=7, Select, Win , GetItem, OpenBox }
    void Awake()
    {
        instance = this;
        Init();
       
    }

    void Init()
    {    
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();
            
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;
            sfxPlayers[index].volume = sfxVolume;
        }
    }    
    public void PlayBgm(bool isPlay)
    {
       if(isPlay)
       {
            bgmPlayer.Play();
       }
       else
       {
            bgmPlayer.Stop();
       }
    }

    public void EffectBgm(bool isPlay)
    {
         if (bgmEffect != null) bgmEffect.enabled = isPlay;
    }
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            if(sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClip[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
       
    }*/
}

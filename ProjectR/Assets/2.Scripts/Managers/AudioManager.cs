using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("#Volume")] 
    [Range(0,1)]
    public float masterVolume;
    [Range(0,1)]
    public float bgmVolume;
    [Range(0,1)]
    public float sfxVolume;
    
    
    
    [Header("#BGM")]
    public AudioClip[] bgmClip;
    private AudioSource _bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    
    public int channels;
    private AudioSource[] _sfxPlayers;
    private int _channelIndex;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayBgm(false,EBgm.Maple);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayBgm(true,EBgm.Maple);
        }
    }

    public enum EBgm
    {
        Maple,
        Cat
    }
    
    public enum ESfx
    {
        ChargingArrow,
        EnemyDead,
        EnemyHit,
        ShootArrow
    }

    void Awake()
    {
        Singleton();
        Init();
        
        PlayBgm(true,EBgm.Maple);
    }

    void Init()
    {
        
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.playOnAwake = false;
        _bgmPlayer.loop = true;
        _bgmPlayer.volume = bgmVolume * masterVolume;
        // bgmPlayer.clip = bgmClip;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        _sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < _sfxPlayers.Length; index++)
        {
            _sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            _sfxPlayers[index].playOnAwake = false;
            _sfxPlayers[index].volume = sfxVolume * masterVolume;
        }
    }
    
    private void Singleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBgm(bool isPlay,EBgm eBgm)
    {
        if (isPlay)
        {
            _bgmPlayer.clip = bgmClip[(int)eBgm];
            StartCoroutine(FadeIn(_bgmPlayer, 1f));
        }
        else
        {
           StartCoroutine(FadeOut(_bgmPlayer,1f));
        }
    }
    
    private IEnumerator FadeIn(AudioSource audioSource,float duration)
    {
       audioSource.volume = 0;
        audioSource.Play();

        float startVolume = 0f;
        float targetVolume = bgmVolume * masterVolume;
        
        for(float t =0;t<duration;t+=Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume,targetVolume,t/duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
    
    private IEnumerator FadeOut(AudioSource audioSource,float duration)
    {
        float startVolume = audioSource.volume;
        
        for(float t=0; t<duration;t+=Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume,0,t/duration);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
    

    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        _bgmPlayer.volume = bgmVolume * masterVolume;
    }

    public float GetBgmVolume()
    {
        return bgmVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        foreach (AudioSource sfxPlayer in _sfxPlayers)
        {
            sfxPlayer.volume = sfxVolume * masterVolume;
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateAllVolumes();
    }

    private void UpdateAllVolumes()
    {
        _bgmPlayer.volume = bgmVolume * masterVolume;
        foreach (AudioSource sfxPlayer in _sfxPlayers)
        {
            sfxPlayer.volume = sfxVolume * masterVolume;
        }
    }

    public void PlayBgm(EBgm eBgm)
    {
        int bgmIndex = (int)eBgm;
        if (bgmIndex >= 0 && bgmIndex < bgmClip.Length) 
        {
            _bgmPlayer.clip = bgmClip[bgmIndex];
            _bgmPlayer.Play();
        }
        else
        {
            Debug.LogWarning("헉");
        }
    }

    public void PlaySfx(ESfx eSfx)
    {
        for (int index = 0; index < _sfxPlayers.Length; index++)
        {
            int loopIndex = (index + _channelIndex) % _sfxPlayers.Length;

            if (_sfxPlayers[loopIndex].isPlaying)
                continue;

            _channelIndex = loopIndex;
            _sfxPlayers[loopIndex].clip = sfxClips[(int)eSfx];
            _sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
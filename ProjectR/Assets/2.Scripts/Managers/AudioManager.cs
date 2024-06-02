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
    


    public enum EBgm
    {
        Main,
        Shop,
        Stage1,
        Stage1Boss,
        Tavern,
    }
    
    public enum ESfx
    {
        ChargingArrow,
        EnemyDead,
        EnemyHit,
        ShootArrow,
        Heal,
        Slash1,
        Slash2,
        TakeDamage,
        ButtonClick,
        ButtonHover,
        ElectricFruit,
        ShopPurchase,
        PlayerDead,
        PlayerFootstepStage1,
        PlayerLevelUp,
        PlayerRoll,
    }

    void Awake()
    {
        Singleton();
        Init();
        
        PlayBgm(EBgm.Main);
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


    private IEnumerator FadeOutFadeInBGM(int bgmIndex, float duration = 1)
    {
      float startVolume = _bgmPlayer.volume;
      for(float t = 0; t < duration; t+=Time.deltaTime)
      {
          _bgmPlayer.volume = Mathf.Lerp(startVolume,0,t/duration);
          yield return null;
      }
      _bgmPlayer.volume = 0;
      _bgmPlayer.Stop();
      
      _bgmPlayer.clip = bgmClip[bgmIndex];
      _bgmPlayer.Play();
      
      for(float t = 0; t<duration; t+=Time.deltaTime)
      {
          _bgmPlayer.volume = Mathf.Lerp(0,bgmVolume * masterVolume,t/duration);
          yield return null;
      }
      _bgmPlayer.volume = bgmVolume * masterVolume;
      
    }
    
    // private IEnumerator FadeOut(AudioSource audioSource,float duration)
    // {
    //     float startVolume = audioSource.volume;
    //     
    //     for(float t=0; t<duration;t+=Time.deltaTime)
    //     {
    //         audioSource.volume = Mathf.Lerp(startVolume,0,t/duration);
    //         yield return null;
    //     }
    //     audioSource.Stop();
    //     audioSource.volume = startVolume;
    // }
    

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
            // _bgmPlayer.clip = bgmClip[bgmIndex];
            // _bgmPlayer.Play();
            StartCoroutine(FadeOutFadeInBGM(bgmIndex));
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
    
    
    public void PlayButtonHover()
    {
        PlaySfx(ESfx.ButtonHover);
    }
    
    public void PlayButtonClick()
    {
        PlaySfx(ESfx.ButtonClick);
    }
    
}
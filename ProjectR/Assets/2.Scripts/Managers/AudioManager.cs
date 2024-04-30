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
        MAIN
    }
    
    public enum ESfx
    {
        
        
    }

    void Awake()
    {
        Singleton();
        Init();
        
        Instance.PlayBgm(EBgm.MAIN);
        //PlayBgm(true);
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

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            if (!_bgmPlayer.isPlaying)
            {
                _bgmPlayer.Play();
            }
        }
        else
        {
            _bgmPlayer.Stop();
        }
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
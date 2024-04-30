using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Master")] 
    
    [Range(0,1)]
    public float masterVolume;
    
    [Header("#BGM")]
    public AudioClip[] bgmClip;
    [Range(0,1)]
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    
    [Range(0,1)]
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;


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
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume * masterVolume;
        // bgmPlayer.clip = bgmClip;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume * masterVolume;
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
            if (!bgmPlayer.isPlaying)
            {
                bgmPlayer.Play();
            }
        }
        else
        {
            bgmPlayer.Stop();
        }
    }
    

    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        bgmPlayer.volume = bgmVolume * masterVolume;
    }

    public float GetBgmVolume()
    {
        return bgmVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        foreach (AudioSource sfxPlayer in sfxPlayers)
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
        bgmPlayer.volume = bgmVolume * masterVolume;
        foreach (AudioSource sfxPlayer in sfxPlayers)
        {
            sfxPlayer.volume = sfxVolume * masterVolume;
        }
    }

    public void PlayBgm(EBgm eBgm)
    {
        int bgmIndex = (int)eBgm;
        if (bgmIndex >= 0 && bgmIndex < bgmClip.Length) 
        {
            bgmPlayer.clip = bgmClip[bgmIndex];
            bgmPlayer.Play();
        }
        else
        {
            Debug.LogWarning("헉");
        }
    }

    public void PlaySfx(ESfx eSfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)eSfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public CanvasGroup Fade_img;
    public GameObject Loading;
    float fadeDuration = 1.5f; //암전되는 시간
    float prologueFadeDuration = 4f;

    public static MySceneManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static MySceneManager instance;

    void Start()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 1)
        {
            AudioManager.Instance.PlayBgm(AudioManager.EBgm.Tavern);
            Debug.Log("Prologue");
            Fade_img.DOFade(0, prologueFadeDuration)
            .OnStart(() =>
            {
                Loading.SetActive(false);
            }).SetEase(Ease.InQuint)
            .OnComplete(() => {
                Fade_img.blocksRaycasts = false;
            });

        }
        else
        {
            AudioManager.Instance.PlayBgm(AudioManager.EBgm.Main);
            Debug.Log("Normal");
            Fade_img.DOFade(0, fadeDuration)
            .OnStart(() => {
                Loading.SetActive(false);
            })
            .OnComplete(() => {
                Fade_img.blocksRaycasts = false;
            });
        }

    }

    public void ChangeScene(string sceneName)
    {
        Fade_img.DOFade(1, fadeDuration)
        .OnStart(() =>
        {
            Fade_img.blocksRaycasts = true; //아래 레이캐스트 막기
            PlayerController.CanControl = false;
        })
        .OnComplete(() =>
        {
            StartCoroutine("LoadScene", sceneName);
        });
    }

    IEnumerator LoadScene(string sceneName)
    {
        Loading.SetActive(true); //로딩 화면을 띄움

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; //퍼센트 딜레이용

        float past_time = 0;
        float percentage = 0;

        while (!(async.isDone))
        {
            yield return null;

            past_time += Time.deltaTime;

            if (percentage >= 90)
            {
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if (percentage == 100)
                {
                    async.allowSceneActivation = true; //씬 전환 준비 완료
                }
            }
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if (percentage >= 90) past_time = 0;
            }
        }

        PlayerController.CanControl = true;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalScript : MonoBehaviour
{
    public string LinkedScene;
    public float fadeDuration = 1f;
    public CanvasGroup canvasGroup;

    private bool canLoadScene = false;
    private bool isLoadingScene = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canLoadScene = true;
        }
    }

    private void Update()
    {
        if (canLoadScene && !isLoadingScene)
        {
            isLoadingScene = true;
            StartCoroutine(LoadSceneWithFade());
        }
    }

    private IEnumerator LoadSceneWithFade()
    {
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(LinkedScene);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeOutAfterSceneLoaded());
    }

    private IEnumerator FadeOutAfterSceneLoaded()
    {
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration); // 페이드 아웃
            yield return null;
        }

        isLoadingScene = false;
    }
}

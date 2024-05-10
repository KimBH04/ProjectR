using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    public IEnumerator FadeOut()
    {
        if (!SceneManager.GetActiveScene().name.Equals("MainScene"))
        {
            fadeImage.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, 1f - (timer / fadeDuration));
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeImage.color = Color.black;

        SceneManager.LoadScene(1);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, 1f - (timer / fadeDuration));
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }

    public void OpenSettings()
    {

    }

    public void SetVolume(float volume)
    {

    }
}

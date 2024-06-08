using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class MainManager : MonoBehaviour
{

    public void StartGame()
    {
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.ButtonClick);
        MySceneManager.Instance.ChangeScene("TavernPrologue");
    }



    public void EndGame()
    {
        AudioManager.Instance.PlaySfx(AudioManager.ESfx.ButtonClick);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

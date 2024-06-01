using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
        Application.Quit();
    }
}

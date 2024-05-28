using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainManager : MonoBehaviour
{

    public void StartGame()
    {
        MySceneManager.Instance.ChangeScene("TavernPrologue");
    }



    public void EndGame()
    {
        Application.Quit();
    }
}

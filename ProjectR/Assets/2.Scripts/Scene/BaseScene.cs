using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BaseScene : MonoBehaviour
{
    public string sceneName;
   
    private void OnTriggerEnter(Collider other)
    {
        MySceneManager.Instance.ChangeScene(sceneName);
    }

}

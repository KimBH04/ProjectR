using UnityEngine;

public class Cheater : MonoBehaviour    // 오직 에디터에서 테스트 용으로 쓸 것
{
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("CreateRoom");
        }
    }
#else
    void Awake()
    {
        Destroy(gameObject);
    }
#endif
}

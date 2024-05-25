using UnityEngine;

public class Cheater : MonoBehaviour    // 오직 에디터에서 테스트 용으로 쓸 것
{
#if UNITY_EDITOR
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Exp++;
        }

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

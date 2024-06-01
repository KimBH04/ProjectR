using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static bool IsStop => Time.timeScale == 0f;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            if (instance == null)
            {
                instance = Instantiate(Resources.Load<GameObject>("ManagerObjects/GameManager")).GetComponent<GameManager>();
            }

            Debug.Log("GameManager");
            return instance;
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}

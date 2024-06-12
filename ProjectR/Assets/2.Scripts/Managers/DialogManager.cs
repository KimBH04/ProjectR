using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI dialogText;
    public string[] dialogs;
    public Image eButton;
    public GameObject move;
    private bool isSkip;
    private int currentDialogIndex = -1;
    private bool isPrinting = false;
    private bool inTrigger = false;
    private bool lastDialogDisplayed = false; // 마지막 대화가 출력되었는지 여부를 나타내는 변수

    private Coroutine dialogRoutine = null;

    void Start()
    {
        text.gameObject.SetActive(false);
    }

    void Update()
    {
        if (lastDialogDisplayed)
        {
            move.SetActive(true);
        }

        if (inTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (!isPrinting)
            {
                eButton.gameObject.SetActive(false);

                if (!lastDialogDisplayed)
                {
                    isSkip = false;         // 다음 출력에 다시 초기화
                    StartCoroutine(PrintDialog(dialogs));
                }
            }
            else
            {
                isSkip = true;              // 출력 도중 누르면 즉시 전부 출력
            }
        }

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);

            if (dialogRoutine != null) StopCoroutine(dialogRoutine);
            dialogRoutine = StartCoroutine(PrintDialog(dialogs)); // 첫 번째 대화 목록부터 시작
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            text.gameObject.SetActive(false);
            inTrigger = false;
        }
    }

    IEnumerator PrintDialog(string[] dialogList)
    {
        PlayerController.CanControl = false;
        inTrigger = true;
        isPrinting = true;
        dialogText.text = "";
        currentDialogIndex++;

        // 마지막 대화까지 출력되었는지 확인
        if (currentDialogIndex >= dialogList.Length - 1)
        {
            PlayerController.CanControl = true;
            currentDialogIndex = dialogList.Length - 1;
            if (!lastDialogDisplayed)   //마지막 번호일 때 출력 취소
            {
                lastDialogDisplayed = true;
                yield break;
            }
        }   // 마지막 대화 후 다음 대화시엔 다시 출력

        eButton.gameObject.SetActive(false);

        // 한 글자씩 출력
        foreach (char letter in dialogList[currentDialogIndex].ToCharArray())
        {
            dialogText.text += letter;
            if (!isSkip)
            {
                yield return new WaitForSeconds(0.07f); // 출력 속도 조절
            }
        }
        eButton.gameObject.SetActive(false);

        if(currentDialogIndex != dialogList.Length -1)
        {
            eButton.gameObject.SetActive(true);
        }

        isPrinting = false;
    }
}

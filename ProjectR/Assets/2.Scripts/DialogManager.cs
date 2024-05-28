using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI dialogText;
    public string[] firstDialogs;
    private int currentDialogIndex = -1;
    private bool isPrinting = false;
    private bool inTrigger = false;
    private bool lastDialogDisplayed = false; // 마지막 대화가 출력되었는지 여부를 나타내는 변수

    void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (inTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (!isPrinting)
            {
                if (lastDialogDisplayed)
                {
                    // 마지막 대화가 출력되면 출력 끝
                    return;
                }
                else
                {
                    StartCoroutine(PrintDialog(firstDialogs));
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            canvas.gameObject.SetActive(true);
            
            StartCoroutine(PrintDialog(firstDialogs)); // 첫 번째 대화 목록부터 시작
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            canvas.gameObject.SetActive(false);
            inTrigger = false;
        }
    }

    IEnumerator PrintDialog(string[] dialogList)
    {
        inTrigger = true;
        isPrinting = true;
        dialogText.text = "";
        currentDialogIndex++;

        // 마지막 대화까지 출력되었는지 확인
        if (currentDialogIndex >= dialogList.Length - 1)
        {
            currentDialogIndex = dialogList.Length - 1;
            if (!lastDialogDisplayed)   //마지막 번호일 때 출력 취소
            {
                lastDialogDisplayed = true;
                yield break;
            }
        }   // 마지막 대화 후 다음 대화시엔 다시 출력

        // 한 글자씩 출력
        foreach (char letter in dialogList[currentDialogIndex].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.1f); // 출력 속도 조절
        }

        isPrinting = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarController : MonoBehaviour
{
    public GameObject DecorateMark;

    public Button decorateBtn;
    public Button exitBtn;

    private Animator animator;

    void Start()
    {
        animator = transform.GetComponent<Animator>();

        decorateBtn.onClick.AddListener(() =>
        {
            EnterDecorateMode();
        });

        exitBtn.onClick.AddListener(() =>
        {
            EnterViewMode();
        });

    }

    public void EnterDecorateMode()
    {
        DecorateMark.SetActive(true);
        animator.Play("showLeftBar");
        GameManager.instance.CurrentGameMode = GameManager.GameModeCode.DecorateFurniture;
    }

    public void EnterViewMode()
    {
        DecorateMark.SetActive(false);
        animator.Play("hideLeftBar");
        GameManager.instance.CurrentGameMode = GameManager.GameModeCode.View;
    }

}

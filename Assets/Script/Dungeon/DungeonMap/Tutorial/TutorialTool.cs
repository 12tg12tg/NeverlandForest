using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialTool : MonoBehaviour
{
    [Header("UI")]
    public RectTransform handIcon;
    public RectTransform dialogBox;
    public GameObject blackoutPanel;
    public TMP_Text dialogBoxText;
    public GameObject storyBoard;
    public RectTransform blackout;

    [Header("��������Ʈ")]
    public Sprite rect;
    public Sprite circle;

    public Button TutorialTargetButtonActivate(Button target)
    {
        var targetObject = target.gameObject;
        var clone = Instantiate(targetObject, blackoutPanel.transform, true);

        var btn = clone.GetComponent<Button>();
        btn.onClick.AddListener(() => Destroy(btn.gameObject));

        return btn;
    }

    public void BlackPanelOn()
    {
        var panel = blackoutPanel.transform.GetChild(1).gameObject;
        panel.SetActive(true);
    }

    public void BlackPanelOff()
    {
        var panel = blackoutPanel.transform.GetChild(1).gameObject;
        panel.SetActive(false);
    }
}

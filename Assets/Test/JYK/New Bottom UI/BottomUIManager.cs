using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BottomUIManager : MonoBehaviour
{
    public enum ButtonState { Item, Skill }

    private static BottomUIManager instance;
    public static BottomUIManager Instance => instance;

    //Instance
    public BottomInfoUI info;
    public List<BottomSkillButtonUI> skillButtons;
    public List<GameObject> itemButtons;
    public List<Button> tags;
    public GameObject progress;
    [SerializeField] private List<Image> progressImg;

    //Vars
    [HideInInspector] public ButtonState buttonState;
    [HideInInspector] public BottomSkillButtonUI curSkillButton;
    [HideInInspector] public ObstacleType curObstacleType;

    private bool isBoySkillDone;
    private bool isGirlSkillDone;

    private void Awake()
    {
        instance = this;
        SkillButtonInit();
    }

    // ���α׷���
    public void UpdateProgress()
    {
        int prog = BattleManager.Instance.Progress;
        if (prog == 0)
        {
            progressImg[0].enabled = false;
            progressImg[1].enabled = false;
        }
        else if(prog == 1)
        {
            progressImg[0].enabled = true;
            progressImg[1].enabled = false;
        }
        else if(prog == 2)
        {
            progressImg[0].enabled = true;
            progressImg[1].enabled = true;
        }
    }

    // ��ų ��ư
    public void InteractiveSkillButton(PlayerType type, bool interactive)
    {
        if(type == PlayerType.Boy)
        {
            isBoySkillDone = !interactive;
        }
        else
        {
            isGirlSkillDone = !interactive;
        }
        UpdateSkillInteractive();
    }

    public void UpdateSkillInteractive()
    {
        skillButtons.ForEach(n =>
        {
            if (n.skill?.SkillTableElem.player == PlayerType.Boy)
            {
                if (isBoySkillDone)
                    n.MakeUnclickable();
                else
                    n.MakeClickable();
            }
            else if (n.skill?.SkillTableElem.player == PlayerType.Girl)
            {
                if (isGirlSkillDone)
                    n.MakeUnclickable();
                else
                    n.MakeClickable();
            }
        });
    }

    public void IntoSkillState(BottomSkillButtonUI skillButton)
    {
        //Time.timeScale = 0.2f;
        this.curSkillButton = skillButton;
        BattleManager.Instance.ReadyTileClick();
        BattleManager.Instance.DisplayMonsterTile(curSkillButton.skill.SkillTableElem.range);
        skillButtons.ForEach(n => { if (n != curSkillButton) n.MakeUnclickable(); });
        tags.ForEach(n => n.interactable = false);
    }

    public void ExitSkillState()
    {
        //Time.timeScale = 1f;
        curSkillButton = null;
        BattleManager.Instance.EndTileClick();
        BattleManager.Instance.UndisplayMonsterTile();
        UpdateSkillInteractive();
        tags.ForEach(n => n.interactable = true);
    }

    public void SkillButtonInit() // ��ų������ 12�� ���� : �±� ��ư + �ڵ� Ȱ��ȭ�� ���� �Լ�
    {
        info.Init();

        buttonState = ButtonState.Skill;
        itemButtons.ForEach((n) => n.SetActive(false));
        skillButtons.ForEach((n) => n.gameObject.SetActive(true));

        var list = Vars.BoySkillList;
        int count = list.Count;

        int buttonIndex = 1;
        for (int i = 0; i < count; i++)
        {
            skillButtons[buttonIndex++].Init(list[i]);
        }

        buttonIndex = 7;
        list = Vars.GirlSkillList;
        for (int i = 0; i < count; i++)
        {
            skillButtons[buttonIndex++].Init(list[i]);
        }
    }

    // ������ ��ư
    public void ItemButtonInit()
    {
        info.Init();

        buttonState = ButtonState.Item;
        itemButtons.ForEach((n) => n.SetActive(true));
        skillButtons.ForEach((n) => n.gameObject.SetActive(false));



    }
}

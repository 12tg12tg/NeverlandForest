using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDrag : MonoBehaviour
{
    [Header("�Ŵ��� ����")]
    [SerializeField] private BattleManager bm;
    [SerializeField] private TileMaker tm;
    [SerializeField] private BottomUIManager bottomManager;

    [HideInInspector] public Vector3 lastDragWorldPos;
    [HideInInspector] public Tiles lastDrapTile;
    [HideInInspector] public BottomSkillButtonUI curDragSkill;

    [Header("�巡�� ������ ����")]
    [SerializeField] private RectTransform dragSlot;
    [SerializeField] private Image skillImg;

    [Header("ĵ���� ����")]
    [SerializeField] private Canvas uiCanvas;
    private Camera uiCamera;

    [Header("�巡�� ���� Ȯ��")]
    public bool isDrag;

    private void Start()
    {
        uiCamera = uiCanvas.worldCamera;
    }

    public void Init(BottomSkillButtonUI skillUi)
    {
        isDrag = true;
        curDragSkill = skillUi;
        dragSlot.gameObject.SetActive(true);
        skillImg.sprite = skillUi.skill.SkillTableElem.IconSprite;
    }

    public void Release()
    {
        isDrag = false;
        lastDrapTile = null;
        lastDragWorldPos = Vector3.zero;
        dragSlot.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDrag)
        {
            var screenPos = MultiTouch.Instance.TouchPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, screenPos, uiCamera, out Vector2 localPos);
            dragSlot.localPosition = localPos;
        }
    }

    public void UpdatePos(Vector2 screenPos)
    {
        var ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask("Tile")))
        {
            lastDragWorldPos = hit.point;
            lastDrapTile = hit.transform.GetComponent<Tiles>();
            if (lastDrapTile.isHighlight || lastDrapTile.isHighlightConsume) // ���ð��� Ÿ���� ���� ���� ǥ��
            {
                tm.SetAllTileMiddleState(curDragSkill.skill.SkillTableElem.range);
                lastDrapTile.HighlightSkillRange(curDragSkill.skill.SkillTableElem.range, lastDragWorldPos);
            }
        }
        else
        {
            lastDragWorldPos = Vector3.zero;
            lastDrapTile = null;
            tm.SetAllTileMiddleState(curDragSkill.skill.SkillTableElem.range);
        }
    }
}

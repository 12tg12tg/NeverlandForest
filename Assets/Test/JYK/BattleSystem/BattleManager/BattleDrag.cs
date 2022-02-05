using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDrag : MonoBehaviour
{
    [Header("매니저 연결")]
    [SerializeField] private BattleManager bm;
    [SerializeField] private TileMaker tm;
    [SerializeField] private BottomUIManager bottomManager;

    [HideInInspector] public Vector3 lastDragWorldPos;
    [HideInInspector] public Tiles lastDrapTile;
    [HideInInspector] public BottomSkillButtonUI curDragSkill;

    [Header("드래그 아이콘 연결")]
    [SerializeField] private RectTransform dragSlot;
    [SerializeField] private Image skillImg;

    [Header("캔버스 연결")]
    [SerializeField] private Canvas uiCanvas;
    private Camera uiCamera;

    [Header("드래그 여부 확인")]
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
            if (lastDrapTile.isHighlight) // 선택가능 타일일 때만 범위 표시
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

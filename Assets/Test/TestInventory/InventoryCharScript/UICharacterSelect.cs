using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour
{
    public ScrollRect scrollRect;
    public UICharacterIcon iconPrefab;

    public List<DataCharacter> characterList = new List<DataCharacter>();
    public List<UICharacterIcon> iconList = new List<UICharacterIcon>();

    public void Init(List<DataCharacter> list)
    {
        characterList = list;

        foreach (var cha in characterList)
        {
            var icon = Instantiate(iconPrefab, scrollRect.content);
            icon.Init(cha);

            var button = icon.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickCharacterIcon(cha));
            iconList.Add(icon);
        }
    }

    // View의 사용자 입력 받는 부분
    public void OnClickCharacterIcon(DataCharacter dataCha)
    {
        var parent = GetComponentInParent<UICharacter>();
        parent.SetCharacter(dataCha);
    }
}

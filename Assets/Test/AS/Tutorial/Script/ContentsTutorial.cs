using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsTutorialProceed
{
    public bool Hunt { get; set; } = false;
    public bool RandomEvent { get; set; } = false;
    public bool Note { get; set; } = false;
    public bool BlueMoon { get; set; } = false;
}

public class ContentsTutorial
{
    public ContentsTutorialProceed contentsTutorialProceed;
    public void Init()
    {
        // ����� ������ ��������
        contentsTutorialProceed = Vars.UserData.contentsTutorial;
    }
}

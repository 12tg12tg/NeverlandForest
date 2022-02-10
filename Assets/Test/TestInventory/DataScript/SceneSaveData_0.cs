using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSaveData_0 : SaveDataBase
{
    public MainTutorialStage MainTutorialStage { get; set; }
    public ContentsTutorialProceed contentsTutorialProceed { get; set; }
    public override SaveDataBase VersionUp()
    {
        return null;
    }
}

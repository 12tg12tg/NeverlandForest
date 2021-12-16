using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command 
{
    public abstract void Execute();
    public abstract void Undo();
    public abstract void Redo(); // undo를 실수로 했을때 다시 되돌리는 기능
}

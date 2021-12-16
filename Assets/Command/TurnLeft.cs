using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeft : Command
{
    private TestController m_controller;
    public TurnLeft(TestController controller)
    {
        m_controller = controller;
    }
    public override void Execute()
    {
        m_controller.Turn(TestController.Direction.Left);
    }
    public override void Undo()
    {
        m_controller.Turn(TestController.Direction.RIght);
    }
    public override void Redo()
    {
        m_controller.Turn(TestController.Direction.Left);
    }
}


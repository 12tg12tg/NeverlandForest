using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Invoker : MonoBehaviour
{
    private bool isRecording;
    private bool isReplaying;
    private float replayTime;
    private float recordingTime;

    private SortedList<float, Command> recordedCommands =
        new SortedList<float, Command>();
    public Command Precommand
    {
        get
        {
            return recordedCommands.Last().Value;
        }
    }
    private List<Command> undocommands = new List<Command>();
    public bool CanRedo
    {
        get
        {
          return undocommands.Count > 0;
        }
    }
    public bool CanUndo
    {
        get
        {
            return recordedCommands.Count > 0;
        }
    }

    public void ExecuteCommand(Command command)
    {
        command.Execute();
        if (isRecording)
            recordedCommands.Add(recordingTime, command);

        Debug.Log("Recorded Time: " + recordingTime);
        Debug.Log("Recorded Command: " + command);
    }
    public void UndoCommand()
    {
        var pre = Precommand;
        pre.Undo(); // 이전상태로되돌린다. ->상충되는 행동을 한다.
        Debug.Log("undoCommand : " + $"{pre}");
        undocommands.Add(pre);
        recordedCommands.RemoveAt(recordedCommands.Count -1);
    }
    public void RedoCommand()
    {
        if (undocommands.Count >0)
        {
            undocommands[undocommands.Count - 1].Redo();
            Debug.Log("redoCommand : " + $"{undocommands[undocommands.Count - 1]}");
            recordedCommands.Add(recordingTime, undocommands[undocommands.Count - 1]);

            undocommands.RemoveAt(undocommands.Count - 1);
        }
    }
    public void Record()
    {
        recordingTime = 0f;
        isRecording = true;
    }
    public void Replay()
    {
        replayTime = 0f;
        isReplaying = true;
        if (recordedCommands.Count < 0)
            Debug.LogError("No command to replay");
    }
    private void FixedUpdate()
    {
        if (isRecording)
            recordingTime += Time.fixedDeltaTime;
        if(isReplaying)
        {
            replayTime += Time.fixedDeltaTime;
            if (recordedCommands.Any())
            {
                if (Mathf.Approximately(replayTime, recordedCommands.Keys[0]))
                {
                    recordedCommands.Values[0].Execute();
                    recordedCommands.RemoveAt(0);
                }
            }
            else
            {
                isReplaying = false;    
            }
           
        }
    }
}

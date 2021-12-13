using UnityEngine;

public class TestHandler : MonoBehaviour
{
    private Invoker invoker;
    private bool isReplaying;
    private bool isRecording;
    private TestController controller;
    private Command buttonA, buttonD;

    private void Start()
    {
        invoker = gameObject.AddComponent<Invoker>();
        controller = FindObjectOfType<TestController>();

        buttonA = new TurnLeft(controller);
        buttonD = new TurnRight(controller);
    }
    private void Update()
    {
        if (!isReplaying && isRecording)
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                invoker.ExecuteCommand(buttonA);
                Debug.Log("<-");
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                invoker.ExecuteCommand(buttonD);
                Debug.Log("->");
            }
        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("Start Recording"))
        {
            controller.ResetPosition();
            isReplaying = false;
            isRecording = true;
            invoker.Record();
        }
        if (GUILayout.Button("Stop Recording"))
        {
            controller.ResetPosition();
            isReplaying = false;

        }
        if (GUILayout.Button("Start Replay"))
        {
            controller.ResetPosition();
            isRecording = false;
            isReplaying = true;
            invoker.Replay();
        }
        if (GUILayout.Button("Undo"))
        {
            invoker.UndoCommand();
        }
        if (invoker.CanRedo)
        {
            if (GUILayout.Button("Redo"))
            {
                invoker.RedoCommand();
            }
        }
    }
}

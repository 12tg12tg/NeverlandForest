using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using NewTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MultiTouch : Singleton<MultiTouch>
{
    //New Input System Controller
    private TouchInput touchInput;

    public float longTapDuration = 1f;

    // Swipe
    public float minSwipeDistanceInch = 0.25f; // Inch
    private float minSwipeDistancePixels;
    public float maxSwipeTime = 1f;

    // Zoom
    public float minZoomInch = 0.2f;
    public float maxZoomInch = 0.5f;


    public int TouchCount { get; private set; }
    //public Vector2 TapPosition { get => PrimaryPos; } 

    public bool IsTap { get; private set; }

    public bool IsDoubleTap { get; private set; }

    public bool IsLongTap { get; private set; }

    public Vector2 Swipe { get; private set; }

    public float Zoom { get; private set; }

    public float RotateAngle { get; private set; }

    public Vector2 PrimaryStartPos { get; set; }

    public Vector2 PrimaryPos { get; private set; }

    private float primaryStartTime;


    private void Awake()
    {
        touchInput = new TouchInput();
        minSwipeDistancePixels = Screen.dpi * minSwipeDistanceInch; //�ȼ����.
        //TouchSimulation.Enable(); // �갡 ����� ��Ƽ��ġ�� 3D ������Ʈ ��ġ�� �ùķ����Ϳ��� �ν� �ȴ�
    }

    private void OnEnable()
    {
        touchInput.Enable();
        EnhancedTouchSupport.Enable();
    }
    private void OnDisable()
    {
        touchInput.Disable();
        EnhancedTouchSupport.Disable();
    }
    private void Start()
    {
        touchInput.Touchs.PrimaryContactForTap.performed += OnPrimaryPerformed;

        touchInput.Touchs.PrimaryContactForSwipe.started += OnPrimaryStarted;
        touchInput.Touchs.PrimaryContactForSwipe.canceled += OnPrimaryEnded;
        touchInput.Touchs.PrimaryPostionForSwipe.performed += OnPrimaryPosition;
    }

    private void Update()
    {
        var touchList = NewTouch.activeTouches;
        TouchCount = touchList.Count;
        if (touchList.Count == 2)
        {
            UpdateDoubleTouch(touchList);
        }
    }

    private void UpdateDoubleTouch(ReadOnlyArray<NewTouch> touches)
    {
        var touch0 = touches[0];
        var touch1 = touches[1];

        // Pinch / Zoom
        var touch0PrevPos = touch0.screenPosition - touch0.delta;
        var touch1PrevPos = touch1.screenPosition - touch1.delta;

        var diffPrev = Vector2.Distance(touch0PrevPos, touch1PrevPos);
        var diffCurr = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);

        var diffPixels = diffCurr - diffPrev; //+ : Ȯ�� / - : ���
        var diffInch = diffPixels / Screen.dpi;

        diffInch = Mathf.Clamp(diffInch, -minZoomInch, maxZoomInch);
        var scale = diffInch * Time.deltaTime;
        Zoom = diffInch;


        // Rotate
        if (touch0.phase != UnityEngine.InputSystem.TouchPhase.Began && touch1.phase != UnityEngine.InputSystem.TouchPhase.Began)
        {
            var prevDir = touch1PrevPos - touch0PrevPos;
            var currDir = touch1.screenPosition - touch0.screenPosition;

            var prevDegree = Vector3.SignedAngle(Vector3.up, prevDir, -Vector3.forward);
            var currDegree = Vector3.SignedAngle(Vector3.up, currDir, -Vector3.forward);

            RotateAngle = currDegree - prevDegree;
        }
    }

    public void OnPrimaryStarted(InputAction.CallbackContext context)
    {
        PrimaryStartPos = PrimaryPos;
        primaryStartTime = (float)context.startTime;
    }
    public void OnPrimaryEnded(InputAction.CallbackContext context)
    {
        var primaryEndPos = PrimaryPos;
        var delta = primaryEndPos - PrimaryStartPos;
        var duration = context.time - primaryStartTime;
        if (delta.magnitude > minSwipeDistancePixels && duration < maxSwipeTime)
        {
            Swipe = delta;
        }
    }

    public void OnPrimaryPerformed(InputAction.CallbackContext context)
    {
        switch (context.interaction)
        {
            case MultiTapInteraction:
                IsDoubleTap = true;
                break;
            case SlowTapInteraction:
                IsLongTap = true;
                break;
            case TapInteraction:
                IsTap = true;
                break;
        }
        // �ӽ��߰�
        primaryPos = context.ReadValue<Vector2>();
        Debug.Log(primaryPos);
    }

    public void OnPrimaryPosition(InputAction.CallbackContext context)
    {
        PrimaryPos = context.ReadValue<Vector2>();
    }

    

    private void LateUpdate()
    {
        IsDoubleTap = false;
        IsLongTap = false;
        IsTap = false;
        Swipe = Vector2.zero;
        Zoom = 0f;
        RotateAngle = 0f;
    }

}
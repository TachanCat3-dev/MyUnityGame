using UnityEngine;
using UnityEngine.InputSystem; // 必须引用这个命名空间

public class CameraRig : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 10f;
    public float moveSmoothing = 10f;

    [Header("旋转设置")]
    public float rotateSpeed = 100f;
    public float rotateSmoothing = 10f;

    [Header("缩放设置")]
    public float zoomSpeed = 0.5f; // 速度设小点
    public float zoomSmoothing = 5f;
    public Vector2 zoomRange = new Vector2(5f, 25f); // x是最小，y是最大

    [Header("归零设置")]
    public Transform followTarget;

    // 内部变量
    private GameControl controls; // 对应生成的 C# 类
    private Transform cameraTransform;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float targetZoom;

    void Awake()
    {
        // 1. 初始化输入系统
        controls = new GameControl();

        targetPosition = transform.position;
        targetRotation = transform.rotation;

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            targetZoom = cameraTransform.localPosition.magnitude;
        }
    }

    // 2. 必须在 OnEnable 和 OnDisable 开关输入监听
    void OnEnable() => controls.GamePlay.Enable();
    void OnDisable() => controls.GamePlay.Disable();

    void Update()
    {
        HandleInput();
        ApplyTransform();
    }

    void HandleInput()
    {
        // === 1. 读取移动 (Vector2) ===
        // 直接从 Input System 读取刚才配置的 WASD
        Vector2 inputMove = controls.GamePlay.Move.ReadValue<Vector2>();

        if (inputMove != Vector2.zero)
        {
            // 基于当前摄像机朝向计算移动方向
            Vector3 moveDir = transform.forward * inputMove.y + transform.right * inputMove.x;
            moveDir.y = 0; // 锁定高度
            moveDir.Normalize();

            targetPosition += moveDir * moveSpeed * Time.deltaTime;
        }

        // === 2. 读取旋转 (Float) ===
        // 读取 Q(-1) 和 E(+1)
        float inputRotate = controls.GamePlay.Rotate.ReadValue<float>();

        if (inputRotate != 0)
        {
            Vector3 currentEuler = targetRotation.eulerAngles;
            // 注意：这里用 inputRotate * 旋转速度
            float newY = currentEuler.y + inputRotate * rotateSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(0, newY, 0);
        }

        // === 3. 读取缩放 (Float) ===
        // 读取鼠标滚轮 Y 值
        float inputZoom = controls.GamePlay.Zoom.ReadValue<float>();

        if (inputZoom != 0)
        {
            // Input System 的滚轮值通常是 120 的倍数，所以要 clamp 或者乘一个小系数
            // 这里我们直接反向减去输入值
            targetZoom -= inputZoom * zoomSpeed * 1f; // 0.01f 是用来把滚轮的大数值缩小
            targetZoom = Mathf.Clamp(targetZoom, zoomRange.x, zoomRange.y);
        }

        // === 4. 读取归位 ===
        //读取Space键归位
        if (controls.GamePlay.Recenter.WasPressedThisFrame())
        {
            if (followTarget != null)
            {
                // 把目标位置直接设为玩家的位置
                targetPosition = followTarget.position;
                Debug.Log("视角已归位！");
            }
            else
            {
                Debug.LogWarning("你还没把 Player 拖到 CameraRig 的 Follow Target 槽里！");
            }
        }

    }

    void ApplyTransform()
    {
        // 平滑插值移动
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSmoothing);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSmoothing);

        // 处理子摄像机的拉近拉远
        if (cameraTransform != null)
        {
            // 1. Rig 本身的移动和旋转 (保持不变)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSmoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSmoothing);

            // 2. 子摄像机的拉近拉远 (修改这里！)
            if (cameraTransform != null)
            {
                // === 核心修改开始 ===
                // 不要写死 (0, 1, -1)，而是直接用摄像机当前角度的“正后方”
                // 这样无论你在 Inspector 里把旋转改成 45度、60度还是 30度，它永远都在正中心！
                Vector3 zoomDir = cameraTransform.localRotation * -Vector3.forward;

                // 应用位置
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomDir * targetZoom, Time.deltaTime * zoomSmoothing);
                // === 核心修改结束 ===
            }
        }
    }
    [Header("调试设置")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.yellow;

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // 设置颜色
        Gizmos.color = gizmoColor;

        // 1. 在目标位置画一个空心球 (半径 0.5 米)
        // 注意：这里用 targetPosition，就是你代码里摄像机插值移动的目标
        Gizmos.DrawWireSphere(targetPosition, 0.5f);

        // 2. 画一条线，连接摄像机和目标点 (方便看距离)
        if (Camera.main != null)
        {
            Gizmos.DrawLine(Camera.main.transform.position, targetPosition);
        }
    }
}
using UnityEngine;

// ✅ 必须是 public，否则子类 Player 访问不了
public class Entity : MonoBehaviour
{
    [Header("Base Attributes")]
    public float moveSpeed = 5f;

    // 子类继承这些变量，不要在 Player 里再写一遍了
    public Rigidbody rb { get; protected set; }
    public Animator anim { get; protected set; }
    public StateMachine StateMachine { get; protected set; } // 注意这里首字母大写

    // ✅ 改为 virtual protected，允许子类重写并补充逻辑
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        StateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        // 留给子类去初始化具体的 State
    }

    protected virtual void Update()
    {
        // 驱动状态机
        if (StateMachine != null)
        {
            StateMachine.UpdateActiveState();
        }
    }
}
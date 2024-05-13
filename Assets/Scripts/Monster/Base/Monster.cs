using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnumType;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FieldOfView))]
public abstract class Monster : BaseMonoBehaviour
{
    public event Action HPChanged;

    [field: SerializeField]
    public MonsterData Data { get; private set; }

    [field: Header("플레이어 추적")]
    [field: SerializeField]
    public float TrackingDistance { get; private set; }

    [field: Header("전투")]
    [field: SerializeField]
    public float AttackDistance { get; private set; }

    [field: SerializeField]
    public float RotationSmoothTime { get; private set; }

    [field: SerializeField]
    public float AttackDelayTime { get; private set; }

    [field: SerializeField]
    public Transform AttackOffset { get; private set; }

    [field: SerializeField]
    public float AttackRadius { get; private set; }

    [field: SerializeField]
    public string DamagedPrefabAddress;

    public Collider Collider { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public FieldOfView Fov { get; private set; }

    public int CurrentHP { get; set; }
    public int CurrentDamage { get; private set; }
    public Vector3 OriginalPosition { get; private set; }
    public bool IsLockOnTarget { get; private set; }
    public IReadOnlyList<Collider> LockOnTargetColliders => _lockOnTargetColliders;

    protected MonsterState CurrentState = MonsterState.Idle;
    protected readonly Collider[] PlayerCollider = new Collider[1];

    private readonly List<Collider> _lockOnTargetColliders = new();
    private UI_MonsterHPBar _hpBar;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Fov = GetComponent<FieldOfView>();

        foreach (var lockOnTarget in GetComponentsInChildren<LockOnTarget>())
        {
            lockOnTarget.LockChanged += lockOn =>
            {
                if (lockOn)
                {
                    ShowHPBar();
                }

                IsLockOnTarget = lockOn;
            };

            _lockOnTargetColliders.Add(lockOnTarget.GetComponent<Collider>());
        }
    }

    private void Start()
    {
        Util.InstantiateMinimapIcon("MonsterMinimapIcon.sprite", Data.MonsterName, transform);
    }

    private void OnEnable()
    {
        CurrentHP = Data.MaxHP;
        OriginalPosition = transform.position;
        Collider.isTrigger = false;
        foreach (var collider in _lockOnTargetColliders)
        {
            collider.enabled = true;
        }
    }

    public void Transition(MonsterState state, float fade = 0.25f)
    {
        CurrentState = state;
        Animator.CrossFade(state.ToString(), fade, -1, 0f);
    }

    public void SetActiveNaveMeshAgentUpdate(bool active)
    {
        NavMeshAgent.isStopped = !active;
        NavMeshAgent.updatePosition = active;
        NavMeshAgent.updateRotation = active;
        if (!active)
        {
            NavMeshAgent.velocity = Vector3.zero;
        }
    }

    private void ShowHPBar()
    {
        if (_hpBar == null)
        {
            var go = Managers.Resource.Instantiate("UI_MonsterHPBar.prefab", null, true);
            _hpBar = go.GetComponent<UI_MonsterHPBar>();
            _hpBar.SetTarget(this);
        }
        else
        {
            HPChanged?.Invoke();
        }
    }
}

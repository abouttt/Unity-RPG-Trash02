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

    public MonsterState CurrentState { get; protected set; } = MonsterState.Idle;
    public int CurrentHP { get; set; }
    public int CurrentDamage { get; private set; }
    public Vector3 OriginalPosition { get; private set; }
    public bool IsLockOnTarget { get; private set; }
    public IReadOnlyList<Collider> LockOnTargetColliders => _lockOnTargetColliders;

    public readonly int AnimIDIdle = Animator.StringToHash("Idle");
    public readonly int AnimIDTracking = Animator.StringToHash("Tracking");
    public readonly int AnimIDRestore = Animator.StringToHash("Restore");
    public readonly int AnimIDAttack = Animator.StringToHash("Attack");
    public readonly int AnimIDStunned = Animator.StringToHash("Stunned");

    protected readonly Collider[] PlayerCollider = new Collider[1];

    private readonly List<Collider> _lockOnTargetColliders = new();
    private readonly Dictionary<MonsterState, int> _stateAnimID = new();
    private UI_MonsterHPBar _hpBar;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Fov = GetComponent<FieldOfView>();

        _stateAnimID.Add(MonsterState.Idle, AnimIDIdle);
        _stateAnimID.Add(MonsterState.Tracking, AnimIDTracking);
        _stateAnimID.Add(MonsterState.Restore, AnimIDRestore);
        _stateAnimID.Add(MonsterState.Attack, AnimIDAttack);
        _stateAnimID.Add(MonsterState.Stunned, AnimIDStunned);
        _stateAnimID.Add(MonsterState.Damaged, -1);
        _stateAnimID.Add(MonsterState.Death, -1);

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

        NavMeshAgent.updateRotation = false;
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

    public void Transition(MonsterState state)
    {
        CurrentState = state;

        if (_stateAnimID[state] == -1)
        {
            Animator.Play(state.ToString(), -1, 0f);
        }
        else
        {
            Animator.SetTrigger(_stateAnimID[state]);
        }
    }

    public void Rotation(Vector3 target)
    {
        var dir = target - transform.position;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), RotationSmoothTime * Time.deltaTime);
        }
    }

    public void SetActiveNaveMeshAgentUpdate(bool active)
    {
        NavMeshAgent.isStopped = !active;
        NavMeshAgent.updatePosition = active;
        if (!active)
        {
            NavMeshAgent.velocity = Vector3.zero;
        }
    }

    public bool TakeDamage(int damage)
    {
        if (CurrentHP <= 0)
        {
            return false;
        }

        ShowHPBar();

        CurrentDamage = Util.CalcDamage(damage, Data.Defense);
        CurrentHP -= CurrentDamage;
        HPChanged?.Invoke();
        CurrentDamage = 0;
        Managers.Resource.Instantiate(DamagedPrefabAddress, Collider.bounds.center, transform, true);

        if (CurrentHP <= 0)
        {
            Transition(MonsterState.Death);
        }
        else
        {
            Transition(MonsterState.Damaged);
        }

        return true;
    }

    public void Stunned()
    {
        if (CurrentHP <= 0)
        {
            return;
        }

        Transition(MonsterState.Stunned);
    }

    public void ResetAllTriggers()
    {
        foreach (var param in Animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                Animator.ResetTrigger(param.name);
            }
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

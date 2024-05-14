using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using EnumType;
using Structs;

public class PlayerStatus : BaseMonoBehaviour, ISavable
{
    public static string SaveKey => "SaveStatus";

    public event Action LevelChanged;
    public event Action HPChanged;
    public event Action MPChanged;
    public event Action SPChanged;
    public event Action XPChanged;
    public event Action StatChanged;
    public event Action GoldChanged;
    public event Action SkillPointChanged;

    public int Level { get; private set; } = 1;

    public int HP
    {
        get => _currentStat.HP;
        set
        {
            _currentStat.HP = Mathf.Clamp(value, 0, _maxStat.HP);
            HPChanged?.Invoke();
        }
    }

    public int MP
    {
        get => _currentStat.MP;
        set
        {
            _currentStat.MP = Mathf.Clamp(value, 0, _maxStat.MP);
            MPChanged?.Invoke();
        }
    }

    public float SP
    {
        get => _currentStat.SP;
        set
        {
            float prevSP = _currentStat.SP;
            _currentStat.SP = Mathf.Clamp(value, 0f, _maxStat.SP);
            if (_currentStat.SP < prevSP)
            {
                _recoverySPDeltaTime = 0f;
            }

            SPChanged?.Invoke();
        }
    }

    public int XP
    {
        get => _currentStat.XP;
        set
        {
            if (IsMaxLevel || _maxStat.XP == 0)
            {
                return;
            }

            _currentStat.XP = value;

            int level = 0;
            while (_currentStat.XP >= _maxStat.XP)
            {
                _currentStat.XP -= _maxStat.XP;
                level++;
            }

            if (level > 0)
            {
                LevelUp(level);
            }

            XPChanged?.Invoke();
        }
    }

    public int Damage => _currentStat.Damage;
    public int Defense => _currentStat.Defense;

    public PlayerStat ExtraFixedStat
    {
        get => _extraFixedStat;
        set
        {
            _extraFixedStat = value;
            RefreshAllStat();
            StatChanged?.Invoke();
        }
    }

    public PlayerStat ExtraPerStat
    {
        get => _extraPerStat;
        set
        {
            _extraPerStat = value;
            RefreshAllStat();
            StatChanged?.Invoke();
        }
    }

    public int Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            GoldChanged?.Invoke();
        }
    }

    public int SkillPoint
    {
        get => _skillPoint;
        set
        {
            _skillPoint = value;
            SkillPointChanged?.Invoke();
        }
    }

    public bool IsMaxLevel => Level >= _playerStatTable.StatTable.Count;
    public int MaxHP => _maxStat.HP;
    public int MaxMP => _maxStat.MP;
    public int MaxSP => (int)_maxStat.SP;
    public int MaxXP => _maxStat.XP;
    public int MaxDamage => _maxStat.Damage;
    public int MaxDefense => _maxStat.Defense;

    [SerializeField]
    private PlayerStatTable _playerStatTable;

    [SerializeField]
    private float _recoverySPDelay;

    [SerializeField]
    private float _recoverySPAmount;

    private readonly PlayerStat _maxStat = new();
    private readonly PlayerStat _currentStat = new();
    private PlayerStat _extraFixedStat = new();
    private PlayerStat _extraPerStat = new();

    private int _gold;
    private int _skillPoint;
    private float _recoverySPDeltaTime; // SP 회복 현재 딜레이 시간

    private void Awake()
    {
        Player.EquipmentInventory.InventoryChanged += equipmentType =>
        {
            RefreshAllStat();
            _currentStat.HP = Mathf.Clamp(_currentStat.HP, _currentStat.HP, _maxStat.HP);
            _currentStat.MP = Mathf.Clamp(_currentStat.MP, _currentStat.MP, _maxStat.MP);
            _currentStat.SP = Mathf.Clamp(_currentStat.SP, _currentStat.SP, _maxStat.SP);
            StatChanged?.Invoke();
        };

        Load();
        RefreshAllStat();
        FillMeleeStat();
        if (Managers.Data.HasSaveData)
        {
            SP = _maxStat.SP;
        }
        else
        {
            FillAbilityStat();
        }
    }

    private void Start()
    {
        StatChanged?.Invoke();
    }

    private void Update()
    {
        RecoverySP(Time.deltaTime);
    }

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        var statusSaveData = new StatusSaveData()
        {
            Level = Level,
            CurrentHP = _currentStat.HP,
            CurrentMP = _currentStat.MP,
            CurrentXP = _currentStat.XP,
            Gold = Gold,
            SkillPoint = SkillPoint,
        };

        saveData.Add(JObject.FromObject(statusSaveData));

        return saveData;
    }

    private void RefreshAllStat()
    {
        int level = (IsMaxLevel ? _playerStatTable.StatTable.Count : Level) - 1;
        _maxStat.HP = _playerStatTable.StatTable[level].HP + _extraFixedStat.HP;
        _maxStat.MP = _playerStatTable.StatTable[level].MP + _extraFixedStat.MP;
        _maxStat.SP = _playerStatTable.StatTable[level].SP + _extraFixedStat.SP;
        _maxStat.XP = _playerStatTable.StatTable[level].XP;
        _maxStat.Damage = _playerStatTable.StatTable[level].Damage + _extraFixedStat.Damage;
        _maxStat.Defense = _playerStatTable.StatTable[level].Defense + _extraFixedStat.Defense;

        var types = Enum.GetValues(typeof(EquipmentType));
        foreach (EquipmentType equipmentType in types)
        {
            var equipment = Player.EquipmentInventory.GetItem(equipmentType);
            if (equipment == null)
            {
                continue;
            }

            _maxStat.HP += equipment.EquipmentData.HP;
            _maxStat.MP += equipment.EquipmentData.MP;
            _maxStat.SP += equipment.EquipmentData.SP;
            _maxStat.Damage += equipment.EquipmentData.Damage;
            _maxStat.Defense += equipment.EquipmentData.Defense;
        }

        _maxStat.HP = Util.CalcIncreasePer(_maxStat.HP, _extraPerStat.HP);
        _maxStat.MP = Util.CalcIncreasePer(_maxStat.MP, _extraPerStat.MP);
        _maxStat.SP = Util.CalcIncreasePer((int)_maxStat.SP, (int)_extraPerStat.SP);
        _maxStat.Damage = Util.CalcIncreasePer(_maxStat.Damage, _extraPerStat.Damage);
        _maxStat.Defense = Util.CalcIncreasePer(_maxStat.Defense, _extraPerStat.Defense);

        FillMeleeStat();
    }

    private void FillAllStat()
    {
        FillAbilityStat();
        FillMeleeStat();
    }

    private void FillAbilityStat()
    {
        _currentStat.HP = _maxStat.HP;
        _currentStat.MP = _maxStat.MP;
        _currentStat.SP = _maxStat.SP;
    }

    private void FillMeleeStat()
    {
        _currentStat.Damage = _maxStat.Damage;
        _currentStat.Defense = _maxStat.Defense;
    }

    private void LevelUp(int level)
    {
        if (level <= 0)
        {
            return;
        }

        Level += level;
        RefreshAllStat();
        FillAllStat();
        Managers.Resource.Instantiate("LevelUp.prefab", transform);
        LevelChanged?.Invoke();
        StatChanged?.Invoke();
    }

    // SP 딜레이 시간이 넘으면 SP회복
    private void RecoverySP(float deltaTime)
    {
        if (!Player.Movement.IsGrounded)
        {
            _recoverySPDeltaTime = 0f;
            return;
        }

        _recoverySPDeltaTime += deltaTime;
        if (_recoverySPDeltaTime >= _recoverySPDelay)
        {
            if (SP < _maxStat.SP)
            {
                SP += Mathf.Clamp(_recoverySPAmount * deltaTime, 0f, _maxStat.SP);
            }
        }
    }

    public void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        var statusSaveData = saveData[0].ToObject<StatusSaveData>();

        Level = statusSaveData.Level;
        _gold = statusSaveData.Gold;
        _skillPoint = statusSaveData.SkillPoint;
        _currentStat.HP = statusSaveData.CurrentHP;
        _currentStat.MP = statusSaveData.CurrentMP;
        _currentStat.XP = statusSaveData.CurrentXP;
    }
}

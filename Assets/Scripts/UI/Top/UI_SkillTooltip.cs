using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_SkillTooltip : UI_Base
{
    enum GameObjects
    {
        Tooltip,
    }

    enum Texts
    {
        SkillNameText,
        SkillTypeText,
        SkillDescText,
    }

    [SerializeField]
    [Tooltip("Distance from mouse")]
    private Vector2 _offset;

    private UI_BaseSlot _slot;
    private SkillData _skillDataRef;
    private RectTransform _rt;
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        _rt = GetObject((int)GameObjects.Tooltip).GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GetObject((int)GameObjects.Tooltip).SetActive(false);
    }

    private void Update()
    {
        if (_slot == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!_slot.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            return;
        }

        if (UI_BaseSlot.IsDragging)
        {
            GetObject((int)GameObjects.Tooltip).SetActive(false);
            return;
        }

        if (_slot.HasObject)
        {
            if (_slot.ObjectRef is Skill skill)
            {
                SetSkillData(skill.Data);
            }
            else if (_slot.ObjectRef is SkillData skillData)
            {
                SetSkillData(skillData);
            }
        }
        else
        {
            GetObject((int)GameObjects.Tooltip).SetActive(false);
        }

        SetPosition(Mouse.current.position.ReadValue());
    }

    public void SetSlot(UI_BaseSlot slot)
    {
        _slot = slot;
        gameObject.SetActive(slot != null);
    }

    private void SetSkillData(SkillData skillData)
    {
        GetObject((int)GameObjects.Tooltip).SetActive(true);

        if (_skillDataRef != null)
        {
            SetDescription(skillData);

            if (_skillDataRef.Equals(skillData))
            {
                return;
            }
        }

        _skillDataRef = skillData;
        GetText((int)Texts.SkillNameText).text = skillData.SkillName;
        GetText((int)Texts.SkillTypeText).text = $"[{skillData.SkillType}]";
        SetDescription(skillData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
    }

    private void SetDescription(SkillData skillData)
    {
        _sb.Clear();
        _sb.AppendFormat("{0}\n\n", _skillDataRef.Description);
        _sb.AppendFormat("※습득조건※\n");

        var skill = Player.SkillTree.GetSkillByData(skillData);

        foreach (var parent in skill.Parents)
        {
            if (parent.Children.TryGetValue(skill, out var level))
            {
                _sb.AppendFormat("- {0} Lv.{1}\n", parent.Data.SkillName, level);
            }
        }

        _sb.AppendFormat("- 필요 스킬 포인트 : {0}\n", _skillDataRef.RequiredSkillPoint);
        _sb.AppendFormat("- 제한레벨 : {0}\n\n", _skillDataRef.LimitLevel);
        if (!string.IsNullOrEmpty(_skillDataRef.StatDescription))
        {
            _sb.AppendFormat("{0}\n", _skillDataRef.StatDescription);
        }

        GetText((int)Texts.SkillDescText).text = _sb.ToString();
    }

    private void SetPosition(Vector3 position)
    {
        var nextPosition = new Vector3()
        {
            x = position.x + (_rt.rect.width * 0.5f) + _offset.x,
            y = position.y + (_rt.rect.height * 0.5f) + _offset.y
        };

        _rt.position = nextPosition;
    }
}

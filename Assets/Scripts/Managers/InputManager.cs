using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : GameControls.IPlayerActions
{
    // Value
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }

    // Button
    public bool Sprint { get; private set; }
    public bool Jump { get; private set; }
    public bool Roll { get; private set; }
    public bool LockOn { get; private set; }
    public bool Interaction { get; private set; }
    public bool Attack { get; private set; }
    public bool Parry { get; private set; }
    public bool Defense { get; private set; }

    public bool CursorLocked
    {
        get => _cursorLocked;
        set
        {
            _cursorLocked = value;
            Cursor.visible = !value;
            SetCursorState(value);
        }
    }

    private GameControls _controls;
    private bool _cursorLocked = true;

    public void Init()
    {
        if (_controls == null)
        {
            _controls = new();
            _controls.Player.SetCallbacks(this);
        }

        _controls.Enable();
    }

    public InputAction GetAction(string actionNameOrId)
    {
        return _controls.FindAction(actionNameOrId);
    }

    public string GetBindingPath(string actionNameOrId, int bindingIndex = 0)
    {
        var key = _controls.FindAction(actionNameOrId).bindings[bindingIndex].path;
        return key.GetLastSlashString().ToUpper();
    }

    public void Clear()
    {
        _controls.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint = context.ReadValueAsButton();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (CursorLocked)
        {
            Look = context.ReadValue<Vector2>();
        }
        else
        {
            Look = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        Roll = context.ReadValueAsButton();
    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        LockOn = context.ReadValueAsButton();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        Interaction = context.ReadValueAsButton();
    }

    public void OnCursorToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CursorLocked = !_cursorLocked;
        }
    }

    public void OnItemInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_ItemInventoryPopup>(context);
    }

    public void OnEquipmentInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_EquipmentInventoryPopup>(context);
    }

    public void OnSkillTree(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_SkillTreePopup>(context);
    }

    public void OnQuick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Managers.UI.IsShowed<UI_ItemSplitPopup>())
            {
                return;
            }

            int index = (int)context.ReadValue<float>();
            Player.QuickInventory.GetUsable(index)?.Use();
        }
    }

    public void OnQuest(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_QuestPopup>(context);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Managers.UI.ActivePopupCount > 0)
            {
                Managers.UI.CloseTopPopup();
            }
            else
            {
                Managers.UI.Show<UI_GameMenuPopup>();
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        Attack = context.ReadValueAsButton();
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        Parry = context.ReadValueAsButton();
    }

    public void OnDefense(InputAction.CallbackContext context)
    {
        Defense = context.ReadValueAsButton();
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void ShowOrClosePopup<T>(InputAction.CallbackContext context) where T : UI_Popup
    {
        if (context.performed)
        {
            if (Managers.UI.IsShowed<UI_ConfirmationPopup>() || Managers.UI.IsShowed<UI_ItemSplitPopup>())
            {
                return;
            }

            Managers.UI.ShowOrClose<T>();
        }
    }
}

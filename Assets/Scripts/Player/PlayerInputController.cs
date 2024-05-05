using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : BaseMonoBehaviour
{
    public PlayerInput PlayerInput { get; private set; }

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

    private bool _cursorLocked = true;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
    }

    public string GetBindingPath(string actionNameOrId, int bindingIndex = 0)
    {
        var key = PlayerInput.currentActionMap.FindAction(actionNameOrId).bindings[bindingIndex].path;
        return key.GetLastSlashString().ToUpper();
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnCursorToggle(InputValue inputValue)
    {
        CursorLocked = !_cursorLocked;
    }

    private void OnItemInventory(InputValue inputValue)
    {
        Managers.UI.ShowOrClose<UI_ItemInventoryPopup>();
    }

    private void OnEquipmentInventory(InputValue inputValue)
    {
        Managers.UI.ShowOrClose<UI_EquipmentInventoryPopup>();
    }

    private void OnQuick(InputValue inputValue)
    {
        if (Managers.UI.IsShowed<UI_ItemSplitPopup>())
        {
            return;
        }

        int index = (int)inputValue.Get<float>();
        Player.QuickInventory.GetUsable(index)?.Use();
    }

    private void OnCancel(InputValue inputValue)
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

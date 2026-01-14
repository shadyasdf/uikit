using System.Linq;
using TMPro;
using UIKit;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIK2DActionDisplayButton : UIK2DButton
{
    [SerializeField] protected TMP_Text text;
    [SerializeField] protected Image image;

    [Space(10)]
    [Header("Editor Preview Only")]
    [SerializeField] protected string editorPreviewText;
    [SerializeField] protected Sprite editorPreviewIcon;
    
    
    protected override void OnPreConstruct(bool _isOnValidate)
    {
        base.OnPreConstruct(_isOnValidate);
        
        if (!_isOnValidate
            && GetOwningPlayer() is UIKPlayer player)
        {
            player.OnInputActionMapChanged.AddListener(Player_OnInputActionMapChanged);
            player.OnInputDeviceTypeChanged.AddListener(Player_OnInputDeviceTypeChanged);
        }

        if (_isOnValidate)
        {
            text.SetText(editorPreviewText);
            image.sprite = editorPreviewIcon;
        }
        else
        {
            UpdateDisplay();
        }
    }
    
    protected virtual void UpdateDisplay()
    {
        if (clickInputAction == null
            || !clickInputAction.IsValid())
        {
            UpdateDisplayInputAction(null);
            
            return;
        }

        if (GetOwningPlayer() is UIKPlayer player
            && player.playerInput is PlayerInput playerInput
            && playerInput.currentActionMap.actions.FirstOrDefault(a => clickInputAction == a) is InputAction inputAction)
        {
            UpdateDisplayInputAction(inputAction);
            
            return;
        }

        UpdateDisplayInputAction(null);
    }

    protected virtual void UpdateDisplayInputAction(InputAction _inputAction)
    {
        if (text == null
            || image == null)
        {
            return;
        }
        
        if (GetOwningPlayer() is UIKPlayer player
            && player.canvas is UIKCanvas canvas)
        {
            text.SetText(clickActionObject.GetActionText(player));
            image.sprite = canvas.GetInputActionIcon(_inputAction);
        }
        else
        {
            text.SetText(string.Empty);
            image.sprite = null;
        }
    }

    private void Player_OnInputActionMapChanged(string _actionMap)
    {
        UpdateDisplay();
    }

    private void Player_OnInputDeviceTypeChanged(UIKInputDevice _inputDeviceType)
    {
        UpdateDisplay();
    }
}

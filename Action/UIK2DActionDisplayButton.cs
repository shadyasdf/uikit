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
    [SerializeField] protected CanvasGroup canvasGroup;
    
    [Space(10)]
    [Header("Editor Preview Only")]
    [SerializeField] protected string editorPreviewText;
    [SerializeField] protected Sprite editorPreviewIcon;

    private bool canExecute;
    private bool canClick;
    
    
    protected override void OnPreConstruct(bool _isOnValidate)
    {
        base.OnPreConstruct(_isOnValidate);
        
        if (!_isOnValidate
            && GetOwningPlayer() is UIKPlayer player)
        {
            player.OnInputActionMapChanged.AddListener(Player_OnInputActionMapChanged);
            player.OnInputDeviceTypeChanged.AddListener(Player_OnInputDeviceTypeChanged);
            UpdateCanClick(player.inputDeviceType.UsesCursor());
            
            if (clickActionObject != null
                && clickActionObject.GetActionObject(player) is UIKActionObject actionObjectInstance)
            {
                actionObjectInstance.OnCanExecuteChanged.AddListener(ActionObject_OnCanExecuteChanged);
                ActionObject_OnCanExecuteChanged(actionObjectInstance.CanExecute());
            }
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

    protected virtual void UpdateCanInteract(bool _canExecute)
    {
        canExecute = _canExecute;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = canExecute ? 1f : 0.5f;
            canvasGroup.interactable = canExecute && canClick;
            canvasGroup.blocksRaycasts = canExecute && canClick;
        }
    }

    protected virtual void UpdateCanClick(bool _canClick)
    {
        canClick = _canClick;
        
        style.SetGraphicVisible(canClick);
        
        canvasGroup.interactable = canExecute && canClick;
        canvasGroup.blocksRaycasts = canExecute && canClick;
    }

    private void Player_OnInputActionMapChanged(string _actionMap)
    {
        UpdateDisplay();
    }

    private void Player_OnInputDeviceTypeChanged(UIKInputDevice _inputDeviceType)
    {
        UpdateDisplay();
        UpdateCanClick(_inputDeviceType.UsesCursor());
    }

    private void ActionObject_OnCanExecuteChanged(bool _canExecute)
    {
        UpdateCanInteract(_canExecute);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class ToogleableButton : MonoBehaviour, IPointerClickHandler
{
    public bool active;

    [Header("Event Logic")]
    public UnityEvent OnClick;
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;


    public enum ToogleableButtonType
    {
        ChangeColor,
        ChangeImage
    }

    [Header("Visuals")]
    public ToogleableButtonType buttonType;

    //-----type == ChangeColor---------
    [ShowWhen("buttonType", ToogleableButtonType.ChangeColor)]
    public Color activeColor;
    [ShowWhen("buttonType", ToogleableButtonType.ChangeColor)]
    public Color inactiveColor;
    [ShowWhen("buttonType", ToogleableButtonType.ChangeColor)]
    public Image imageWhichColorToChange;


    //-----type == ChangeImage---------
    [ShowWhen("buttonType", ToogleableButtonType.ChangeImage)]
    public Sprite activeSprite;
    [ShowWhen("buttonType", ToogleableButtonType.ChangeImage)]
    public Sprite inactiveSprite;
    [ShowWhen("buttonType", ToogleableButtonType.ChangeImage)]
    public Image imageWhichSpriteToChange;


    private void Start()
    {
        if (active)
        {
            ActivateVisuals();
        }
        else
        {
            DeactivateVisuals();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        active = !active;
        OnClick.Invoke();
        if (active)
        {
            OnActivate.Invoke();

            ActivateVisuals();
        }
        else
        {
            OnDeactivate.Invoke();

            DeactivateVisuals();
        }
    }

    void ActivateVisuals()
    {
        if (buttonType == ToogleableButtonType.ChangeColor)
        {
            imageWhichColorToChange.color = activeColor;
        }
        else if (buttonType == ToogleableButtonType.ChangeImage)
        {
            imageWhichSpriteToChange.sprite = activeSprite;
        }
    }

    void DeactivateVisuals()
    {
        if (buttonType == ToogleableButtonType.ChangeColor)
        {
            imageWhichColorToChange.color = inactiveColor;
        }
        else if (buttonType == ToogleableButtonType.ChangeImage)
        {
            imageWhichSpriteToChange.sprite = inactiveSprite;
        }
    }

    public void SetActiveExternally(bool active)
    {
        this.active = active;
        if (active)
        {
            ActivateVisuals();
        }
        else
        {

            DeactivateVisuals();
        }
    }


}

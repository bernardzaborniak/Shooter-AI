using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

//simple clickeable button, cause ive had problems with the unity one
public class ClickeableButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnLmB;
    public UnityEvent OnRmb;
    public Image image;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLmB.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRmb.Invoke();
        }
    }

    public void SetColor(Color color)
    {
        //setting image thwice because otherwise this custom shader wont notice the color change
        image.color = color;
        image.color = color;
    }
}

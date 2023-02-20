using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class PopUpWindow : MonoBehaviour
{
    public RectTransform rectTransform;
    
    public Button xButton;
    public Button leftButton;
    public TextMeshProUGUI leftButtonText;
    public Button rightButton;
    public TextMeshProUGUI rightButtonText;

    public TextMeshProUGUI messageText;

    private UnityEvent onXButton = new UnityEvent();
    private UnityEvent onRightButton = new UnityEvent();
    private UnityEvent onLeftButton = new UnityEvent();

    public void Initialize(UnityAction xAction, UnityAction leftAction, UnityAction rightAction, Vector3 worldSpacePos, string _messageText, string _leftButtonText, string _rightButtonText, bool hideLeftButton, bool hideRightButton)
    {
        if (hideLeftButton)
        {
            leftButton.gameObject.SetActive(false);
        }
        else
        {
            leftButtonText.text = _leftButtonText;
            if (leftAction != null) onLeftButton.AddListener(leftAction);
        }
        if (hideRightButton)
        {
            rightButton.gameObject.SetActive(false);
        }
        else
        {
            if (rightAction != null) onRightButton.AddListener(rightAction);
            rightButtonText.text = _rightButtonText;
        }

        if (xAction != null) onXButton.AddListener(xAction);

        messageText.text = _messageText;
  
        rectTransform.position = Camera.main.WorldToScreenPoint(worldSpacePos);
    }

    public void OnXButton()
    {
        onXButton?.Invoke();
        ClosePopUp();
    }

    public void OnRightButton()
    {
        onRightButton?.Invoke();
        ClosePopUp();
    }

    public void OnLeftButton()
    {
        onLeftButton?.Invoke();
        ClosePopUp();
    }

    public void ClosePopUp()
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITouch : MonoBehaviour, ITouchedObject
{
    public UnityEvent UIEvent {
        get => currentButton.onClick;
    }
    
    [SerializeField] private Button currentButton;


    void Awake() {
        if(currentButton is null)
            currentButton = GetComponent<Button>();
    }
}

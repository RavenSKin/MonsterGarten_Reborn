using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextText : MonoBehaviour
{
    public string _text;
    [SerializeField] private TextAsset InkJson;
    void Start()
    {
        DialogManager.GetInstance().EnterDialogMode(InkJson);

    }
}

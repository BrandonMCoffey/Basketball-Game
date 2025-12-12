using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NextEffectStackUI : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _textStack = new List<TMP_Text>();

    private void OnEnable()
    {
        PlayerManager.UpdateEffectNextStack += RefreshStack;
        RefreshStack();
    }

    private void OnDisable()
    {
        PlayerManager.UpdateEffectNextStack -= RefreshStack;
    }

    private void Start()
    {
        RefreshStack();
    }

    private void RefreshStack()
    {
        if (PlayerManager.Instance == null) return;
        var stack = PlayerManager.Instance.EffectNextStack;
        int i = 0;
        for (; i < stack.Count; i++)
        {
            _textStack[i].text = stack[i].GetDisplayText();
        }
        for (; i < _textStack.Count; i++)
        {
            _textStack[i].text = "";
        }
    }
}
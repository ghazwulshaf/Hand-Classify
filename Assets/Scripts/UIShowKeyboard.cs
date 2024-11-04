using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShowKeyboard : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        inputField.GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(delegate {OpenKeyboard();});
    }

    private void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);
    }
}

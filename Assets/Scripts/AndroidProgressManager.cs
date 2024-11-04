using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class AndroidProgressManager : MonoBehaviour
{
    public Text progressData;

    void Update()
    {
        Image progressFill = gameObject.GetComponent<Image>();

        NumberFormatInfo provider = new()
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };
        progressFill.fillAmount = float.Parse(progressData.text, provider);
    }
}

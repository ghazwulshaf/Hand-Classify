using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AndroidFieldManager : MonoBehaviour
{
    // field
    public Text display;
    public Text field;

    void Start()
    {
        Dropdown dropdown = gameObject.GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate
        {
            int index = dropdown.value;
            string value = dropdown.options[index].text;

            field.text = value;
            display.text += $"Change to {value}\n";
        });
    }
}

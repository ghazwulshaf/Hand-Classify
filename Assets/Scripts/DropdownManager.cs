using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    // UI
    public Text display;
    public Dropdown dropSensor;
    public Dropdown dropRange;
    public Dropdown dropSize;
    public GameObject publicSensor;
    public GameObject publicRange;
    public GameObject publicSize;

    public void SensorChange()
    {
        var sensorIndex = dropSensor.value;
        var sensor = dropSensor.options[sensorIndex].text;
        publicSensor.GetComponent<Text>().text = sensor;
        display.text = "Sensor: " + publicSensor.GetComponent<Text>().text;
    }

    public void RangeChange()
    {
        var rangeIndex = dropRange.value;
        var range = dropRange.options[rangeIndex].text;
        publicRange.GetComponent<Text>().text = range;
        display.text = "Range: " + publicRange.GetComponent<Text>().text;
    }

    public void SizeChange()
    {
        var sizeIndex = dropSize.value;
        var size = dropSize.options[sizeIndex].text;
        publicSize.GetComponent<Text>().text = size;
        display.text = "Size: " + publicSize.GetComponent<Text>().text;
    }
}

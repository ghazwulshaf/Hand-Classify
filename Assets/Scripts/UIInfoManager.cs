using UnityEngine;
using UnityEngine.UI;

public class UIInfoManager : MonoBehaviour
{
    // sensor
    public Text infoSensor;
    public GameObject fieldSensor;

    // data size
    public Text infoDataSize;
    public GameObject fieldDataSize;

    // feature length
    public Text infoFELength;
    public GameObject fieldFELength;

    // feature model
    public Text infoFEModel;
    public GameObject fieldFEModel;

    // model epoch
    public Text infoModelEpoch;
    public GameObject fieldModelEpoch;

    // model loss
    public Text infoModelLoss;
    public GameObject fieldModelLoss;

    // model accuracy
    public Text infoModelAccuracy;
    public GameObject fieldModelAccuracy;

    // model time
    public Text infoModelTime;
    public GameObject fieldModelTime;

    void Start()
    {
        // info data
        infoSensor.text = fieldSensor.GetComponent<Text>().text;
        infoDataSize.text = fieldDataSize.GetComponent<Text>().text;
        infoFELength.text = fieldFELength.GetComponent<Text>().text;
        infoFEModel.text = fieldFEModel.GetComponent<Text>().text;

        // info model
        infoModelEpoch.text = fieldModelEpoch.GetComponent<Text>().text;
        infoModelLoss.text = fieldModelLoss.GetComponent<Text>().text;
        infoModelAccuracy.text = fieldModelAccuracy.GetComponent<Text>().text + " %";
        infoModelTime.text = fieldModelTime.GetComponent<Text>().text + " s";
    }

    void Update()
    {
        // info data
        infoSensor.text = fieldSensor.GetComponent<Text>().text;
        infoDataSize.text = fieldDataSize.GetComponent<Text>().text;
        infoFELength.text = fieldFELength.GetComponent<Text>().text;
        infoFEModel.text = fieldFEModel.GetComponent<Text>().text;

        // info model
        infoModelEpoch.text = fieldModelEpoch.GetComponent<Text>().text;
        infoModelLoss.text = fieldModelLoss.GetComponent<Text>().text;
        infoModelAccuracy.text = fieldModelAccuracy.GetComponent<Text>().text + " %";
        infoModelTime.text = fieldModelTime.GetComponent<Text>().text + " s";
    }
}

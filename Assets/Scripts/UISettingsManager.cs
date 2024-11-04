using UnityEngine;
using UnityEngine.UI;

public class UISettingsManager : MonoBehaviour
{
    /******************************
    ** FIELDS
    *******************************/

    // device mac address
    public InputField inputDeviceMacAddr;
    public GameObject fieldDeviceMacAddr;
    public GameObject fieldDeviceMacAddrDefault;

    // sensor
    public Dropdown dropSensor;
    public GameObject fieldSensor;

    // data record size
    public Dropdown dropDataSize;
    public GameObject fieldDataSize;

    // feature extraction length
    public Dropdown dropFELength;
    public GameObject fieldFELength;

    // feature extraction model
    public Dropdown dropFEModel;
    public GameObject fieldFEModel;

    // ann model
    public Dropdown dropANNModel;
    public GameObject fieldANNModel;

    // model epoch limit
    public InputField inputEpochLim;
    public GameObject fieldEpochLim;
    public GameObject fieldEpochLimDefault;

    // model loss limit
    public InputField inputLossLim;
    public GameObject fieldLossLim;
    public GameObject fieldLossLimDefault;


    /******************************
    ** METHODS
    *******************************/

    void Start()
    {
        inputDeviceMacAddr.onValueChanged.AddListener(delegate {
            ChangeInputValue(inputDeviceMacAddr, fieldDeviceMacAddr);
        });
        dropSensor.onValueChanged.AddListener(delegate {
            ChangeDropValue(dropSensor, fieldSensor);
        });
        dropDataSize.onValueChanged.AddListener(delegate {
            ChangeDropValue(dropDataSize, fieldDataSize);
        });
        dropFELength.onValueChanged.AddListener(delegate {
            ChangeDropValue(dropFELength, fieldFELength);
        });
        dropFEModel.onValueChanged.AddListener(delegate {
            ChangeDropValue(dropFEModel, fieldFEModel);
        });
        dropANNModel.onValueChanged.AddListener(delegate {
            ChangeDropValue(dropANNModel, fieldANNModel);
        });
        inputEpochLim.onValueChanged.AddListener(delegate {
            ChangeInputValue(inputEpochLim, fieldEpochLim);
        });
        inputLossLim.onValueChanged.AddListener(delegate {
            ChangeInputValue(inputLossLim, fieldLossLim);
        });
    }

    public void DefaultDevice()
    {
        string macAddr = fieldDeviceMacAddrDefault.GetComponent<Text>().text;
        inputDeviceMacAddr.text = macAddr;
    }

    private void ChangeDropValue(Dropdown drop, GameObject field)
    {
        int dropIndex = drop.value;
        string dropVal = drop.options[dropIndex].text;
        field.GetComponent<Text>().text = dropVal;
    }

    private void ChangeInputValue(InputField input, GameObject field)
    {
        string inputVal = input.text;
        field.GetComponent<Text>().text = inputVal;
    }

    public void DefaultEpochLim()
    {
        string epochLim = fieldEpochLimDefault.GetComponent<Text>().text;
        inputEpochLim.text = epochLim;
    }

    public void DefaultLossLim()
    {
        string lossLim = fieldLossLimDefault.GetComponent<Text>().text;
        inputLossLim.text = lossLim;
    }

}

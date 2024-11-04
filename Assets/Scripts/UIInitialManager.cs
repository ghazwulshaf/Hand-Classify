using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInitialManager : MonoBehaviour
{
    // setting device mac address
    private string macAddr;
    public InputField inputMacAddr;
    public Text fieldMacAddr;
    public Button btnMacAddr;

    // setting sensor
    private List<string> sensors;
    public Dropdown dropSensor;
    public Text fieldSensor;

    // setting data electrode
    private List<string> electrodes;
    public Dropdown dropElectrode;
    public Text fieldElectrode;

    // setting data position
    private List<string> positions;
    public Dropdown dropPosition;
    public Text fieldPosition;

    // setting data group
    private List<string> groups;
    public Dropdown dropGroup;
    public Text fieldGroup;

    // setting feature extraction length
    private List<int> feLengths;
    public Dropdown dropFELength;
    public Text fieldFELength;

    // setting feature extraction model
    private List<string> feModels;
    public Dropdown dropFEModel;
    public Text fieldFEModel;

    // setting ann model
    private List<string> annModels;
    public Dropdown dropAnnModel;
    public Text fieldAnnModel;

    // setting epoch limit
    private int epochLim;
    public InputField inputEpochLim;
    public Text fieldEpochLim;
    public Button btnEpochLim;

    // setting loss limit
    private double lossLim;
    public InputField inputLossLim;
    public Text fieldLossLim;
    public Button btnLossLim;

    // record data model
    private List<string> dataModels;
    public Dropdown dropDataModel;
    public Text fieldDataModel;

    void Start()
    {
        // setting device mac address
        macAddr = "78:21:84:7D:80:CA";
        inputMacAddr.text = macAddr;
        fieldMacAddr.text = macAddr;
        GenerateOnClick(btnMacAddr, inputMacAddr, macAddr);

        // setting sensor
        sensors = new List<string>
        {
            "AD8221",
            "AD8232"
        };
        InitDropdown(dropSensor, sensors, fieldSensor);
        GenerateOnChange(dropSensor, fieldSensor);

        // setting data electrode
        electrodes = new List<string>
        {
            "A",
            "B",
            "C"
        };
        InitDropdown(dropElectrode, electrodes, fieldElectrode);
        GenerateOnChange(dropElectrode, fieldElectrode);

        // setting data position
        positions = new List<string>
        {
            "Stand nF",
            "Stand wF",
            "Sit"
        };
        InitDropdown(dropPosition, positions, fieldPosition);
        GenerateOnChange(dropPosition, fieldPosition);

        // setting data group
        groups = new List<string>
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"
        };
        InitDropdown(dropGroup, groups, fieldGroup);
        GenerateOnChange(dropGroup, fieldGroup);

        // setting feature extraction length
        feLengths = new List<int>
        {
            20,
            40,
            50,
            100,
            150,
            200,
            250,
            300
        };
        InitDropdown(dropFELength, feLengths, fieldFELength);
        GenerateOnChange(dropFELength, fieldFELength);

        // setting feature extraction model
        feModels = new List<string>
        {
            "Riilo",
            "Ahsan",
            "Jayaweera"
        };
        InitDropdown(dropFEModel, feModels, fieldFEModel);
        GenerateOnChange(dropFEModel, fieldFEModel);
        
        // setting feature extraction model
        annModels = new List<string>
        {
            "1 Hidden Layer",
            "2 Hidden Layer",
            "3 Hidden Layer"
        };
        InitDropdown(dropAnnModel, annModels, fieldAnnModel);
        GenerateOnChange(dropAnnModel, fieldAnnModel);

        // setting epoch limit
        epochLim = 10000;
        inputEpochLim.text = epochLim.ToString();
        fieldEpochLim.text = epochLim.ToString();
        GenerateOnClick(btnEpochLim, inputEpochLim, epochLim.ToString());

        // setting loss limit
        lossLim = 0.001;
        inputLossLim.text = lossLim.ToString();
        fieldLossLim.text = lossLim.ToString();
        GenerateOnClick(btnLossLim, inputLossLim, lossLim.ToString());

        // record data model
        dataModels = new List<string>
        {
            "Finger_Index",
            "Finger_Middle",
            "Finger_Little",
            "Finger_Thumb",
            "Hand_Close",
            "Hand_Open",
            "Hand_Peace",
            "Hand_Pistol"
        };
        InitDropdown(dropDataModel, dataModels, fieldDataModel);
        GenerateOnChange(dropDataModel, fieldDataModel);
    }

    private void InitDropdown(Dropdown drop, List<string> options, Text field)
    {
        drop.options.Clear();
        foreach (string option in options)
        {
            var newOption = new Dropdown.OptionData
            {
                text = option
            };
            drop.options.Add(newOption);
        }
        drop.value = 0;
        field.text = options[0];
    }

    private void InitDropdown(Dropdown drop, List<int> options, Text field)
    {
        drop.options.Clear();
        foreach (int option in options)
        {
            var newOption = new Dropdown.OptionData
            {
                text = option.ToString()
            };
            drop.options.Add(newOption);
        }
        drop.value = 0;
        field.text = options[0].ToString();
    }

    private void GenerateOnChange(Dropdown drop, Text field)
    {
        drop.onValueChanged.AddListener(delegate
        {
            int dropIndex = drop.value;
            string dropVal = drop.options[dropIndex].text;
            field.text = dropVal;
        });
    }

    private void GenerateOnChange(InputField input, Text field)
    {
        input.onValueChanged.AddListener(delegate
        {
            string inputVal = input.text;
            field.text = inputVal;
        });
    }

    private void GenerateOnClick(Button button, InputField input, string value)
    {
        button.onClick.AddListener(delegate
        {
            input.text = value;
        });
    }

}

using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIRecordManager : MonoBehaviour
{
    /******************************
    ** FIELDS
    *******************************/

    public Dropdown dropDataModel;
    public GameObject fieldDataModel;
    public GameObject recordProgress;
    public GameObject recrodProgressData;
    private NumberFormatInfo provider;

    private Image recordProgressBar;


    /******************************
    ** METHODS
    *******************************/

    void Start()
    {
        provider = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };
    
        recordProgressBar = recordProgress.GetComponent<Image>();
        recordProgressBar.fillAmount = 0;
    }

    void Update()
    {
        dropDataModel.onValueChanged.AddListener(delegate {
            ChangeDropValue(dropDataModel, fieldDataModel);
        });

        string recordProgressVal = recrodProgressData.GetComponent<Text>().text;
        recordProgressBar.fillAmount = float.Parse(recordProgressVal, provider);
    }

    private void ChangeDropValue(Dropdown drop, GameObject field)
    {
        int dropIndex = drop.value;
        string dropVal = drop.options[dropIndex].text;
        field.GetComponent<Text>().text = dropVal;
    }

}

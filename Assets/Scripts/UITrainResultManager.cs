using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UITrainResultManager : MonoBehaviour
{
    // parent object
    public GameObject timestampParent;
    public GameObject electrodeParent;
    public GameObject positionParent;
    public GameObject groupParent;
    public GameObject feLengthParent;
    public GameObject lossParent;
    public GameObject accuracyParent;
    public GameObject timeParent;

    // text style
    public Font font;

    // data
    private string pathTrainResult;

    // method
    private CSVHandler CSVHandler;
    
    private NumberFormatInfo provider;

    // Start is called before the first frame update
    void Start()
    {
        // method
        CSVHandler = new CSVHandler();
        provider = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };

        // data path
        pathTrainResult = Application.persistentDataPath + "/EMG_TrainResult.csv";

        // clear data table
        timestampParent.transform.DetachChildren();
        electrodeParent.transform.DetachChildren();
        positionParent.transform.DetachChildren();
        groupParent.transform.DetachChildren();
        feLengthParent.transform.DetachChildren();
        lossParent.transform.DetachChildren();
        accuracyParent.transform.DetachChildren();
        timeParent.transform.DetachChildren();
    }

    public void ReloadDataTable()
    {
        // get data
        var datasList = CSVHandler.ReadCSV(pathTrainResult, true);
        
        // update table
        int num = 1;
        timestampParent.transform.DetachChildren();
        electrodeParent.transform.DetachChildren();
        positionParent.transform.DetachChildren();
        groupParent.transform.DetachChildren();
        feLengthParent.transform.DetachChildren();
        lossParent.transform.DetachChildren();
        accuracyParent.transform.DetachChildren();
        timeParent.transform.DetachChildren();
        foreach (var list in datasList)
        {
            var datas = list.Split(',');

            GenerateChildText(timestampParent, datas[0], TextAnchor.MiddleLeft, $"Data {num}");
            GenerateChildText(electrodeParent, datas[2], TextAnchor.MiddleCenter, $"Data {num}");
            GenerateChildText(positionParent, datas[3], TextAnchor.MiddleCenter, $"Data {num}");
            GenerateChildText(groupParent, datas[4], TextAnchor.MiddleCenter, $"Data {num}");
            GenerateChildText(feLengthParent, datas[5], TextAnchor.MiddleCenter, $"Data {num}");
            GenerateChildText(lossParent, datas[9], TextAnchor.MiddleCenter, $"Data {num}");
            GenerateChildText(accuracyParent, datas[10], TextAnchor.MiddleCenter, $"Data {num}");
            GenerateChildText(timeParent, datas[11], TextAnchor.MiddleCenter, $"Data {num}");

            num++;
        }
    }

    private void GenerateChildText(GameObject parent, string text, TextAnchor textAnchor, string name = null)
    {
        GameObject childText = new();
        childText.AddComponent<Text>();
        childText.GetComponent<Text>().text = text;
        childText.GetComponent<Text>().font = font;
        childText.GetComponent<Text>().fontSize = 24;
        childText.GetComponent<Text>().alignment = textAnchor;
        childText.GetComponent<Text>().color = Color.black;
        childText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);

        GameObject child = Instantiate(childText, parent.transform);
        child.name = name ?? "Text";
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModelHandlingProgram : MonoBehaviour
{
    /******************************
    ** FIELDS
    *******************************/

    // UI
    public Text display;
    public TMP_Dropdown dropSource;
    public TMP_Dropdown dropLength;
    public TMP_Dropdown dropLayer;
    public Button btnTrain;

    // method
    private CSVHandler CSVHandler;

    // model
    private ANNClassification EMG;
    private ANNClassification1Layer EMG1Layer;

    // global field
    private NumberFormatInfo provider;


    /******************************
    ** METHODS
    *******************************/

    void Start()
    {
        // method
        CSVHandler = new CSVHandler();

        // global field
        provider = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };
    }

    public async void Train()
    {
        // start text
        display.text = "Wait for training...";
        btnTrain.enabled = false;

        // get source
        string source = "";
        int sourceIndex = dropSource.value;
        string sourceVal = dropSource.options[sourceIndex].text;
        switch (sourceVal)
        {
            case "Raw Data":
                source = "raw";
                break;
            case "Filtered Data":
                source = "filt";
                break;
        }

        Debug.Log("Pass 1");

        // get feature length dropdown value
        int lengthIndex = dropLength.value;
        string lengthVal = dropLength.options[lengthIndex].text;

        // get layer dropdown value
        int layerIndex = dropLayer.value;
        string layerVal = dropLayer.options[layerIndex].text;

        Debug.Log("Pass 2");

        try
        {
            Debug.Log("Pass 2.1");
            // read csv
            string pathTrain = Application.persistentDataPath + "/EMG_" + source + "_" + lengthVal + "_train.csv";
            string pathTest = Application.persistentDataPath + "/EMG_" + source + "_" + lengthVal + "_test.csv";

            Debug.Log("Pass 2.2");
            var dataTrain = CSVHandler.ReadCSV(pathTrain, true);
            var dataTest = CSVHandler.ReadCSV(pathTest, true);
            Debug.Log("Pass 2.5");
            int featureCount = dataTrain[0].Split(',').Length - 1;

            // shuffle data
            dataTrain = FisherYates(dataTrain);

            Debug.Log("Pass 3");

            // preprocess data train
            var inputTrain = new double[dataTrain.Count, featureCount];
            var labelTrain = new string[dataTrain.Count];
            for (int i = 0; i < dataTrain.Count; i++)
            {
                var data = dataTrain[i].Split(',');
                for (int j = 0; j < data.Length; j++)
                {
                    if (j != data.Length - 1)
                        inputTrain[i,j] = double.Parse(data[j], provider);
                    else
                        labelTrain[i] = data[j];
                }
            }

            // preprocess data test
            var inputTest = new double[dataTest.Count, featureCount];
            var labelTest = new string[dataTest.Count];
            for (int i = 0; i < dataTest.Count; i++)
            {
                var data = dataTest[i].Split(',');
                for (int j = 0; j < data.Length; j++)
                {
                    if (j != data.Length - 1)
                        inputTest[i,j] = double.Parse(data[j], provider);
                    else
                        labelTest[i] = data[j];
                }
            }

            Debug.Log("Pass 4");

            // init model
            EMG = new ANNClassification(inputTrain, labelTrain, inputTest, labelTest, 10, 10);
            EMG1Layer = new ANNClassification1Layer(6, 8, 10);

            // param path
            string pathWH1 = Application.persistentDataPath + "/EMG_WH1.csv";
            string pathWH2 = Application.persistentDataPath + "/EMG_WH2.csv";
            string pathWOut = Application.persistentDataPath + "/EMG_WOut.csv";
            string pathBH1 = Application.persistentDataPath + "/EMG_BH1.csv";
            string pathBH2 = Application.persistentDataPath + "/EMG_BH2.csv";
            string pathBOut = Application.persistentDataPath + "/EMG_BOut.csv";

            // nn param
            int epoch = 10000;
            double loss = 0;
            double accuracy = 0;
            double time = 0;

            Debug.Log("Pass 5");

            // check layer
            switch (layerVal)
            {
                case "1":

                    // train model
                    await EMG1Layer.Train(inputTrain, labelTrain, epoch, 0.001);

                    // validate model
                    await EMG1Layer.Validate(inputTest, labelTest);

                    Debug.Log("Pass 6");

                    // store param
                    CSVHandler.WriteCSV(pathWH1, EMG1Layer.wH, true);
                    CSVHandler.WriteCSV(pathWOut, EMG1Layer.wOut, true);
                    CSVHandler.WriteCSV(pathBH1, EMG1Layer.bH, true);
                    CSVHandler.WriteCSV(pathBOut, EMG1Layer.bOut, true);

                    Debug.Log("Pass 7");

                    // get nn param
                    epoch = EMG1Layer.epoch;
                    loss = EMG1Layer.loss;
                    accuracy = EMG1Layer.accuracy;
                    time = EMG1Layer.time;

                    Debug.Log("Pass 8");

                    // break
                    break;

                case "2":
                    
                    // train model
                    await EMG.Train(epoch, 0.001);

                    // store param
                    CSVHandler.WriteCSV(pathWH1, EMG.wH1, true);
                    CSVHandler.WriteCSV(pathWH2, EMG.wH2, true);
                    CSVHandler.WriteCSV(pathWOut, EMG.wOut, true);
                    CSVHandler.WriteCSV(pathBH1, EMG.bH1, true);
                    CSVHandler.WriteCSV(pathBH2, EMG.bH2, true);
                    CSVHandler.WriteCSV(pathBOut, EMG.bOut, true);

                    // get nn param
                    epoch = EMG.epoch;
                    loss = EMG.loss;
                    accuracy = EMG.accuracy;
                    time = EMG.time;

                    // break
                    break;
            }

            // finish text
            display.text = "Accuracy: " + accuracy.ToString() + "% | Time: " + time + " s";

            // init model
            var modelPath = Application.persistentDataPath + "/EMG_models.csv";
            var modelHeader = new string[]
            {
                new("Timestamp"),
                new("Data Source"),
                new("FE Range"),
                new("Hidden Layer"),
                new("Epoch"),
                new("Loss"),
                new("Accuracy (%)"),
                new("Time Train (s)")
            };
            var modelData = new string[]
            {
                new(System.DateTime.Now.ToString()),
                new(source),
                new(lengthVal),
                new(layerVal),
                new(epoch.ToString(provider)),
                new(loss.ToString(provider)),
                new(accuracy.ToString(provider)),
                new(time.ToString(provider))
            };


            // store data
            bool header = false;
            if (!File.Exists(modelPath))
                header = true;
            CSVHandler.WriteCSV(modelPath, modelData, false, header, modelHeader);
        }
        catch (System.Exception ex)
        {
            display.text = ex.Message;
        }
        finally
        {
            btnTrain.enabled = true;
        }

    }


    /******************************
    ** ADD. METHODS
    *******************************/

    private List<string> FisherYates(List<string> _datas)
    {
        var n = _datas.Count;
        var rand = new System.Random();

        for (int i = n - 1; i > 0; i--)
        {
            var k = rand.Next(i + 1);
            (_datas[i], _datas[k]) = (_datas[k], _datas[i]);
        }

        return _datas;
    }

}

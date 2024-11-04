using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DataHandlingProgram : MonoBehaviour
{
    /******************************
    ** FIELDS
    *******************************/

    // UI
    public Text display;

    // method
    private CSVHandler CSVHandler;
    private FeatureExtraction FeatureExtraction;

    // field
    private NumberFormatInfo provider;


    /******************************
    ** METHODS
    *******************************/

    void Start()
    {
        // method
        CSVHandler = new CSVHandler();
        FeatureExtraction = new FeatureExtraction();

        // field
        provider = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };
    }

    public void ProcessData()
    {
        // start text
        display.text = "Wait for processing data...";

        // label
        var labels = new List<string>()
        {
            new("finger_index"),
            new("finger_little"),
            new("finger_middle"),
            new("finger_thumb"),
            new("hand_close"),
            new("hand_open"),
            new("hand_Peace"),
            new("hand_pistol")
        };

        // header
        var headers = new List<string>()
        {
            new("ME"),
            new("RMS"),
            // new("WA"),
            new("SSC"),
            new("SSI"),
            new("VAR"),
            new("WL"),
            new("Label")
        };

        // source path
        string pathFingerIndex = Application.persistentDataPath + "/FingerIndex.csv";
        string pathFingerLittle = Application.persistentDataPath + "/FingerLittle.csv";
        string pathFingerMiddle = Application.persistentDataPath + "/FingerMiddle.csv";
        string pathFingerThumb = Application.persistentDataPath + "/FingerThumb.csv";
        string pathHandClose = Application.persistentDataPath + "/HandClose.csv";
        string pathHandOpen = Application.persistentDataPath + "/HandOpen.csv";
        string pathHandPeace = Application.persistentDataPath + "/HandPeace.csv";
        string pathHandPistol = Application.persistentDataPath + "/HandPistol.csv";

        try
        {
            // read data source
            var datasFingerIndex = CSVHandler.ReadCSV(pathFingerIndex);
            var datasFingerLittle = CSVHandler.ReadCSV(pathFingerLittle);
            var datasFingerMiddle = CSVHandler.ReadCSV(pathFingerMiddle);
            var datasFingerThumb = CSVHandler.ReadCSV(pathFingerThumb);
            var datasHandClose = CSVHandler.ReadCSV(pathHandClose);
            var datasHandOpen = CSVHandler.ReadCSV(pathHandOpen);
            var datasHandPeace = CSVHandler.ReadCSV(pathHandPeace);
            var datasHandPistol = CSVHandler.ReadCSV(pathHandPistol);

            // set param
            bool newData = true;
            int feRange = 100;
            int dataCount = datasFingerIndex.Count;
            int remRange = dataCount % feRange;
            int remIndex = dataCount - remRange;
            bool rem = remRange != 0;

            // remove some data
            if (rem)
            {
                datasFingerIndex.RemoveRange(remIndex, remRange);
                datasFingerLittle.RemoveRange(remIndex, remRange);
                datasFingerMiddle.RemoveRange(remIndex, remRange);
                datasFingerThumb.RemoveRange(remIndex, remRange);
                datasHandClose.RemoveRange(remIndex, remRange);
                datasHandOpen.RemoveRange(remIndex, remRange);
                datasHandPeace.RemoveRange(remIndex, remRange);
                datasHandPistol.RemoveRange(remIndex, remRange);
            }

            // convert to double
            var doubleFingerIndex = ConvertToDouble(datasFingerIndex);
            var doubleFingerLittle = ConvertToDouble(datasFingerLittle);
            var doubleFingerMiddle = ConvertToDouble(datasFingerMiddle);
            var doubleFingerThumb = ConvertToDouble(datasFingerThumb);
            var doubleHandClose = ConvertToDouble(datasHandClose);
            var doubleHandOpen = ConvertToDouble(datasHandOpen);
            var doubleHandPeace = ConvertToDouble(datasHandPeace);
            var doubleHandPistol = ConvertToDouble(datasHandPistol);

            // feature extraction: Mean
            var meFingerIndex = FeatureExtraction.ME(doubleFingerIndex, feRange);
            var meFingerLittle = FeatureExtraction.ME(doubleFingerLittle, feRange);
            var meFingerMiddle = FeatureExtraction.ME(doubleFingerMiddle, feRange);
            var meFingerThumb = FeatureExtraction.ME(doubleFingerThumb, feRange);
            var meHandClose = FeatureExtraction.ME(doubleHandClose, feRange);
            var meHandOpen = FeatureExtraction.ME(doubleHandOpen, feRange);
            var meHandPeace = FeatureExtraction.ME(doubleHandPeace, feRange);
            var meHandPistol = FeatureExtraction.ME(doubleHandPistol, feRange);

            // feature extraction: Root Mean Square
            var rmsFingerIndex = FeatureExtraction.RMS(doubleFingerIndex, feRange);
            var rmsFingerLittle = FeatureExtraction.RMS(doubleFingerLittle, feRange);
            var rmsFingerMiddle = FeatureExtraction.RMS(doubleFingerMiddle, feRange);
            var rmsFingerThumb = FeatureExtraction.RMS(doubleFingerThumb, feRange);
            var rmsHandClose = FeatureExtraction.RMS(doubleHandClose, feRange);
            var rmsHandOpen = FeatureExtraction.RMS(doubleHandOpen, feRange);
            var rmsHandPeace = FeatureExtraction.RMS(doubleHandPeace, feRange);
            var rmsHandPistol = FeatureExtraction.RMS(doubleHandPistol, feRange);

            // feature extraction: Willison Amplitude

            // feature extraction: Slope Sign Change
            var sscFingerIndex = FeatureExtraction.SSC(doubleFingerIndex, feRange);
            var sscFingerLittle = FeatureExtraction.SSC(doubleFingerLittle, feRange);
            var sscFingerMiddle = FeatureExtraction.SSC(doubleFingerMiddle, feRange);
            var sscFingerThumb = FeatureExtraction.SSC(doubleFingerThumb, feRange);
            var sscHandClose = FeatureExtraction.SSC(doubleHandClose, feRange);
            var sscHandOpen = FeatureExtraction.SSC(doubleHandOpen, feRange);
            var sscHandPeace = FeatureExtraction.SSC(doubleHandPeace, feRange);
            var sscHandPistol = FeatureExtraction.SSC(doubleHandPistol, feRange);

            // feature extraction: Simple Square Integral
            var ssiFingerIndex = FeatureExtraction.SSI(doubleFingerIndex, feRange);
            var ssiFingerLittle = FeatureExtraction.SSI(doubleFingerLittle, feRange);
            var ssiFingerMiddle = FeatureExtraction.SSI(doubleFingerMiddle, feRange);
            var ssiFingerThumb = FeatureExtraction.SSI(doubleFingerThumb, feRange);
            var ssiHandClose = FeatureExtraction.SSI(doubleHandClose, feRange);
            var ssiHandOpen = FeatureExtraction.SSI(doubleHandOpen, feRange);
            var ssiHandPeace = FeatureExtraction.SSI(doubleHandPeace, feRange);
            var ssiHandPistol = FeatureExtraction.SSI(doubleHandPistol, feRange);

            // feature extraction: Variace
            var varFingerIndex = FeatureExtraction.VAR(doubleFingerIndex, feRange);
            var varFingerLittle = FeatureExtraction.VAR(doubleFingerLittle, feRange);
            var varFingerMiddle = FeatureExtraction.VAR(doubleFingerMiddle, feRange);
            var varFingerThumb = FeatureExtraction.VAR(doubleFingerThumb, feRange);
            var varHandClose = FeatureExtraction.VAR(doubleHandClose, feRange);
            var varHandOpen = FeatureExtraction.VAR(doubleHandOpen, feRange);
            var varHandPeace = FeatureExtraction.VAR(doubleHandPeace, feRange);
            var varHandPistol = FeatureExtraction.VAR(doubleHandPistol, feRange);

            // feature extraction: Waveform Length
            var wlFingerIndex = FeatureExtraction.WL(doubleFingerIndex, feRange);
            var wlFingerLittle = FeatureExtraction.WL(doubleFingerLittle, feRange);
            var wlFingerMiddle = FeatureExtraction.WL(doubleFingerMiddle, feRange);
            var wlFingerThumb = FeatureExtraction.WL(doubleFingerThumb, feRange);
            var wlHandClose = FeatureExtraction.WL(doubleHandClose, feRange);
            var wlHandOpen = FeatureExtraction.WL(doubleHandOpen, feRange);
            var wlHandPeace = FeatureExtraction.WL(doubleHandPeace, feRange);
            var wlHandPistol = FeatureExtraction.WL(doubleHandPistol, feRange);

            // init train num
            int feSize = meFingerIndex.Count;
            int testNum = feSize * 30 / 100;
            int trainNum = feSize - testNum;
            int feCount = 7;

            // init train array
            var trainFingerIndex = new string[trainNum, feCount];
            var trainFingerLittle = new string[trainNum, feCount];
            var trainFingerMiddle = new string[trainNum, feCount];
            var trainFingerThumb = new string[trainNum, feCount];
            var trainHandClose = new string[trainNum, feCount];
            var trainHandOpen = new string[trainNum, feCount];
            var trainHandPeace = new string[trainNum, feCount];
            var trainHandPistol = new string[trainNum, feCount];

            // init test array
            var testFingerIndex = new string[testNum, feCount];
            var testFingerLittle = new string[testNum, feCount];
            var testFingerMiddle = new string[testNum, feCount];
            var testFingerThumb = new string[testNum, feCount];
            var testHandClose = new string[testNum, feCount];
            var testHandOpen = new string[testNum, feCount];
            var testHandPeace = new string[testNum, feCount];
            var testHandPistol = new string[testNum, feCount];

            // wrap datas
            int trainIter = 0;
            int testIter = 0;
            for (int i = 0; i < feSize; i++)
            {
                if (i < trainNum)
                {
                    // finger index
                    trainFingerIndex[trainIter,0] = meFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,1] = rmsFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,2] = sscFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,3] = ssiFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,4] = varFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,5] = wlFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,6] = labels[0];

                    // finger little
                    trainFingerLittle[trainIter,0] = meFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,1] = rmsFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,2] = sscFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,3] = ssiFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,4] = varFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,5] = wlFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,6] = labels[1];

                    // finger middle
                    trainFingerMiddle[trainIter,0] = meFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,1] = rmsFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,2] = sscFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,3] = ssiFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,4] = varFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,5] = wlFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,6] = labels[2];

                    // finger thumb
                    trainFingerThumb[trainIter,0] = meFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,1] = rmsFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,2] = sscFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,3] = ssiFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,4] = varFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,5] = wlFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,6] = labels[3];

                    // hand close
                    trainHandClose[trainIter,0] = meHandClose[i].ToString(provider);
                    trainHandClose[trainIter,1] = rmsHandClose[i].ToString(provider);
                    trainHandClose[trainIter,2] = sscHandClose[i].ToString(provider);
                    trainHandClose[trainIter,3] = ssiHandClose[i].ToString(provider);
                    trainHandClose[trainIter,4] = varHandClose[i].ToString(provider);
                    trainHandClose[trainIter,5] = wlHandClose[i].ToString(provider);
                    trainHandClose[trainIter,6] = labels[4];

                    // hand open
                    trainHandOpen[trainIter,0] = meHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,1] = rmsHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,2] = sscHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,3] = ssiHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,4] = varHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,5] = wlHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,6] = labels[5];

                    // hand Peace
                    trainHandPeace[trainIter,0] = meHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,1] = rmsHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,2] = sscHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,3] = ssiHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,4] = varHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,5] = wlHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,6] = labels[6];

                    // hand pistol
                    trainHandPistol[trainIter,0] = meHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,1] = rmsHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,2] = sscHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,3] = ssiHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,4] = varHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,5] = wlHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,6] = labels[7];

                    // increase iter
                    trainIter++;
                }
                else
                {
                    // finger index
                    testFingerIndex[testIter,0] = meFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,1] = rmsFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,2] = sscFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,3] = ssiFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,4] = varFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,5] = wlFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,6] = labels[0];

                    // finger little
                    testFingerLittle[testIter,0] = meFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,1] = rmsFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,2] = sscFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,3] = ssiFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,4] = varFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,5] = wlFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,6] = labels[1];

                    // finger middle
                    testFingerMiddle[testIter,0] = meFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,1] = rmsFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,2] = sscFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,3] = ssiFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,4] = varFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,5] = wlFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,6] = labels[2];

                    // finger thumb
                    testFingerThumb[testIter,0] = meFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,1] = rmsFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,2] = sscFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,3] = ssiFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,4] = varFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,5] = wlFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,6] = labels[3];

                    // hand close
                    testHandClose[testIter,0] = meHandClose[i].ToString(provider);
                    testHandClose[testIter,1] = rmsHandClose[i].ToString(provider);
                    testHandClose[testIter,2] = sscHandClose[i].ToString(provider);
                    testHandClose[testIter,3] = ssiHandClose[i].ToString(provider);
                    testHandClose[testIter,4] = varHandClose[i].ToString(provider);
                    testHandClose[testIter,5] = wlHandClose[i].ToString(provider);
                    testHandClose[testIter,6] = labels[4];

                    // hand open
                    testHandOpen[testIter,0] = meHandOpen[i].ToString(provider);
                    testHandOpen[testIter,1] = rmsHandOpen[i].ToString(provider);
                    testHandOpen[testIter,2] = sscHandOpen[i].ToString(provider);
                    testHandOpen[testIter,3] = ssiHandOpen[i].ToString(provider);
                    testHandOpen[testIter,4] = varHandOpen[i].ToString(provider);
                    testHandOpen[testIter,5] = wlHandOpen[i].ToString(provider);
                    testHandOpen[testIter,6] = labels[5];

                    // hand Peace
                    testHandPeace[testIter,0] = meHandPeace[i].ToString(provider);
                    testHandPeace[testIter,1] = rmsHandPeace[i].ToString(provider);
                    testHandPeace[testIter,2] = sscHandPeace[i].ToString(provider);
                    testHandPeace[testIter,3] = ssiHandPeace[i].ToString(provider);
                    testHandPeace[testIter,4] = varHandPeace[i].ToString(provider);
                    testHandPeace[testIter,5] = wlHandPeace[i].ToString(provider);
                    testHandPeace[testIter,6] = labels[6];

                    // hand pistol
                    testHandPistol[testIter,0] = meHandPistol[i].ToString(provider);
                    testHandPistol[testIter,1] = rmsHandPistol[i].ToString(provider);
                    testHandPistol[testIter,2] = sscHandPistol[i].ToString(provider);
                    testHandPistol[testIter,3] = ssiHandPistol[i].ToString(provider);
                    testHandPistol[testIter,4] = varHandPistol[i].ToString(provider);
                    testHandPistol[testIter,5] = wlHandPistol[i].ToString(provider);
                    testHandPistol[testIter,6] = labels[7];

                    // increase iter
                    testIter++;
                }
            }

            // init path
            string trainPath = Application.persistentDataPath + "/EMG_train.csv";
            string testPath = Application.persistentDataPath + "/EMG_test.csv";
            if (newData)
            {
                if (File.Exists(trainPath)) File.Delete(trainPath);
                if (File.Exists(testPath)) File.Delete(testPath);
            }

            // store data train
            CSVHandler.WriteCSV(trainPath, trainFingerIndex, false, true, headers.ToArray());
            CSVHandler.WriteCSV(trainPath, trainFingerLittle, false);
            CSVHandler.WriteCSV(trainPath, trainFingerMiddle, false);
            CSVHandler.WriteCSV(trainPath, trainFingerThumb, false);
            CSVHandler.WriteCSV(trainPath, trainHandClose, false);
            CSVHandler.WriteCSV(trainPath, trainHandOpen, false);
            CSVHandler.WriteCSV(trainPath, trainHandPeace, false);
            CSVHandler.WriteCSV(trainPath, trainHandPistol, false);

            // store data test
            CSVHandler.WriteCSV(testPath, testFingerIndex, false, true, headers.ToArray());
            CSVHandler.WriteCSV(testPath, testFingerLittle, false);
            CSVHandler.WriteCSV(testPath, testFingerMiddle, false);
            CSVHandler.WriteCSV(testPath, testFingerThumb, false);
            CSVHandler.WriteCSV(testPath, testHandClose, false);
            CSVHandler.WriteCSV(testPath, testHandOpen, false);
            CSVHandler.WriteCSV(testPath, testHandPeace, false);
            CSVHandler.WriteCSV(testPath, testHandPistol, false);

            // complete text
            display.text = "Data processing completed";
        }
        catch (System.Exception ex)
        {
            display.text = ex.Message;
        }
    }


    /******************************
    ** ADD. METHODS
    *******************************/

    private List<double> ConvertToDouble(List<string> datas)
    {
        var newDatas = new List<double>();
        foreach (var data in datas)
        {
            newDatas.Add(double.Parse(data));
        }
        return newDatas;
    }

}

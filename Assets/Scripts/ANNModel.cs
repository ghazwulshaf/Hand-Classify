using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ANNModel : MonoBehaviour
{
    /******************************
    ** FIELDS
    *******************************/

    // UI
    public Text display;
    public Text modelLabel;
    public Image trainProgressBar;
    public InputField dataReceived;

    // global field
    public Text fieldSensor;
    public Text fieldElectrode;
    public Text fieldPosition;
    public Text fieldGroup;
    public Text fieldFELength;
    public Text fieldFEModel;
    public Text fieldANNModel;
    public Text fieldEpoch;
    public Text fieldLoss;
    public Text fieldAccuracy;
    public Text fieldTime;

    // field
    private string dataSensor;
    private string dataElectrode;
    private string dataPosition;
    private string dataGroup;
    private string modelFELength;
    private string modelFEModel;
    private string modelANNModel;
    private string modelLabelVal;
    private int feLength;

    // path
    private string pathFingerIndex;
    // private string pathFingerLittle;
    // private string pathFingerMiddle;
    private string pathFingerThumb;
    private string pathHandClose;
    private string pathHandOpen;
    // private string pathHandPeace;
    // private string pathHandPistol;

    // other path
    private string pathTrain;
    private string pathTest;
    private string pathWH;
    private string pathWOut;
    private string pathBH;
    private string pathBOut;
    private string pathTrainResult;

    // private field
    private List<double> datas;
    private NumberFormatInfo provider;

    // model
    private ANNClassification1Layer EMG;
    private int meNum;
    private int rmsNum;
    private int sscNum;
    private int ssiNum;
    private int varNum;
    private int wlNum;

    // methods
    private CSVHandler CSVHandler;
    private FeatureExtraction FeatureExtraction;


    /******************************
    ** METHODS
    *******************************/

    void Start()
    {
        // init field
        datas = new List<double>();
        trainProgressBar.fillAmount = 0;
        provider = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };

        modelLabelVal = "";

        // init model
        EMG = new ANNClassification1Layer(_featureNum: 6, _labelNum: 4, _hiddenNode: 12);
        meNum = 3;
        rmsNum = 2;
        sscNum = 0;
        ssiNum = 6;
        varNum = 6;
        wlNum = 4;
        
        // init global field
        fieldEpoch.text = EMG.epoch.ToString();
        fieldLoss.text = EMG.loss.ToString();
        fieldAccuracy.text = EMG.accuracy.ToString();
        fieldTime.text = EMG.time.ToString();

        // init methods
        CSVHandler = new CSVHandler();
        FeatureExtraction = new FeatureExtraction();
    }

    void Update()
    {
        // display info model
        fieldEpoch.text = EMG.epoch.ToString();
        fieldLoss.text = EMG.loss.ToString();
        fieldAccuracy.text = EMG.accuracy.ToString();
        fieldTime.text = EMG.time.ToString();

        // display model label
        modelLabel.text = modelLabelVal;

        // update train progress bar
        float trainProgressFill = (float) EMG.epoch / (float) 10000.0;
        trainProgressBar.fillAmount = trainProgressFill;

        // field
        dataSensor = fieldSensor.text;
        dataElectrode = fieldElectrode.text;
        dataPosition = fieldPosition.text;
        dataGroup = fieldGroup.text;
        modelFELength = fieldFELength.text;
        modelFEModel = fieldFEModel.text;
        modelANNModel = fieldANNModel.text;
        feLength = int.Parse(modelFELength);

        // source path
        pathFingerIndex = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_Finger_Index.csv";
        // pathFingerLittle = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_FingerLittle.csv";
        // pathFingerMiddle = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_FingerMiddle.csv";
        pathFingerThumb = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_Finger_Thumb.csv";
        pathHandClose = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_Hand_Close.csv";
        pathHandOpen = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_Hand_Open.csv";
        // pathHandPeace = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_HandPeace.csv";
        // pathHandPistol = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_HandPistol.csv";

        // model path
        pathTrain = Application.persistentDataPath + "/EMG_Train_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_" + feLength + ".csv";
        pathTest = Application.persistentDataPath + "/EMG_Test_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_" + feLength + ".csv";

        // param path
        pathWH = Application.persistentDataPath + "/EMG_WH_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_" + feLength + ".csv";
        pathWOut = Application.persistentDataPath + "/EMG_WOut_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_" + feLength + ".csv";
        pathBH = Application.persistentDataPath + "/EMG_BH_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_" + feLength + ".csv";
        pathBOut = Application.persistentDataPath + "/EMG_BOut_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_" + feLength + ".csv";

        // train result path
        pathTrainResult = Application.persistentDataPath + "/EMG_TrainResult.csv";

        // stream data
        modelLabel.text = modelLabelVal;
        if (EMGData.isStreaming)
        {
            try
            {
                dataReceived.text = EMGData.DataStream.Count.ToString();
                modelLabelVal = PredictModel(EMGData.DataStream);
            }
            catch (System.Exception ex)
            {
                display.text += ex.Message + "\n";
            }
        }
    }

    public void DataChanged()
    {
        // wrap data
        double data = double.Parse(dataReceived.text);
        datas.Add(data);

        // set predict
        if (datas.Count == 150)
        {
            // predict data
            modelLabelVal = PredictModel(datas);

            // clear datas
            datas.Clear();
        }
    }

    public async void StartStream()
    {
        await StreamModel();
    }

    private async Task StreamModel()
    {
        await Task.Run(async () =>
        {
            try
            {
                while (EMGData.isStreaming)
                {
                    dataReceived.text = EMGData.DataStream.Count.ToString();
                    modelLabelVal = PredictModel(EMGData.DataStream);
                    await Task.Delay(1);
                }
            }
            catch (System.Exception ex)
            {
                display.text += ex.Message + "\n";
            }
        });
    }

    public void ProcessModel()
    {
        // start text
        display.text += "Wait for processing data...\n";

        // label
        var labels = new List<string>()
        {
            new("finger_index"),
            // new("finger_little"),
            // new("finger_middle"),
            new("finger_thumb"),
            new("hand_close"),
            new("hand_open"),
            // new("hand_Peace"),
            // new("hand_pistol")
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

        try
        {
            // read data source
            var datasFingerIndex = CSVHandler.ReadCSV(pathFingerIndex);
            // var datasFingerLittle = CSVHandler.ReadCSV(pathFingerLittle);
            // var datasFingerMiddle = CSVHandler.ReadCSV(pathFingerMiddle);
            var datasFingerThumb = CSVHandler.ReadCSV(pathFingerThumb);
            var datasHandClose = CSVHandler.ReadCSV(pathHandClose);
            var datasHandOpen = CSVHandler.ReadCSV(pathHandOpen);
            // var datasHandPeace = CSVHandler.ReadCSV(pathHandPeace);
            // var datasHandPistol = CSVHandler.ReadCSV(pathHandPistol);

            // set param
            bool newData = true;
            int dataCount = datasFingerIndex.Count;
            int remRange = dataCount % feLength;
            int remIndex = dataCount - remRange;
            bool rem = remRange != 0;

            // remove some data
            if (rem)
            {
                datasFingerIndex.RemoveRange(remIndex, remRange);
                // datasFingerLittle.RemoveRange(remIndex, remRange);
                // datasFingerMiddle.RemoveRange(remIndex, remRange);
                datasFingerThumb.RemoveRange(remIndex, remRange);
                datasHandClose.RemoveRange(remIndex, remRange);
                datasHandOpen.RemoveRange(remIndex, remRange);
                // datasHandPeace.RemoveRange(remIndex, remRange);
                // datasHandPistol.RemoveRange(remIndex, remRange);
            }

            // convert to double
            var doubleFingerIndex = ConvertToDouble(datasFingerIndex);
            // var doubleFingerLittle = ConvertToDouble(datasFingerLittle);
            // var doubleFingerMiddle = ConvertToDouble(datasFingerMiddle);
            var doubleFingerThumb = ConvertToDouble(datasFingerThumb);
            var doubleHandClose = ConvertToDouble(datasHandClose);
            var doubleHandOpen = ConvertToDouble(datasHandOpen);
            // var doubleHandPeace = ConvertToDouble(datasHandPeace);
            // var doubleHandPistol = ConvertToDouble(datasHandPistol);

            // feature extraction: Mean
            var meFingerIndex = FeatureExtraction.ME(doubleFingerIndex, feLength, num: meNum);
            // var meFingerLittle = FeatureExtraction.ME(doubleFingerLittle, feLength, num: meNum);
            // var meFingerMiddle = FeatureExtraction.ME(doubleFingerMiddle, feLength, num: meNum);
            var meFingerThumb = FeatureExtraction.ME(doubleFingerThumb, feLength, num: meNum);
            var meHandClose = FeatureExtraction.ME(doubleHandClose, feLength, num: meNum);
            var meHandOpen = FeatureExtraction.ME(doubleHandOpen, feLength, num: meNum);
            // var meHandPeace = FeatureExtraction.ME(doubleHandPeace, feLength, num: meNum);
            // var meHandPistol = FeatureExtraction.ME(doubleHandPistol, feLength, num: meNum);

            // feature extraction: Root Mean Square
            var rmsFingerIndex = FeatureExtraction.RMS(doubleFingerIndex, feLength, num: rmsNum);
            // var rmsFingerLittle = FeatureExtraction.RMS(doubleFingerLittle, feLength, num: rmsNum);
            // var rmsFingerMiddle = FeatureExtraction.RMS(doubleFingerMiddle, feLength, num: rmsNum);
            var rmsFingerThumb = FeatureExtraction.RMS(doubleFingerThumb, feLength, num: rmsNum);
            var rmsHandClose = FeatureExtraction.RMS(doubleHandClose, feLength, num: rmsNum);
            var rmsHandOpen = FeatureExtraction.RMS(doubleHandOpen, feLength, num: rmsNum);
            // var rmsHandPeace = FeatureExtraction.RMS(doubleHandPeace, feLength, num: rmsNum);
            // var rmsHandPistol = FeatureExtraction.RMS(doubleHandPistol, feLength, num: rmsNum);

            // feature extraction: Willison Amplitude

            // feature extraction: Slope Sign Change
            var sscFingerIndex = FeatureExtraction.SSC(doubleFingerIndex, feLength, num: sscNum);
            // var sscFingerLittle = FeatureExtraction.SSC(doubleFingerLittle, feLength, num: sscNum);
            // var sscFingerMiddle = FeatureExtraction.SSC(doubleFingerMiddle, feLength, num: sscNum);
            var sscFingerThumb = FeatureExtraction.SSC(doubleFingerThumb, feLength, num: sscNum);
            var sscHandClose = FeatureExtraction.SSC(doubleHandClose, feLength, num: sscNum);
            var sscHandOpen = FeatureExtraction.SSC(doubleHandOpen, feLength, num: sscNum);
            // var sscHandPeace = FeatureExtraction.SSC(doubleHandPeace, feLength, num: sscNum);
            // var sscHandPistol = FeatureExtraction.SSC(doubleHandPistol, feLength, num: sscNum);

            // feature extraction: Simple Square Integral
            var ssiFingerIndex = FeatureExtraction.SSI(doubleFingerIndex, feLength, num: ssiNum);
            // var ssiFingerLittle = FeatureExtraction.SSI(doubleFingerLittle, feLength, num: ssiNum);
            // var ssiFingerMiddle = FeatureExtraction.SSI(doubleFingerMiddle, feLength, num: ssiNum);
            var ssiFingerThumb = FeatureExtraction.SSI(doubleFingerThumb, feLength, num: ssiNum);
            var ssiHandClose = FeatureExtraction.SSI(doubleHandClose, feLength, num: ssiNum);
            var ssiHandOpen = FeatureExtraction.SSI(doubleHandOpen, feLength, num: ssiNum);
            // var ssiHandPeace = FeatureExtraction.SSI(doubleHandPeace, feLength, num: ssiNum);
            // var ssiHandPistol = FeatureExtraction.SSI(doubleHandPistol, feLength, num: ssiNum);

            // feature extraction: Variace
            var varFingerIndex = FeatureExtraction.VAR(doubleFingerIndex, feLength, num: varNum);
            // var varFingerLittle = FeatureExtraction.VAR(doubleFingerLittle, feLength, num: varNum);
            // var varFingerMiddle = FeatureExtraction.VAR(doubleFingerMiddle, feLength, num: varNum);
            var varFingerThumb = FeatureExtraction.VAR(doubleFingerThumb, feLength, num: varNum);
            var varHandClose = FeatureExtraction.VAR(doubleHandClose, feLength, num: varNum);
            var varHandOpen = FeatureExtraction.VAR(doubleHandOpen, feLength, num: varNum);
            // var varHandPeace = FeatureExtraction.VAR(doubleHandPeace, feLength, num: varNum);
            // var varHandPistol = FeatureExtraction.VAR(doubleHandPistol, feLength, num: varNum);

            // feature extraction: Waveform Length
            var wlFingerIndex = FeatureExtraction.WL(doubleFingerIndex, feLength, num: wlNum);
            // var wlFingerLittle = FeatureExtraction.WL(doubleFingerLittle, feLength, num: wlNum);
            // var wlFingerMiddle = FeatureExtraction.WL(doubleFingerMiddle, feLength, num: wlNum);
            var wlFingerThumb = FeatureExtraction.WL(doubleFingerThumb, feLength, num: wlNum);
            var wlHandClose = FeatureExtraction.WL(doubleHandClose, feLength, num: wlNum);
            var wlHandOpen = FeatureExtraction.WL(doubleHandOpen, feLength, num: wlNum);
            // var wlHandPeace = FeatureExtraction.WL(doubleHandPeace, feLength, num: wlNum);
            // var wlHandPistol = FeatureExtraction.WL(doubleHandPistol, feLength, num: wlNum);

            // init train num
            int feSize = meFingerIndex.Count;
            int testNum = feSize * 30 / 100;
            int trainNum = feSize - testNum;
            int feCount = 7;

            // init train array
            var trainFingerIndex = new string[trainNum, feCount];
            // var trainFingerLittle = new string[trainNum, feCount];
            // var trainFingerMiddle = new string[trainNum, feCount];
            var trainFingerThumb = new string[trainNum, feCount];
            var trainHandClose = new string[trainNum, feCount];
            var trainHandOpen = new string[trainNum, feCount];
            // var trainHandPeace = new string[trainNum, feCount];
            // var trainHandPistol = new string[trainNum, feCount];

            // init test array
            var testFingerIndex = new string[testNum, feCount];
            // var testFingerLittle = new string[testNum, feCount];
            // var testFingerMiddle = new string[testNum, feCount];
            var testFingerThumb = new string[testNum, feCount];
            var testHandClose = new string[testNum, feCount];
            var testHandOpen = new string[testNum, feCount];
            // var testHandPeace = new string[testNum, feCount];
            // var testHandPistol = new string[testNum, feCount];

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
                    // trainFingerLittle[trainIter,0] = meFingerLittle[i].ToString(provider);
                    // trainFingerLittle[trainIter,1] = rmsFingerLittle[i].ToString(provider);
                    // trainFingerLittle[trainIter,2] = sscFingerLittle[i].ToString(provider);
                    // trainFingerLittle[trainIter,3] = ssiFingerLittle[i].ToString(provider);
                    // trainFingerLittle[trainIter,4] = varFingerLittle[i].ToString(provider);
                    // trainFingerLittle[trainIter,5] = wlFingerLittle[i].ToString(provider);
                    // trainFingerLittle[trainIter,6] = labels[1];

                    // finger middle
                    // trainFingerMiddle[trainIter,0] = meFingerMiddle[i].ToString(provider);
                    // trainFingerMiddle[trainIter,1] = rmsFingerMiddle[i].ToString(provider);
                    // trainFingerMiddle[trainIter,2] = sscFingerMiddle[i].ToString(provider);
                    // trainFingerMiddle[trainIter,3] = ssiFingerMiddle[i].ToString(provider);
                    // trainFingerMiddle[trainIter,4] = varFingerMiddle[i].ToString(provider);
                    // trainFingerMiddle[trainIter,5] = wlFingerMiddle[i].ToString(provider);
                    // trainFingerMiddle[trainIter,6] = labels[2];

                    // finger thumb
                    trainFingerThumb[trainIter,0] = meFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,1] = rmsFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,2] = sscFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,3] = ssiFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,4] = varFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,5] = wlFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,6] = labels[1];

                    // hand close
                    trainHandClose[trainIter,0] = meHandClose[i].ToString(provider);
                    trainHandClose[trainIter,1] = rmsHandClose[i].ToString(provider);
                    trainHandClose[trainIter,2] = sscHandClose[i].ToString(provider);
                    trainHandClose[trainIter,3] = ssiHandClose[i].ToString(provider);
                    trainHandClose[trainIter,4] = varHandClose[i].ToString(provider);
                    trainHandClose[trainIter,5] = wlHandClose[i].ToString(provider);
                    trainHandClose[trainIter,6] = labels[2];

                    // hand open
                    trainHandOpen[trainIter,0] = meHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,1] = rmsHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,2] = sscHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,3] = ssiHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,4] = varHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,5] = wlHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,6] = labels[3];

                    // hand Peace
                    // trainHandPeace[trainIter,0] = meHandPeace[i].ToString(provider);
                    // trainHandPeace[trainIter,1] = rmsHandPeace[i].ToString(provider);
                    // trainHandPeace[trainIter,2] = sscHandPeace[i].ToString(provider);
                    // trainHandPeace[trainIter,3] = ssiHandPeace[i].ToString(provider);
                    // trainHandPeace[trainIter,4] = varHandPeace[i].ToString(provider);
                    // trainHandPeace[trainIter,5] = wlHandPeace[i].ToString(provider);
                    // trainHandPeace[trainIter,6] = labels[6];

                    // hand pistol
                    // trainHandPistol[trainIter,0] = meHandPistol[i].ToString(provider);
                    // trainHandPistol[trainIter,1] = rmsHandPistol[i].ToString(provider);
                    // trainHandPistol[trainIter,2] = sscHandPistol[i].ToString(provider);
                    // trainHandPistol[trainIter,3] = ssiHandPistol[i].ToString(provider);
                    // trainHandPistol[trainIter,4] = varHandPistol[i].ToString(provider);
                    // trainHandPistol[trainIter,5] = wlHandPistol[i].ToString(provider);
                    // trainHandPistol[trainIter,6] = labels[7];

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
                    // testFingerLittle[testIter,0] = meFingerLittle[i].ToString(provider);
                    // testFingerLittle[testIter,1] = rmsFingerLittle[i].ToString(provider);
                    // testFingerLittle[testIter,2] = sscFingerLittle[i].ToString(provider);
                    // testFingerLittle[testIter,3] = ssiFingerLittle[i].ToString(provider);
                    // testFingerLittle[testIter,4] = varFingerLittle[i].ToString(provider);
                    // testFingerLittle[testIter,5] = wlFingerLittle[i].ToString(provider);
                    // testFingerLittle[testIter,6] = labels[1];

                    // finger middle
                    // testFingerMiddle[testIter,0] = meFingerMiddle[i].ToString(provider);
                    // testFingerMiddle[testIter,1] = rmsFingerMiddle[i].ToString(provider);
                    // testFingerMiddle[testIter,2] = sscFingerMiddle[i].ToString(provider);
                    // testFingerMiddle[testIter,3] = ssiFingerMiddle[i].ToString(provider);
                    // testFingerMiddle[testIter,4] = varFingerMiddle[i].ToString(provider);
                    // testFingerMiddle[testIter,5] = wlFingerMiddle[i].ToString(provider);
                    // testFingerMiddle[testIter,6] = labels[2];

                    // finger thumb
                    testFingerThumb[testIter,0] = meFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,1] = rmsFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,2] = sscFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,3] = ssiFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,4] = varFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,5] = wlFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,6] = labels[1];

                    // hand close
                    testHandClose[testIter,0] = meHandClose[i].ToString(provider);
                    testHandClose[testIter,1] = rmsHandClose[i].ToString(provider);
                    testHandClose[testIter,2] = sscHandClose[i].ToString(provider);
                    testHandClose[testIter,3] = ssiHandClose[i].ToString(provider);
                    testHandClose[testIter,4] = varHandClose[i].ToString(provider);
                    testHandClose[testIter,5] = wlHandClose[i].ToString(provider);
                    testHandClose[testIter,6] = labels[2];

                    // hand open
                    testHandOpen[testIter,0] = meHandOpen[i].ToString(provider);
                    testHandOpen[testIter,1] = rmsHandOpen[i].ToString(provider);
                    testHandOpen[testIter,2] = sscHandOpen[i].ToString(provider);
                    testHandOpen[testIter,3] = ssiHandOpen[i].ToString(provider);
                    testHandOpen[testIter,4] = varHandOpen[i].ToString(provider);
                    testHandOpen[testIter,5] = wlHandOpen[i].ToString(provider);
                    testHandOpen[testIter,6] = labels[3];

                    // hand Peace
                    // testHandPeace[testIter,0] = meHandPeace[i].ToString(provider);
                    // testHandPeace[testIter,1] = rmsHandPeace[i].ToString(provider);
                    // testHandPeace[testIter,2] = sscHandPeace[i].ToString(provider);
                    // testHandPeace[testIter,3] = ssiHandPeace[i].ToString(provider);
                    // testHandPeace[testIter,4] = varHandPeace[i].ToString(provider);
                    // testHandPeace[testIter,5] = wlHandPeace[i].ToString(provider);
                    // testHandPeace[testIter,6] = labels[6];

                    // hand pistol
                    // testHandPistol[testIter,0] = meHandPistol[i].ToString(provider);
                    // testHandPistol[testIter,1] = rmsHandPistol[i].ToString(provider);
                    // testHandPistol[testIter,2] = sscHandPistol[i].ToString(provider);
                    // testHandPistol[testIter,3] = ssiHandPistol[i].ToString(provider);
                    // testHandPistol[testIter,4] = varHandPistol[i].ToString(provider);
                    // testHandPistol[testIter,5] = wlHandPistol[i].ToString(provider);
                    // testHandPistol[testIter,6] = labels[7];

                    // increase iter
                    testIter++;
                }
            }

            // init path
            if (newData)
            {
                if (File.Exists(pathTrain)) File.Delete(pathTrain);
                if (File.Exists(pathTest)) File.Delete(pathTest);
            }

            // store data train
            CSVHandler.WriteCSV(pathTrain, trainFingerIndex, false, true, headers.ToArray());
            // CSVHandler.WriteCSV(pathTrain, trainFingerLittle, false);
            // CSVHandler.WriteCSV(pathTrain, trainFingerMiddle, false);
            CSVHandler.WriteCSV(pathTrain, trainFingerThumb, false);
            CSVHandler.WriteCSV(pathTrain, trainHandClose, false);
            CSVHandler.WriteCSV(pathTrain, trainHandOpen, false);
            // CSVHandler.WriteCSV(pathTrain, trainHandPeace, false);
            // CSVHandler.WriteCSV(pathTrain, trainHandPistol, false);

            // store data test
            CSVHandler.WriteCSV(pathTest, testFingerIndex, false, true, headers.ToArray());
            // CSVHandler.WriteCSV(pathTest, testFingerLittle, false);
            // CSVHandler.WriteCSV(pathTest, testFingerMiddle, false);
            CSVHandler.WriteCSV(pathTest, testFingerThumb, false);
            CSVHandler.WriteCSV(pathTest, testHandClose, false);
            CSVHandler.WriteCSV(pathTest, testHandOpen, false);
            // CSVHandler.WriteCSV(pathTest, testHandPeace, false);
            // CSVHandler.WriteCSV(pathTest, testHandPistol, false);

            // complete text
            display.text += "Data processing completed\n";
        }
        catch (System.Exception ex)
        {
            display.text += ex.Message + "\n";
        }
    }
    
    public async void TrainModel()
    {
        // start text
        display.text += "Wait for training...\n";
        trainProgressBar.fillAmount = 0;

        try
        {
            // read csv
            var dataTrain = CSVHandler.ReadCSV(pathTrain, true);
            var dataTest = CSVHandler.ReadCSV(pathTest, true);
            int featureCount = dataTrain[0].Split(',').Length - 1;

            // shuffle data
            var dataTrain2 = EMG.FisherYates(dataTrain);

            // preprocess data train
            var inputTrain = new double[dataTrain2.Count, featureCount];
            var labelTrain = new string[dataTrain2.Count];
            for (int i = 0; i < dataTrain2.Count; i++)
            {
                var data = dataTrain2[i].Split(',');
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

            // train model
            await EMG.Train(inputTrain, labelTrain, 10000, 0.001);

            // validate model
            await EMG.Validate(inputTest, labelTest);

            // get nn param
            int epoch = EMG.epoch;
            double loss = EMG.loss;
            double accuracy = EMG.accuracy;
            double time = EMG.time;

            // store param
            CSVHandler.WriteCSV(pathWH, EMG.wH, true);
            CSVHandler.WriteCSV(pathWOut, EMG.wOut, true);
            CSVHandler.WriteCSV(pathBH, EMG.bH, true);
            CSVHandler.WriteCSV(pathBOut, EMG.bOut, true);

            // complete text
            display.text += $"Accuracy: {accuracy}% | Time: {time} s | Epoch: {epoch} | Loss: {loss}\n";

            // init data train model
            var dataTrainHeader = new string[12]
            {
                "Timestamp",
                "Sensor",
                "Electrode",
                "Position",
                "Group",
                "FE Length",
                "FE Model",
                "ANN Model",
                "Epoch",
                "Loss",
                "Accuracy",
                "Time"
            };
            var dataTrainDatas = new string[12]
            {
                System.DateTime.Now.ToString(provider),
                "AD8232",
                dataElectrode,
                dataPosition,
                dataGroup,
                modelFELength,
                modelFEModel,
                modelANNModel,
                epoch.ToString(provider),
                loss.ToString(provider),
                accuracy.ToString(provider),
                time.ToString(provider)
            };

            // store data train
            bool header = true;
            if (File.Exists(pathTrainResult))
                header = false;
            CSVHandler.WriteCSV(pathTrainResult, dataTrainDatas, false, header, dataTrainHeader);
        }
        catch (System.Exception ex)
        {
            display.text += ex.Message + "\n";
        }
    }

    public void SetModel()
    {
        // start text
        display.text += "Wait for setting model...\n";

        try
        {
            // get data
            var wHString = CSVHandler.ReadCSV(pathWH);
            var wOutString = CSVHandler.ReadCSV(pathWOut);
            var bHString = CSVHandler.ReadCSV(pathBH);
            var bOutString = CSVHandler.ReadCSV(pathBOut);

            // get numerik
            var wHDatas = GetMatrix(wHString);
            var wOutDatas = GetMatrix(wOutString);
            var bHDatas = GetArray(bHString);
            var bOutDatas = GetArray(bOutString);

            // set model param
            EMG.SetParam("weight_hidden", wHDatas);
            EMG.SetParam("weight_output", wOutDatas);
            EMG.SetParam("bias_hidden", bHDatas);
            EMG.SetParam("bias_output", bOutDatas);

            // complete text
            display.text += "Setting model completed\n";
        }
        catch (System.Exception ex)
        {
            display.text += ex.Message + "\n";
        }
    }

    public async void ValidateModel()
    {
        // start text
        display.text += "Wait for validating...\n";

        try
        {
            // read csv
            var dataTest = CSVHandler.ReadCSV(pathTest, true);
            int featureCount = dataTest[0].Split(',').Length - 1;

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

            // validate model
            await EMG.Validate(inputTest, labelTest);

            // get nn param
            double accuracy = EMG.accuracy;

            // complete text
            display.text += "Accuracy: " + accuracy.ToString() + "%\n";
        }
        catch (System.Exception ex)
        {
            display.text += ex.Message + "\n";
        }
    }

    public string PredictModel(List<double> data)
    {
        // Mean
        var meData = FeatureExtraction.ME(data, num: meNum);

        // Root Mean Square
        var rmsData = FeatureExtraction.RMS(data, num: rmsNum);

        // Slope Sign Change
        var sscData = FeatureExtraction.SSC(data, num: sscNum);

        // Simple Square Integral
        var ssiData = FeatureExtraction.SSI(data, num: ssiNum);

        // Variance
        var varData = FeatureExtraction.VAR(data, num: varNum);

        // Wave Length
        var wlData = FeatureExtraction.WL(data, num: wlNum);

        // wrap data
        double[] input = new double[6]
        {
            meData, rmsData, sscData, ssiData, varData, wlData
        };

        // predict data
        var label = EMG.Predict(input);

        return label;
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

    private double[] GetArray(List<string> _datas)
    {
        int row = _datas.Count;
        var returnDatas = new double[row];
        
        for (int i = 0; i < row; i++)
        {
            returnDatas[i] = double.Parse(_datas[i], provider);
        }

        return returnDatas;
    }

    private double[,] GetMatrix(List<string> _datas)
    {
        int row = _datas.Count;
        int col = _datas[0].Split(',').Length;
        var returnDatas = new double[row, col];
        
        for (int i = 0; i < row; i++)
        {
            var data = _datas[i].Split(',');
            for (int j = 0; j < col; j++)
            {
                returnDatas[i,j] = double.Parse(data[j], provider);
            }
        }

        return returnDatas;
    }

}

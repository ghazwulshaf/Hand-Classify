using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JayaweeraModel : MonoBehaviour
{
    /******************************
    ** FIELDS
    *******************************/

    // UI
    public Text display;
    public Dropdown dropSensor;
    public Dropdown dropRange;
    public Dropdown dropSize;
    public InputField dataReceived;

    // global field
    private List<double> sensorDatas;
    private NumberFormatInfo provider;

    // model
    private ANNClassification1Layer EMG;
    private int features;
    private int labels;
    private int hiddenNodes;
    private int mavNum;
    private int iemgNum;
    private int zctNum;
    private int ssctNum;
    private int skewnessNum;
    private int myopNum;
    private int logNum;
    private int ldamvNum;
    private int ltkeoNum;
    private int mflNum;
    private int mmavNum;
    private int madNum;
    private int voNum;
    private int waNum;
    private int aacNum;
    private int dmavNum;

    // methods
    private CSVHandler CSVHandler;
    private FeatureExtraction FeatureExtraction;


    /******************************
    ** METHODS
    *******************************/

    void Start()
    {
        // init field
        sensorDatas = new List<double>();
        provider = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };

        // init model param
        features = 16;
        labels = 8;
        hiddenNodes = 16;

        // init model
        EMG = new ANNClassification1Layer(_featureNum: features, _labelNum: labels, _hiddenNode: hiddenNodes);
        mavNum = 3;
        iemgNum = 4;
        zctNum = 0;
        ssctNum = 0;
        skewnessNum = 0;
        myopNum = 0;
        logNum = 3;
        ldamvNum = 0;
        ltkeoNum = 0;
        mflNum = 0;
        mmavNum = 2;
        madNum = 2;
        voNum = 3;
        waNum = 1;
        aacNum = 1;
        dmavNum = 1;

        // init methods
        CSVHandler = new CSVHandler();
        FeatureExtraction = new FeatureExtraction();
    }

    public void DataChanged()
    {
        // wrap data
        double data = double.Parse(dataReceived.text);
        sensorDatas.Add(data);

        // set predict
        if (sensorDatas.Count == 150)
        {
            // predict data
            var label = PredictModel(sensorDatas);

            // print data
            display.text = label;

            // clear datas
            sensorDatas.Clear();
        }
    }

    public void ProcessModel()
    {
        // start text
        display.text = "Wait for processing data...";

        // get dropdown value
        var sensorIndex = dropSensor.value;
        var sensor = dropSensor.options[sensorIndex].text;
        var rangeIndex = dropRange.value;
        var range = dropRange.options[rangeIndex].text;
        var sizeIndex = dropSize.value;
        var size = dropSize.options[sizeIndex].text;

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
            new("MAV"),
            new("IEMG"),
            new("ZCT"),
            new("SSCT"),
            new("SKEWNESS"),
            new("MYOP"),
            new("LOG"),
            new("LDAMV"),
            new("LTKEO"),
            new("MFL"),
            new("MMAV"),
            new("MAD"),
            new("VO"),
            new("WA"),
            new("AAC"),
            new("DMAV"),
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

        // new source path
        pathFingerIndex = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_1.csv";
        pathFingerLittle = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_2.csv";
        pathFingerMiddle = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_3.csv";
        pathFingerThumb = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_4.csv";
        pathHandClose = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_5.csv";
        pathHandOpen = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_6.csv";
        pathHandPeace = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_7.csv";
        pathHandPistol = Application.persistentDataPath + "/" + sensor + "_Raw_" + size + "_8.csv";

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
            int feRange = int.Parse(range);
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

            // feature: Mean Absolute Value
            var mavFingerIndex = FeatureExtraction.MAV(doubleFingerIndex, feRange, num: mavNum);
            var mavFingerLittle = FeatureExtraction.MAV(doubleFingerLittle, feRange, num: mavNum);
            var mavFingerMiddle = FeatureExtraction.MAV(doubleFingerMiddle, feRange, num: mavNum);
            var mavFingerThumb = FeatureExtraction.MAV(doubleFingerThumb, feRange, num: mavNum);
            var mavHandClose = FeatureExtraction.MAV(doubleHandClose, feRange, num: mavNum);
            var mavHandOpen = FeatureExtraction.MAV(doubleHandOpen, feRange, num: mavNum);
            var mavHandPeace = FeatureExtraction.MAV(doubleHandPeace, feRange, num: mavNum);
            var mavHandPistol = FeatureExtraction.MAV(doubleHandPistol, feRange, num: mavNum);

            // feature: Autoregressive Coefficients

            // feature: Integrated EMG
            var iemgFingerIndex = FeatureExtraction.IEMG(doubleFingerIndex, feRange, num: iemgNum);
            var iemgFingerLittle = FeatureExtraction.IEMG(doubleFingerLittle, feRange, num: iemgNum);
            var iemgFingerMiddle = FeatureExtraction.IEMG(doubleFingerMiddle, feRange, num: iemgNum);
            var iemgFingerThumb = FeatureExtraction.IEMG(doubleFingerThumb, feRange, num: iemgNum);
            var iemgHandClose = FeatureExtraction.IEMG(doubleHandClose, feRange, num: iemgNum);
            var iemgHandOpen = FeatureExtraction.IEMG(doubleHandOpen, feRange, num: iemgNum);
            var iemgHandPeace = FeatureExtraction.IEMG(doubleHandPeace, feRange, num: iemgNum);
            var iemgHandPistol = FeatureExtraction.IEMG(doubleHandPistol, feRange, num: iemgNum);

            // feature: Zero Crossings with Threshold
            double zctThreshold = 0.01;
            var zctFingerIndex = FeatureExtraction.ZCT(doubleFingerIndex, feRange, zctThreshold);
            var zctFingerLittle = FeatureExtraction.ZCT(doubleFingerLittle, feRange, zctThreshold);
            var zctFingerMiddle = FeatureExtraction.ZCT(doubleFingerMiddle, feRange, zctThreshold);
            var zctFingerThumb = FeatureExtraction.ZCT(doubleFingerThumb, feRange, zctThreshold);
            var zctHandClose = FeatureExtraction.ZCT(doubleHandClose, feRange, zctThreshold);
            var zctHandOpen = FeatureExtraction.ZCT(doubleHandOpen, feRange, zctThreshold);
            var zctHandPeace = FeatureExtraction.ZCT(doubleHandPeace, feRange, zctThreshold);
            var zctHandPistol = FeatureExtraction.ZCT(doubleHandPistol, feRange, zctThreshold);

            // feature: Slope Sign Change with Threshold
            double ssctThreshold = 0.01;
            var ssctFingerIndex = FeatureExtraction.SSCT(doubleFingerIndex, feRange, ssctThreshold);
            var ssctFingerLittle = FeatureExtraction.SSCT(doubleFingerLittle, feRange, ssctThreshold);
            var ssctFingerMiddle = FeatureExtraction.SSCT(doubleFingerMiddle, feRange, ssctThreshold);
            var ssctFingerThumb = FeatureExtraction.SSCT(doubleFingerThumb, feRange, ssctThreshold);
            var ssctHandClose = FeatureExtraction.SSCT(doubleHandClose, feRange, ssctThreshold);
            var ssctHandOpen = FeatureExtraction.SSCT(doubleHandOpen, feRange, ssctThreshold);
            var ssctHandPeace = FeatureExtraction.SSCT(doubleHandPeace, feRange, ssctThreshold);
            var ssctHandPistol = FeatureExtraction.SSCT(doubleHandPistol, feRange, ssctThreshold);

            // feature: Skewness from Paper
            var skewnessFingerIndex = FeatureExtraction.SKEWNESS(doubleFingerIndex, feRange);
            var skewnessFingerLittle = FeatureExtraction.SKEWNESS(doubleFingerLittle, feRange);
            var skewnessFingerMiddle = FeatureExtraction.SKEWNESS(doubleFingerMiddle, feRange);
            var skewnessFingerThumb = FeatureExtraction.SKEWNESS(doubleFingerThumb, feRange);
            var skewnessHandClose = FeatureExtraction.SKEWNESS(doubleHandClose, feRange);
            var skewnessHandOpen = FeatureExtraction.SKEWNESS(doubleHandOpen, feRange);
            var skewnessHandPeace = FeatureExtraction.SKEWNESS(doubleHandPeace, feRange);
            var skewnessHandPistol = FeatureExtraction.SKEWNESS(doubleHandPistol, feRange);

            // feature: Myopulse Percentage Rate
            double myopThreshold = 0.016;
            var myopFingerIndex = FeatureExtraction.MYOP(doubleFingerIndex, feRange, myopThreshold);
            var myopFingerLittle = FeatureExtraction.MYOP(doubleFingerLittle, feRange, myopThreshold);
            var myopFingerMiddle = FeatureExtraction.MYOP(doubleFingerMiddle, feRange, myopThreshold);
            var myopFingerThumb = FeatureExtraction.MYOP(doubleFingerThumb, feRange, myopThreshold);
            var myopHandClose = FeatureExtraction.MYOP(doubleHandClose, feRange, myopThreshold);
            var myopHandOpen = FeatureExtraction.MYOP(doubleHandOpen, feRange, myopThreshold);
            var myopHandPeace = FeatureExtraction.MYOP(doubleHandPeace, feRange, myopThreshold);
            var myopHandPistol = FeatureExtraction.MYOP(doubleHandPistol, feRange, myopThreshold);

            // feature: Log Detector from Paper
            var logFingerIndex = FeatureExtraction.LOG(doubleFingerIndex, feRange, num: logNum);
            var logFingerLittle = FeatureExtraction.LOG(doubleFingerLittle, feRange, num: logNum);
            var logFingerMiddle = FeatureExtraction.LOG(doubleFingerMiddle, feRange, num: logNum);
            var logFingerThumb = FeatureExtraction.LOG(doubleFingerThumb, feRange, num: logNum);
            var logHandClose = FeatureExtraction.LOG(doubleHandClose, feRange, num: logNum);
            var logHandOpen = FeatureExtraction.LOG(doubleHandOpen, feRange, num: logNum);
            var logHandPeace = FeatureExtraction.LOG(doubleHandPeace, feRange, num: logNum);
            var logHandPistol = FeatureExtraction.LOG(doubleHandPistol, feRange, num: logNum);

            // feature: Temporal Moment

            // feature: Log Difference Absolute Mean Value
            var ldamvFingerIndex = FeatureExtraction.LDAMV(doubleFingerIndex, feRange);
            var ldamvFingerLittle = FeatureExtraction.LDAMV(doubleFingerLittle, feRange);
            var ldamvFingerMiddle = FeatureExtraction.LDAMV(doubleFingerMiddle, feRange);
            var ldamvFingerThumb = FeatureExtraction.LDAMV(doubleFingerThumb, feRange);
            var ldamvHandClose = FeatureExtraction.LDAMV(doubleHandClose, feRange);
            var ldamvHandOpen = FeatureExtraction.LDAMV(doubleHandOpen, feRange);
            var ldamvHandPeace = FeatureExtraction.LDAMV(doubleHandPeace, feRange);
            var ldamvHandPistol = FeatureExtraction.LDAMV(doubleHandPistol, feRange);

            // feature: Log Teager Kaiser Energy Operator
            var ltkeoFingerIndex = FeatureExtraction.LTKEO(doubleFingerIndex, feRange);
            var ltkeoFingerLittle = FeatureExtraction.LTKEO(doubleFingerLittle, feRange);
            var ltkeoFingerMiddle = FeatureExtraction.LTKEO(doubleFingerMiddle, feRange);
            var ltkeoFingerThumb = FeatureExtraction.LTKEO(doubleFingerThumb, feRange);
            var ltkeoHandClose = FeatureExtraction.LTKEO(doubleHandClose, feRange);
            var ltkeoHandOpen = FeatureExtraction.LTKEO(doubleHandOpen, feRange);
            var ltkeoHandPeace = FeatureExtraction.LTKEO(doubleHandPeace, feRange);
            var ltkeoHandPistol = FeatureExtraction.LTKEO(doubleHandPistol, feRange);

            // feature: Maximum Fractal Length
            var mflFingerIndex = FeatureExtraction.MFL(doubleFingerIndex, feRange);
            var mflFingerLittle = FeatureExtraction.MFL(doubleFingerLittle, feRange);
            var mflFingerMiddle = FeatureExtraction.MFL(doubleFingerMiddle, feRange);
            var mflFingerThumb = FeatureExtraction.MFL(doubleFingerThumb, feRange);
            var mflHandClose = FeatureExtraction.MFL(doubleHandClose, feRange);
            var mflHandOpen = FeatureExtraction.MFL(doubleHandOpen, feRange);
            var mflHandPeace = FeatureExtraction.MFL(doubleHandPeace, feRange);
            var mflHandPistol = FeatureExtraction.MFL(doubleHandPistol, feRange);

            // feature: Modified Mean Absolute Value
            var mmavFingerIndex = FeatureExtraction.MMAV(doubleFingerIndex, feRange, num: mmavNum);
            var mmavFingerLittle = FeatureExtraction.MMAV(doubleFingerLittle, feRange, num: mmavNum);
            var mmavFingerMiddle = FeatureExtraction.MMAV(doubleFingerMiddle, feRange, num: mmavNum);
            var mmavFingerThumb = FeatureExtraction.MMAV(doubleFingerThumb, feRange, num: mmavNum);
            var mmavHandClose = FeatureExtraction.MMAV(doubleHandClose, feRange, num: mmavNum);
            var mmavHandOpen = FeatureExtraction.MMAV(doubleHandOpen, feRange, num: mmavNum);
            var mmavHandPeace = FeatureExtraction.MMAV(doubleHandPeace, feRange, num: mmavNum);
            var mmavHandPistol = FeatureExtraction.MMAV(doubleHandPistol, feRange, num: mmavNum);

            // feature: Mean Absolute Deviation
            var madFingerIndex = FeatureExtraction.MAD(doubleFingerIndex, feRange, num: madNum);
            var madFingerLittle = FeatureExtraction.MAD(doubleFingerLittle, feRange, num: madNum);
            var madFingerMiddle = FeatureExtraction.MAD(doubleFingerMiddle, feRange, num: madNum);
            var madFingerThumb = FeatureExtraction.MAD(doubleFingerThumb, feRange, num: madNum);
            var madHandClose = FeatureExtraction.MAD(doubleHandClose, feRange, num: madNum);
            var madHandOpen = FeatureExtraction.MAD(doubleHandOpen, feRange, num: madNum);
            var madHandPeace = FeatureExtraction.MAD(doubleHandPeace, feRange, num: madNum);
            var madHandPistol = FeatureExtraction.MAD(doubleHandPistol, feRange, num: madNum);

            // feature: v-Order
            int voOrder = 2;
            var voFingerIndex = FeatureExtraction.VO(doubleFingerIndex, feRange, voOrder, num: voNum);
            var voFingerLittle = FeatureExtraction.VO(doubleFingerLittle, feRange, voOrder, num: voNum);
            var voFingerMiddle = FeatureExtraction.VO(doubleFingerMiddle, feRange, voOrder, num: voNum);
            var voFingerThumb = FeatureExtraction.VO(doubleFingerThumb, feRange, voOrder, num: voNum);
            var voHandClose = FeatureExtraction.VO(doubleHandClose, feRange, voOrder, num: voNum);
            var voHandOpen = FeatureExtraction.VO(doubleHandOpen, feRange, voOrder, num: voNum);
            var voHandPeace = FeatureExtraction.VO(doubleHandPeace, feRange, voOrder, num: voNum);
            var voHandPistol = FeatureExtraction.VO(doubleHandPistol, feRange, voOrder, num: voNum);

            // feature: Willison Amplitude
            double waThreshold = 0.01;
            var waFingerIndex = FeatureExtraction.WA(doubleFingerIndex, feRange, waThreshold, num: waNum);
            var waFingerLittle = FeatureExtraction.WA(doubleFingerLittle, feRange, waThreshold, num: waNum);
            var waFingerMiddle = FeatureExtraction.WA(doubleFingerMiddle, feRange, waThreshold, num: waNum);
            var waFingerThumb = FeatureExtraction.WA(doubleFingerThumb, feRange, waThreshold, num: waNum);
            var waHandClose = FeatureExtraction.WA(doubleHandClose, feRange, waThreshold, num: waNum);
            var waHandOpen = FeatureExtraction.WA(doubleHandOpen, feRange, waThreshold, num: waNum);
            var waHandPeace = FeatureExtraction.WA(doubleHandPeace, feRange, waThreshold, num: waNum);
            var waHandPistol = FeatureExtraction.WA(doubleHandPistol, feRange, waThreshold, num: waNum);

            // feature: Average Amplitude Change
            var aacFingerIndex = FeatureExtraction.AAC(doubleFingerIndex, feRange, num: aacNum);
            var aacFingerLittle = FeatureExtraction.AAC(doubleFingerLittle, feRange, num: aacNum);
            var aacFingerMiddle = FeatureExtraction.AAC(doubleFingerMiddle, feRange, num: aacNum);
            var aacFingerThumb = FeatureExtraction.AAC(doubleFingerThumb, feRange, num: aacNum);
            var aacHandClose = FeatureExtraction.AAC(doubleHandClose, feRange, num: aacNum);
            var aacHandOpen = FeatureExtraction.AAC(doubleHandOpen, feRange, num: aacNum);
            var aacHandPeace = FeatureExtraction.AAC(doubleHandPeace, feRange, num: aacNum);
            var aacHandPistol = FeatureExtraction.AAC(doubleHandPistol, feRange, num: aacNum);

            // feature: Hjorth Mobility and Complexity

            // feature: Difference Mean Absolute Value
            var dmavFingerIndex = FeatureExtraction.DMAV(doubleFingerIndex, feRange, num: dmavNum);
            var dmavFingerLittle = FeatureExtraction.DMAV(doubleFingerLittle, feRange, num: dmavNum);
            var dmavFingerMiddle = FeatureExtraction.DMAV(doubleFingerMiddle, feRange, num: dmavNum);
            var dmavFingerThumb = FeatureExtraction.DMAV(doubleFingerThumb, feRange, num: dmavNum);
            var dmavHandClose = FeatureExtraction.DMAV(doubleHandClose, feRange, num: dmavNum);
            var dmavHandOpen = FeatureExtraction.DMAV(doubleHandOpen, feRange, num: dmavNum);
            var dmavHandPeace = FeatureExtraction.DMAV(doubleHandPeace, feRange, num: dmavNum);
            var dmavHandPistol = FeatureExtraction.DMAV(doubleHandPistol, feRange, num: dmavNum);

            // init train num
            int feSize = mavFingerIndex.Count;
            int testNum = feSize * 30 / 100;
            int trainNum = feSize - testNum;
            int feCount = 16;
            int datasCol = feCount + 1;

            // init train array
            var trainFingerIndex = new string[trainNum, datasCol];
            var trainFingerLittle = new string[trainNum, datasCol];
            var trainFingerMiddle = new string[trainNum, datasCol];
            var trainFingerThumb = new string[trainNum, datasCol];
            var trainHandClose = new string[trainNum, datasCol];
            var trainHandOpen = new string[trainNum, datasCol];
            var trainHandPeace = new string[trainNum, datasCol];
            var trainHandPistol = new string[trainNum, datasCol];

            // init test array
            var testFingerIndex = new string[testNum, datasCol];
            var testFingerLittle = new string[testNum, datasCol];
            var testFingerMiddle = new string[testNum, datasCol];
            var testFingerThumb = new string[testNum, datasCol];
            var testHandClose = new string[testNum, datasCol];
            var testHandOpen = new string[testNum, datasCol];
            var testHandPeace = new string[testNum, datasCol];
            var testHandPistol = new string[testNum, datasCol];

            // wrap datas
            int trainIter = 0;
            int testIter = 0;
            for (int i = 0; i < feSize; i++)
            {
                if (i < trainNum)
                {
                    // finger index
                    trainFingerIndex[trainIter,0] = mavFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,1] = iemgFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,2] = zctFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,3] = ssctFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,4] = skewnessFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,5] = myopFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,6] = logFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,7] = ldamvFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,8] = ltkeoFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,9] = mflFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,10] = mmavFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,11] = madFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,12] = voFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,13] = waFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,14] = aacFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,15] = dmavFingerIndex[i].ToString(provider);
                    trainFingerIndex[trainIter,16] = labels[0];

                    // finger little
                    trainFingerLittle[trainIter,0] = mavFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,1] = iemgFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,2] = zctFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,3] = ssctFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,4] = skewnessFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,5] = myopFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,6] = logFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,7] = ldamvFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,8] = ltkeoFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,9] = mflFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,10] = mmavFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,11] = madFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,12] = voFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,13] = waFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,14] = aacFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,15] = dmavFingerLittle[i].ToString(provider);
                    trainFingerLittle[trainIter,16] = labels[1];

                    // finger middle
                    trainFingerMiddle[trainIter,0] = mavFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,1] = iemgFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,2] = zctFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,3] = ssctFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,4] = skewnessFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,5] = myopFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,6] = logFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,7] = ldamvFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,8] = ltkeoFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,9] = mflFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,10] = mmavFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,11] = madFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,12] = voFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,13] = waFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,14] = aacFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,15] = dmavFingerMiddle[i].ToString(provider);
                    trainFingerMiddle[trainIter,16] = labels[2];

                    // finger thumb
                    trainFingerThumb[trainIter,0] = mavFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,1] = iemgFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,2] = zctFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,3] = ssctFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,4] = skewnessFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,5] = myopFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,6] = logFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,7] = ldamvFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,8] = ltkeoFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,9] = mflFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,10] = mmavFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,11] = madFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,12] = voFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,13] = waFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,14] = aacFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,15] = dmavFingerThumb[i].ToString(provider);
                    trainFingerThumb[trainIter,16] = labels[3];

                    // hand close
                    trainHandClose[trainIter,0] = mavHandClose[i].ToString(provider);
                    trainHandClose[trainIter,1] = iemgHandClose[i].ToString(provider);
                    trainHandClose[trainIter,2] = zctHandClose[i].ToString(provider);
                    trainHandClose[trainIter,3] = ssctHandClose[i].ToString(provider);
                    trainHandClose[trainIter,4] = skewnessHandClose[i].ToString(provider);
                    trainHandClose[trainIter,5] = myopHandClose[i].ToString(provider);
                    trainHandClose[trainIter,6] = logHandClose[i].ToString(provider);
                    trainHandClose[trainIter,7] = ldamvHandClose[i].ToString(provider);
                    trainHandClose[trainIter,8] = ltkeoHandClose[i].ToString(provider);
                    trainHandClose[trainIter,9] = mflHandClose[i].ToString(provider);
                    trainHandClose[trainIter,10] = mmavHandClose[i].ToString(provider);
                    trainHandClose[trainIter,11] = madHandClose[i].ToString(provider);
                    trainHandClose[trainIter,12] = voHandClose[i].ToString(provider);
                    trainHandClose[trainIter,13] = waHandClose[i].ToString(provider);
                    trainHandClose[trainIter,14] = aacHandClose[i].ToString(provider);
                    trainHandClose[trainIter,15] = dmavHandClose[i].ToString(provider);
                    trainHandClose[trainIter,16] = labels[4];

                    // hand open
                    trainHandOpen[trainIter,0] = mavHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,1] = iemgHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,2] = zctHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,3] = ssctHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,4] = skewnessHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,5] = myopHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,6] = logHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,7] = ldamvHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,8] = ltkeoHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,9] = mflHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,10] = mmavHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,11] = madHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,12] = voHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,13] = waHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,14] = aacHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,15] = dmavHandOpen[i].ToString(provider);
                    trainHandOpen[trainIter,16] = labels[5];

                    // hand Peace
                    trainHandPeace[trainIter,0] = mavHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,1] = iemgHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,2] = zctHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,3] = ssctHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,4] = skewnessHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,5] = myopHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,6] = logHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,7] = ldamvHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,8] = ltkeoHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,9] = mflHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,10] = mmavHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,11] = madHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,12] = voHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,13] = waHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,14] = aacHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,15] = dmavHandPeace[i].ToString(provider);
                    trainHandPeace[trainIter,16] = labels[6];

                    // hand pistol
                    trainHandPistol[trainIter,0] = mavHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,1] = iemgHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,2] = zctHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,3] = ssctHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,4] = skewnessHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,5] = myopHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,6] = logHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,7] = ldamvHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,8] = ltkeoHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,9] = mflHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,10] = mmavHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,11] = madHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,12] = voHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,13] = waHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,14] = aacHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,15] = dmavHandPistol[i].ToString(provider);
                    trainHandPistol[trainIter,16] = labels[7];

                    // increase iter
                    trainIter++;
                }
                else
                {
                    // finger index
                    testFingerIndex[testIter,0] = mavFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,1] = iemgFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,2] = zctFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,3] = ssctFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,4] = skewnessFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,5] = myopFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,6] = logFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,7] = ldamvFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,8] = ltkeoFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,9] = mflFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,10] = mmavFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,11] = madFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,12] = voFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,13] = waFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,14] = aacFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,15] = dmavFingerIndex[i].ToString(provider);
                    testFingerIndex[testIter,16] = labels[0];

                    // finger little
                    testFingerLittle[testIter,0] = mavFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,1] = iemgFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,2] = zctFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,3] = ssctFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,4] = skewnessFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,5] = myopFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,6] = logFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,7] = ldamvFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,8] = ltkeoFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,9] = mflFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,10] = mmavFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,11] = madFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,12] = voFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,13] = waFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,14] = aacFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,15] = dmavFingerLittle[i].ToString(provider);
                    testFingerLittle[testIter,16] = labels[1];

                    // finger middle
                    testFingerMiddle[testIter,0] = mavFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,1] = iemgFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,2] = zctFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,3] = ssctFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,4] = skewnessFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,5] = myopFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,6] = logFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,7] = ldamvFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,8] = ltkeoFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,9] = mflFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,10] = mmavFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,11] = madFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,12] = voFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,13] = waFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,14] = aacFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,15] = dmavFingerMiddle[i].ToString(provider);
                    testFingerMiddle[testIter,16] = labels[2];

                    // finger thumb
                    testFingerThumb[testIter,0] = mavFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,1] = iemgFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,2] = zctFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,3] = ssctFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,4] = skewnessFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,5] = myopFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,6] = logFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,7] = ldamvFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,8] = ltkeoFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,9] = mflFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,10] = mmavFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,11] = madFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,12] = voFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,13] = waFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,14] = aacFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,15] = dmavFingerThumb[i].ToString(provider);
                    testFingerThumb[testIter,16] = labels[3];

                    // hand close
                    testHandClose[testIter,0] = mavHandClose[i].ToString(provider);
                    testHandClose[testIter,1] = iemgHandClose[i].ToString(provider);
                    testHandClose[testIter,2] = zctHandClose[i].ToString(provider);
                    testHandClose[testIter,3] = ssctHandClose[i].ToString(provider);
                    testHandClose[testIter,4] = skewnessHandClose[i].ToString(provider);
                    testHandClose[testIter,5] = myopHandClose[i].ToString(provider);
                    testHandClose[testIter,6] = logHandClose[i].ToString(provider);
                    testHandClose[testIter,7] = ldamvHandClose[i].ToString(provider);
                    testHandClose[testIter,8] = ltkeoHandClose[i].ToString(provider);
                    testHandClose[testIter,9] = mflHandClose[i].ToString(provider);
                    testHandClose[testIter,10] = mmavHandClose[i].ToString(provider);
                    testHandClose[testIter,11] = madHandClose[i].ToString(provider);
                    testHandClose[testIter,12] = voHandClose[i].ToString(provider);
                    testHandClose[testIter,13] = waHandClose[i].ToString(provider);
                    testHandClose[testIter,14] = aacHandClose[i].ToString(provider);
                    testHandClose[testIter,15] = dmavHandClose[i].ToString(provider);
                    testHandClose[testIter,16] = labels[4];

                    // hand open
                    testHandOpen[testIter,0] = mavHandOpen[i].ToString(provider);
                    testHandOpen[testIter,1] = iemgHandOpen[i].ToString(provider);
                    testHandOpen[testIter,2] = zctHandOpen[i].ToString(provider);
                    testHandOpen[testIter,3] = ssctHandOpen[i].ToString(provider);
                    testHandOpen[testIter,4] = skewnessHandOpen[i].ToString(provider);
                    testHandOpen[testIter,5] = myopHandOpen[i].ToString(provider);
                    testHandOpen[testIter,6] = logHandOpen[i].ToString(provider);
                    testHandOpen[testIter,7] = ldamvHandOpen[i].ToString(provider);
                    testHandOpen[testIter,8] = ltkeoHandOpen[i].ToString(provider);
                    testHandOpen[testIter,9] = mflHandOpen[i].ToString(provider);
                    testHandOpen[testIter,10] = mmavHandOpen[i].ToString(provider);
                    testHandOpen[testIter,11] = madHandOpen[i].ToString(provider);
                    testHandOpen[testIter,12] = voHandOpen[i].ToString(provider);
                    testHandOpen[testIter,13] = waHandOpen[i].ToString(provider);
                    testHandOpen[testIter,14] = aacHandOpen[i].ToString(provider);
                    testHandOpen[testIter,15] = dmavHandOpen[i].ToString(provider);
                    testHandOpen[testIter,16] = labels[5];

                    // hand Peace
                    testHandPeace[testIter,0] = mavHandPeace[i].ToString(provider);
                    testHandPeace[testIter,1] = iemgHandPeace[i].ToString(provider);
                    testHandPeace[testIter,2] = zctHandPeace[i].ToString(provider);
                    testHandPeace[testIter,3] = ssctHandPeace[i].ToString(provider);
                    testHandPeace[testIter,4] = skewnessHandPeace[i].ToString(provider);
                    testHandPeace[testIter,5] = myopHandPeace[i].ToString(provider);
                    testHandPeace[testIter,6] = logHandPeace[i].ToString(provider);
                    testHandPeace[testIter,7] = ldamvHandPeace[i].ToString(provider);
                    testHandPeace[testIter,8] = ltkeoHandPeace[i].ToString(provider);
                    testHandPeace[testIter,9] = mflHandPeace[i].ToString(provider);
                    testHandPeace[testIter,10] = mmavHandPeace[i].ToString(provider);
                    testHandPeace[testIter,11] = madHandPeace[i].ToString(provider);
                    testHandPeace[testIter,12] = voHandPeace[i].ToString(provider);
                    testHandPeace[testIter,13] = waHandPeace[i].ToString(provider);
                    testHandPeace[testIter,14] = aacHandPeace[i].ToString(provider);
                    testHandPeace[testIter,15] = dmavHandPeace[i].ToString(provider);
                    testHandPeace[testIter,16] = labels[6];

                    // hand pistol
                    testHandPistol[testIter,0] = mavHandPistol[i].ToString(provider);
                    testHandPistol[testIter,1] = iemgHandPistol[i].ToString(provider);
                    testHandPistol[testIter,2] = zctHandPistol[i].ToString(provider);
                    testHandPistol[testIter,3] = ssctHandPistol[i].ToString(provider);
                    testHandPistol[testIter,4] = skewnessHandPistol[i].ToString(provider);
                    testHandPistol[testIter,5] = myopHandPistol[i].ToString(provider);
                    testHandPistol[testIter,6] = logHandPistol[i].ToString(provider);
                    testHandPistol[testIter,7] = ldamvHandPistol[i].ToString(provider);
                    testHandPistol[testIter,8] = ltkeoHandPistol[i].ToString(provider);
                    testHandPistol[testIter,9] = mflHandPistol[i].ToString(provider);
                    testHandPistol[testIter,10] = mmavHandPistol[i].ToString(provider);
                    testHandPistol[testIter,11] = madHandPistol[i].ToString(provider);
                    testHandPistol[testIter,12] = voHandPistol[i].ToString(provider);
                    testHandPistol[testIter,13] = waHandPistol[i].ToString(provider);
                    testHandPistol[testIter,14] = aacHandPistol[i].ToString(provider);
                    testHandPistol[testIter,15] = dmavHandPistol[i].ToString(provider);
                    testHandPistol[testIter,16] = labels[7];

                    // increase iter
                    testIter++;
                }
            }

            // init path
            string trainPath = Application.persistentDataPath + "/" + sensor + "_Train_" + size + "_" + range + "_Jayaweera.csv";
            string testPath = Application.persistentDataPath + "/" + sensor + "_Test_" + size + "_" + range + "_Jayaweera.csv";
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
    
    public async void TrainModel()
    {
        // start text
        display.text = "Wait for training...";

        // get dropdown value
        var sensorIndex = dropSensor.value;
        var sensor = dropSensor.options[sensorIndex].text;
        var rangeIndex = dropRange.value;
        var range = dropRange.options[rangeIndex].text;
        var sizeIndex = dropSize.value;
        var size = dropSize.options[sizeIndex].text;

        // source path
        string pathTrain = Application.persistentDataPath + "/" + sensor + "_Train_" + size + "_" + range + "_Jayaweera.csv";
        string pathTest = Application.persistentDataPath + "/" + sensor + "_Test_" + size + "_" + range + "_Jayaweera.csv";

        // param path
        string pathWH = Application.persistentDataPath + "/" + sensor + "_WH_" + size + "_" + range + "_Jayaweera.csv";
        string pathWOut = Application.persistentDataPath + "/" + sensor + "_WOut_" + size + "_" + range + "_Jayaweera.csv";
        string pathBH = Application.persistentDataPath + "/" + sensor + "_BH_" + size + "_" + range + "_Jayaweera.csv";
        string pathBOut = Application.persistentDataPath + "/" + sensor + "_BOut_" + size + "_" + range + "_Jayaweera.csv";

        try
        {
            // read csv
            var dataTrain = CSVHandler.ReadCSV(pathTrain, true);
            var dataTest = CSVHandler.ReadCSV(pathTest, true);
            int featureCount = features;

            // optimize data
            // dataTrain = OptimizeData(dataTrain);
            // dataTest = OptimizeData(dataTest);

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
            display.text = $"Accuracy: {accuracy}% | Time: {time} s | Epoch: {epoch} | Loss: {loss}";

            // init data train model
            var dataTrainHeader = new string[8]
            {
                "Timestamp",
                "Sensor",
                "Size",
                "Range",
                "Epoch",
                "Loss",
                "Accuracy",
                "Time"
            };
            var dataTrainDatas = new string[8]
            {
                System.DateTime.Now.ToString(provider),
                sensor,
                size,
                range,
                epoch.ToString(provider),
                loss.ToString(provider),
                accuracy.ToString(provider),
                time.ToString(provider)
            };

            // store data train
            string dataTrainPath = Application.persistentDataPath + "/EMG_DataTrain_Jayaweera.csv";
            bool header = true;
            if (File.Exists(dataTrainPath))
                header = false;
            CSVHandler.WriteCSV(dataTrainPath, dataTrainDatas, false, header, dataTrainHeader);
        }
        catch (System.Exception ex)
        {
            display.text = ex.Message;
        }
    }

    public void SetModel()
    {
        // start text
        display.text = "Wait for setting model...";

        // get dropdown value
        var sensorIndex = dropSensor.value;
        var sensor = dropSensor.options[sensorIndex].text;
        var rangeIndex = dropRange.value;
        var range = dropRange.options[rangeIndex].text;
        var sizeIndex = dropSize.value;
        var size = dropSize.options[sizeIndex].text;

        // source path
        string wHPath = Application.persistentDataPath + "/" + sensor + "_WH_" + size + "_" + range + "._Jayaweeracsv";
        string wOutPath = Application.persistentDataPath + "/" + sensor + "_WOut_" + size + "_" + range + "_Jayaweera.csv";
        string bHPath = Application.persistentDataPath + "/" + sensor + "_BH_" + size + "_" + range + "_Jayaweera.csv";
        string bOutPath = Application.persistentDataPath + "/" + sensor + "_BOut_" + size + "_" + range + "_Jayaweera.csv";

        try
        {
            // get data
            var wHString = CSVHandler.ReadCSV(wHPath);
            var wOutString = CSVHandler.ReadCSV(wOutPath);
            var bHString = CSVHandler.ReadCSV(bHPath);
            var bOutString = CSVHandler.ReadCSV(bOutPath);

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
            display.text = "Setting model completed";
        }
        catch (System.Exception ex)
        {
            display.text = ex.Message;
        }
    }

    public async void ValidateModel()
    {
        // start text
        display.text = "Wait for validating...";

        // get dropdown value
        var sensorIndex = dropSensor.value;
        var sensor = dropSensor.options[sensorIndex].text;
        var rangeIndex = dropRange.value;
        var range = dropRange.options[rangeIndex].text;
        var sizeIndex = dropSize.value;
        var size = dropSize.options[sizeIndex].text;

        // source path
        string pathTest = Application.persistentDataPath + "/" + sensor + "_Test_" + size + "_" + range + "_Jayaweera.csv";

        try
        {
            // read csv
            var dataTest = CSVHandler.ReadCSV(pathTest, true);
            int featureCount = features;

            // optimize data

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
            display.text = "Accuracy: " + accuracy.ToString() + "%";
        }
        catch (System.Exception ex)
        {
            display.text = ex.Message;
        }
    }

    public string PredictModel(List<double> datas)
    {
        var mavData = FeatureExtraction.MAV(datas, num: mavNum);
        var iemgData = FeatureExtraction.IEMG(datas, num: iemgNum);
        var zctData = FeatureExtraction.ZCT(datas, 0.01);
        var ssctData = FeatureExtraction.SSCT(datas, 0.01);
        var skewnessData = FeatureExtraction.SKEWNESS(datas);
        var myopData = FeatureExtraction.MYOP(datas, 0.016);
        var logData = FeatureExtraction.LOG(datas, num: logNum);
        var ldamvData = FeatureExtraction.LDAMV(datas);
        var ltkeoData = FeatureExtraction.LTKEO(datas);
        var mflData = FeatureExtraction.MFL(datas);
        var mmavData = FeatureExtraction.MMAV(datas, num: mmavNum);
        var madData = FeatureExtraction.MAD(datas, num: madNum);
        var voData = FeatureExtraction.VO(datas, 2, num: voNum);
        var waData = FeatureExtraction.WA(datas, 0.01, num: waNum);
        var aacData = FeatureExtraction.AAC(datas, num: aacNum);
        var dmavData = FeatureExtraction.DMAV(datas, num: dmavNum);

        // wrap data
        double[] input = new double[16]
        {
            mavData, iemgData, zctData, ssctData,
            skewnessData, myopData, logData, ldamvData,
            ltkeoData, mflData, mmavData, madData,
            voData, waData, aacData, dmavData
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

    private List<string> OptimizeData(List<string> datas)
    {
        var newDatas = new List<string>(datas);
        int iter = 0;
        foreach (var data in datas)
        {
            if (data.Contains("NaN"))
            {
                newDatas.RemoveAt(iter);
                continue;
            }
            iter++;
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

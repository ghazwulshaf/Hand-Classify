using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class ANNClassification1Layer
{
    /******************************
    ** CLASS FIELDS
    *******************************/
    
    // data
    public int featureCount;
    public int labelCount;
    public double[,] inputTrain;
    public double[,] inputTest;
    public List<string> singleLabels;
    public double[,] labelTrain;
    public double[,] labelTest;
    public string[] labelTestLing;
    public double[] lossArr;
    public string[] predictedLabels;
    
    // node
    public int nodeIn;
    public int nodeH;
    public int nodeOut;

    // weight
    public double[,] wH;
    public double[,] wOut;

    // bias
    public double[] bH;
    public double[] bOut;

    // hyperparameters
    public int epoch;
    public double loss;
    public double accuracy;
    public double time;
    public double alpha;
    public double beta1;
    public double beta2;
    public double epsilon;

    // private
    private NumberFormatInfo provider;


    /******************************
    ** INIT OBJECT
    *******************************/
    
    public ANNClassification1Layer(int _featureNum, int _labelNum, int _hiddenNode)
    {
        // init node
        featureCount = _featureNum;
        labelCount = _labelNum;
        nodeIn = featureCount;
        nodeH = _hiddenNode;
        nodeOut = labelCount;

        // init weight shape
        wH = new double[nodeIn, nodeH];
        wOut = new double[nodeH, nodeOut];

        // init weight
        InitWeight(wH);
        InitWeight(wOut);

        // init bias shape
        bH = new double[nodeH];
        bOut = new double[nodeOut];

        // init bias
        InitBias(bH);
        InitBias(bOut);

        // init hyperparameters
        alpha = 0.001;
        beta1 = 0.9;
        beta2 = 0.999;
        epsilon = 1E-8;
    }


    /******************************
    ** ADD. METHODS FOR INIT OBJECT
    *******************************/

    private void InitWeight(double[,] weight)
    {
        int row = weight.GetLength(0);
        int col = weight.GetLength(1);

        var rand = new Random(0);

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                weight[i,j] = rand.NextDouble();
            }
        }
    }

    private void InitBias(double[] bias)
    {
        int length = bias.Length;

        var rand = new Random(0);

        for (int i = 0; i < length; i++)
        {
            bias[i] = rand.NextDouble();
        }
    }

    public void SetParam(string _target, double[,] _source)
    {
        // weight
        switch (_target)
        {
            case "weight_hidden":
                wH = _source;
                break;
            
            case "weight_output":
                wOut = _source;
                break;
        }
    }

    public void SetParam(string _target, double[] _source)
    {
        // bias
        switch (_target)
        {
            case "bias_hidden":
                bH = _source;
                break;
            
            case "bias_output":
                bOut = _source;
                break;
        }
    }


    /******************************
    ** CLASS METHODS
    *******************************/

    public async Task Train(double[,] _inputTrain, string[] _labelTrain, int _epoch_lim, double _loss_lim)
    {
        // get time
        var last = DateTime.Now;

        // init data train
        inputTrain = _inputTrain;
        labelTrain = OneHotEncoder(_labelTrain);

        int _epoch = 1;
        double _loss = 1;

        await Task.Run(() => 
        {
            // train while epoch < epoch_lim and loss > loss_lim
            while (_epoch < _epoch_lim + 1 && _loss > _loss_lim)
            {
                
                /* FEED FORWARD */

                // hidden layer
                var zH = AddArray(Dot(inputTrain, wH), bH);
                var aH = new double[zH.GetLength(0), zH.GetLength(1)];
                for (int i = 0; i < zH.GetLength(0); i++)
                {
                    for (int j = 0; j < zH.GetLength(1); j++)
                    {
                        aH[i,j] = ReLu(zH[i,j]);
                    }
                }

                // output layer
                var zOut = AddArray(Dot(aH, wOut), bOut);
                var aOut = new double[zOut.GetLength(0), zOut.GetLength(1)];
                for (int i = 0; i < zOut.GetLength(0); i++)
                {
                    var singleArr = new double[zOut.GetLength(1)];
                    for (int j = 0; j < zOut.GetLength(1); j++)
                    {
                        singleArr[j] = zOut[i,j];
                    }

                    var sofmaxVal = Softmax(singleArr);
                    for (int j = 0; j < zOut.GetLength(1); j++)
                    {
                        aOut[i,j] = sofmaxVal[j];
                    }
                }

                // loss
                lossArr = CategoricalCrossEntropy(labelTrain, aOut);
                _loss = lossArr.Sum() / lossArr.Length;
                loss = _loss;

                // print loss
                if (_epoch % 100 == 0)
                    UnityEngine.Debug.Log($"Epoch: {_epoch} | Loss: {_loss}");

                
                /* BACK PROPAGATION */

                // output layer
                var dcost_dzout = Transpose(Sub(aOut, labelTrain));
                var dzout_dwout = aH;
                var dcost_dwout = Dot(dcost_dzout, dzout_dwout);
                var dcost_dbout = new double[dcost_dzout.GetLength(0)];
                for (int i = 0; i < dcost_dzout.GetLength(0); i++)
                {
                    var singleArr = new double[dcost_dzout.GetLength(1)];
                    for (int j = 0; j < dcost_dzout.GetLength(1); j++)
                    {
                        singleArr[j] = dcost_dzout[i,j];
                    }
                    dcost_dbout[i] = singleArr.Sum();
                }

                // hidden layer
                var dzout_dah = wOut;
                var dah_dzh = new double[zH.GetLength(0), zH.GetLength(1)];
                for (int i = 0; i < zH.GetLength(0); i++)
                {
                    for (int j = 0; j < zH.GetLength(1); j++)
                    {
                        dah_dzh[i,j] = ReLuDerivative(zH[i,j]);
                    }
                }
                var dzh_dwh = inputTrain;
                var dcost_dah = Dot(dzout_dah, dcost_dzout);
                var dcost_dzh = Multiply(dcost_dah, Transpose(dah_dzh));
                var dcost_dwh = Dot(dcost_dzh, dzh_dwh);
                var dcost_dbh = new double[dcost_dzh.GetLength(0)];
                for (int i = 0; i < dcost_dzh.GetLength(0); i++)
                {
                    var singleArr = new double[dcost_dzh.GetLength(1)];
                    for (int j = 0; j < dcost_dzh.GetLength(1); j++)
                    {
                        singleArr[j] = dcost_dzh[i,j];
                    }
                    dcost_dbh[i] = singleArr.Sum();
                }

                
                /* OPTIMIZATION */

                // hidden layer
                ADAMWeight(wH, dcost_dwh);
                ADAMBias(bH, dcost_dbh);

                // output layer
                ADAMWeight(wOut, dcost_dwout);
                ADAMBias(bOut, dcost_dbout);

                // update epoch
                epoch = _epoch;
                _epoch++;
            }
            
            // get time
            var current = DateTime.Now;
            time = (current - last).TotalSeconds;
        });
    }

    public async Task Validate(double[,] _inputTest, string[] _labelTest)
    {
        // init data test
        inputTest = _inputTest;
        labelTest = OneHotEncoder(_labelTest);
        labelTestLing = _labelTest;

        predictedLabels = new string[labelTestLing.Length];
        int predictedTrue = 0;

        await Task.Run(() =>
        {
            for (int i = 0; i < inputTest.GetLength(0); i++)
            {
                var input = new double[inputTest.GetLength(1)];
                for (int j = 0; j < inputTest.GetLength(1); j++)
                {
                    input[j] = inputTest[i,j];
                }

                predictedLabels[i] = Predict(input);
                if (predictedLabels[i] == labelTestLing[i])
                {
                    predictedTrue++;
                }
            }
        });

        accuracy = 100.0 * predictedTrue / predictedLabels.Length;
    }

    public string Predict(double[] input)
    {
        var newInput = ArrayToMatrix("row", input);

        var zH = AddArray(Dot(newInput, wH), bH);
        var aH = new double[zH.GetLength(0), zH.GetLength(1)];
        for (int i = 0; i < zH.GetLength(0); i++)
        {
            for (int j = 0; j < zH.GetLength(1); j++)
            {
                aH[i,j] = ReLu(zH[i,j]);
            }
        }

        var zOut = AddArray(Dot(aH, wOut), bOut);
        var new_zOut = MatrixToArray(zOut);
        
        var sofmaxVal = Softmax(new_zOut);
        var argmaxVal = Argmax(sofmaxVal);
        var aOut = argmaxVal.ToList();

        var index = 0;
        for (int i = 0; i < argmaxVal.Length; i++)
        {
            if (argmaxVal[i] == 1)
            {
                index = i;
            }
        }
        var predictedLabel = singleLabels[index];

        return predictedLabel;
    }


    /******************************
    ** ADDITIONAL METHODS
    *******************************/

    // Shuffle Algorithm: Fisher-Yates
    public List<string> FisherYates(List<string> _datas)
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

    // Label Encoding: One Hot Encoder
    private double[,] OneHotEncoder(string[] _labels)
    {
        string[] sortLabels = new string[_labels.Length];
        for (int i = 0; i < _labels.Length; i++)
        {
            sortLabels[i] = _labels[i];
        }
        Array.Sort(sortLabels);

        string label = "";
        singleLabels = new List<string>();
        foreach (var item in sortLabels)
        {
            if (label != item)
            {
                singleLabels.Add(item);
                label = item;
            }
        }

        var dictLabels = new Dictionary<string, double[]>();

        for (int i = 0; i < singleLabels.Count; i++)
        {
            var oneHot = new double[singleLabels.Count];
            for (int j = 0; j < oneHot.Length; j++)
            {
                if (j == i)
                {
                    oneHot[j] = 1;
                }
                else
                {
                    oneHot[j] = 0;
                }
            }
            dictLabels.Add(singleLabels[i], oneHot);
        }

        var newLabels = new double[_labels.Length, singleLabels.Count];
        for (int i = 0; i < _labels.Length; i++)
        {
            var oneHotLabel = dictLabels[_labels[i]];
            for (int j = 0; j < oneHotLabel.Length; j++)
            {
                newLabels[i,j] = oneHotLabel[j];
            }
        }

        return newLabels;
    }

    // Activation Function: ReLu
    private double ReLu(double x)
    {
        return System.Math.Max(0, x);
    }

    // Activation Function: Argmax
    private double[] Argmax(double[] array)
    {
        var result = new double[array.Length];
        double maxNum = array.Max();

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == maxNum)
            {
                result[i] = 1;
            }
            else
            {
                result[i] = 0;
            }
        }

        return result;
    }

    // Activation Function: Softmax
    private double[] Softmax(double[] array)
    {
        var result = new double[array.Length];
        var arrayExp = new double[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            arrayExp[i] = Math.Exp(array[i]);
        }

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = arrayExp[i] / arrayExp.Sum();
        }

        return result;
    }

    // Derivative Activation Function: ReLu Derivative
    private double ReLuDerivative(double x)
    {
        if (x < 0)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    // Loss Function: Categorical Cross Entropy
    private double[] CategoricalCrossEntropy(double[,] actualVal, double[,] predictVal)
    {
        var result = new double[predictVal.GetLength(0)];
        for (int i = 0; i < predictVal.GetLength(0); i++)
        {
            var cce = new double[predictVal.GetLength(1)];
            for (int j = 0; j < predictVal.GetLength(1); j++)
            {
                cce[j] = actualVal[i,j] * Math.Log(predictVal[i,j]);
            }

            result[i] = -cce.Sum();
        }

        return result;
    }

    // Optimization: ADAM for weight
    private void ADAMWeight(double[,] w, double[,] w_der)
    {
        var dw = Transpose(w_der);
        double m1 = 0;
        double m2 = 0;

        for (int i = 0; i < w.GetLength(0); i++)
        {
            for (int j = 0; j < w.GetLength(1); j++)
            {
                var gt = dw[i,j];
                m1 = beta1 * m1 + (1 - beta1) * gt;
                m2 = beta2 * m2 + (1 - beta2) * Math.Pow(gt, 2);

                var m1_hat = m1 / (1 - beta1);
                var m2_hat = m2 / (1 - beta2);

                w[i,j] = w[i,j] - (alpha * m1_hat / (Math.Sqrt(m2_hat) + epsilon));
            }
        }
    }

    // Optimization: ADAM for bias
    private void ADAMBias(double[] b, double[] b_der)
    {
        var db = b_der;
        double m1 = 0;
        double m2 = 0;

        for (int i = 0; i < b.Length; i++)
        {
            var gt = db[i];
            m1 = beta1 * m1 + (1 - beta1) * gt;
            m2 = beta2 * m2 + (1 - beta2) * System.Math.Pow(gt, 2);

            var m1_hat = m1 / (1 - beta1);
            var m2_hat = m2 / (1 - beta2);

            b[i] = b[i] - (alpha * m1_hat / (System.Math.Sqrt(m2_hat) + epsilon));
        }
    }

    // Matrix: Add Array
    private double[,] AddArray(double[,] matrix, double[] array)
    {
        double[,] result;

        int row = matrix.GetLength(0);
        int col = matrix.GetLength(1);

        if (array.Length == col)
        {
            result = new double[row, col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    result[i,j] = matrix[i,j] + array[j];
                }
            }

            return result;
        }
        else
        {
            throw new Exception("The matrix cannot be added with the array.");
        }
    }

    // Matrix: Sub
    private double[,] Sub(double[,] matrix1, double[,] matrix2)
    {
        double[,] result;

        int matrix1Row = matrix1.GetLength(0);
        int matrix1Col = matrix1.GetLength(1);
        int matrix2Row = matrix2.GetLength(0);
        int matrix2Col = matrix2.GetLength(1);

        if (matrix1Row == matrix2Row && matrix1Col == matrix2Col)
        {
            int row = matrix1Row;
            int col = matrix1Col;

            result = new double[row, col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    result[i,j] = matrix1[i,j] - matrix2[i,j];
                }
            }

            return result;
        }
        else
        {
            throw new Exception("The matrixs cannot be subtracted.");
        }
    }

    // Matrix: Multiply
    private double[,] Multiply(double[,] matrix1, double[,] matrix2)
    {
        double[,] result;

        int matrix1Row = matrix1.GetLength(0);
        int matrix1Col = matrix1.GetLength(1);
        int matrix2Row = matrix2.GetLength(0);
        int matrix2Col = matrix2.GetLength(1);

        if (matrix1Row == matrix2Row && matrix1Col == matrix2Col)
        {
            int row = matrix1Row;
            int col = matrix1Col;

            result = new double[row, col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    result[i,j] = matrix1[i,j] * matrix2[i,j];
                }
            }

            return result;
        }
        else
        {
            throw new Exception("The matrixs cannot be multiply.");
        }
    }

    // Matrix: Dot Product
    private double[,] Dot(double[,] matrix1, double[,] matrix2)
    {
        double[,] result;

        int matrix1Row = matrix1.GetLength(0);
        int matrix1Col = matrix1.GetLength(1);
        int matrix2Row = matrix2.GetLength(0);
        int matrix2Col = matrix2.GetLength(1);

        if (matrix1Col == matrix2Row)
        {
            result = new double[matrix1Row, matrix2Col];

            for (int i = 0; i < matrix1Row; i++)
            {
                for (int j = 0; j < matrix2Col; j++)
                {
                    result[i,j] = 0;

                    for (int k = 0; k < matrix1Col; k++)
                    {
                        result[i,j] += matrix1[i,k] * matrix2[k,j];
                    }
                }
            }

            return result;
        }
        else
        {
            throw new Exception("The first and the second matrix can't process with dot product. The first matrix's row and the second matrix's column aren't same");
        }
    }

    // Matrix: Transpose
    private double[,] Transpose(double[,] matrix)
    {
        double[,] result;

        int row = matrix.GetLength(0);
        int col = matrix.GetLength(1);

        result = new double[col, row];
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                result[i,j] = matrix[j,i];
            }
        }

        return result;
    }

    // Matrix: Array to Matrix
    private double[,] ArrayToMatrix(string type, double[] array)
    {
        var result = new double[0,0];
        var length = array.Length;

        if (type == "row")
        {
            result = new double[1, length];

            for (int i = 0; i < length; i++)
            {
                result[0,i] = array[i];
            }
        }

        else if (type == "col")
        {
            result = new double[length, 1];

            for (int i = 0; i < length; i++)
            {
                result[i,0] = array[i];
            }
        }

        return result;
    }

    // Matrix: Matrix to Array
    private double[] MatrixToArray(double[,] matrix)
    {
        var list = new List<double>();
        foreach (var item in matrix)
        {
            list.Add(item);
        }

        var result = list.ToArray();
        return result;
    }

}

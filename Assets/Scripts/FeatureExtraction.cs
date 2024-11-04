using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FeatureExtraction
{

    /******************************
    ** METHODS
    *******************************/

    // Integration of absolute of EMG signal
    public double IEMG(List<double> datas, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Abs(dat);
        }
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> IEMG(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var iemg = IEMG(dataRange, num);
            newDatas.Add(iemg);
        }

        return newDatas;
    }

    // Mean
    public double ME(List<double> datas, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += dat;
        }
        data /= datas.Count;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> ME(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var me = ME(dataRange, num);
            newDatas.Add(me);
        }

        return newDatas;
    }

    // Mean Absolute Value
    public double MAV(List<double> datas, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Abs(dat);
        }
        data /= datas.Count;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> MAV(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var mav = MAV(dataRange, num);
            newDatas.Add(mav);
        }

        return newDatas;
    }

    // Root Mean Squared
    public double RMS(List<double> datas, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Pow(dat, 2);
        }
        data = Math.Sqrt(data);
        data /= datas.Count;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> RMS(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var rms = RMS(dataRange, num);
            newDatas.Add(rms);
        }

        return newDatas;
    }

    // Wave Length
    public double WL(List<double> datas, int? num = null)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += Math.Abs(datas[j+1] - datas[j]);
        }
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> WL(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var wl = WL(dataRange, num);
            newDatas.Add(wl);
        }

        return newDatas;
    }

    // Difference Absolute Mean Value
    public double DAMV(List<double> datas)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += Math.Abs(datas[j+1] - datas[j]);
        }
        data /= datas.Count - 1;

        return data;
    }
    public List<double> DAMV(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var damv = DAMV(dataRange);
            newDatas.Add(damv);
        }

        return newDatas;
    }

    // Variance
    public double VAR(List<double> datas, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Pow(dat, 2);
        }
        data /= datas.Count - 1;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> VAR(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var data = VAR(dataRange, num);
            newDatas.Add(data);
        }

        return newDatas;
    }

    // Zero Crossings
    public double ZC(List<double> datas)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += SgnZC(- datas[j] * datas[j+1]);
        }

        return data;
    }
    public List<double> ZC(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var zc = ZC(dataRange);
            newDatas.Add(zc);
        }

        return newDatas;
    }

    // Zero Crossings with Threshold
    public double ZCT(List<double> datas, double threshold)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += SgnZCT(datas[j], datas[j+1], threshold);
        }

        return data;
    }
    public List<double> ZCT(List<double> datas, int length, double threshold)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var zct = ZCT(dataRange, threshold);
            newDatas.Add(zct);
        }

        return newDatas;
    }

    // Wilson Amplitude
    public double WA(List<double> datas, double threshold, int? num = null)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += FunWA(Math.Abs(datas[j] - datas[j+1]), threshold);
        }
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> WA(List<double> datas, int length, double threshold, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var wa = WA(dataRange, threshold, num);
            newDatas.Add(wa);
        }

        return newDatas;
    }

    // Slope Sign Change
    public double SSC(List<double> datas, int? num = null)
    {
        double data = 0;
        for (int j = 1; j < datas.Count - 2; j++)
        {
            data += FunSSC((datas[j+1] - datas[j]) * (datas[j] - datas[j-1]));
        }
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> SSC(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var ssc = SSC(dataRange, num);
            newDatas.Add(ssc);
        }

        return newDatas;
    }

    // Slope Sign Change with Threshold
    public double SSCT(List<double> datas, double threshold)
    {
        double data = 0;
        for (int j = 1; j < datas.Count - 2; j++)
        {
            data += FunSSCT(datas[j-1], datas[j], datas[j+1], threshold);
        }
        data = Simplification(data, 1);

        return data;
    }
    public List<double> SSCT(List<double> datas, int length, double threshold)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var ssct = SSCT(dataRange, threshold);
            newDatas.Add(ssct);
        }

        return newDatas;
    }

    // Simple Square Integral
    public double SSI(List<double> datas, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Pow(dat, 2);
        }
        data /= datas.Count;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> SSI(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var ssi = SSI(dataRange, num);
            newDatas.Add(ssi);
        }

        return newDatas;
    }

    // Mean Absolute Value Slope
    public double MAVS(List<double> datas)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += Math.Abs(datas[j+1] - datas[j]);
        }
        data /= datas.Count;

        return data;
    }
    public List<double> MAVS(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var mavs = MAVS(dataRange);
            newDatas.Add(mavs);
        }

        return newDatas;
    }

    // Skewness
    public double SKEW(List<double> datas)
    {
        var mean = Mean(datas);
        var deviation = Deviation(datas);
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Pow((dat - mean) / deviation, 3);
        }
        data *= datas.Count / (datas.Count - 1) * (datas.Count - 2);

        return data;
    }
    public List<double> SKEW(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var skew = SKEW(dataRange);
            newDatas.Add(skew);
        }

        return newDatas;
    }

    // Skewness from Paper
    public double SKEWNESS(List<double> datas)
    {
        var mean = Mean(datas);
        var deviation = Deviation(datas);
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Pow(dat - mean, 3);
        }
        data /= (datas.Count - 1) * Math.Pow(deviation, 3);

        return data;
    }
    public List<double> SKEWNESS(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var skewness = SKEWNESS(dataRange);
            newDatas.Add(skewness);
        }

        return newDatas;
    }

    // Log Detector
    public double LD(List<double> datas)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Abs(dat);
        }
        data /= datas.Count;
        data = Math.Log(data);

        return data;
    }
    public List<double> LD(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var ld = LD(dataRange);
            newDatas.Add(ld);
        }

        return newDatas;
    }

    // Log Detector from Paper
    public double LOG(List<double> datas, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Log(Math.Abs(dat));
        }
        data /= datas.Count;
        data = Math.Exp(data);
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> LOG(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var log = LOG(dataRange, num);
            newDatas.Add(log);
        }

        return newDatas;
    }

    // Log Difference Absolute Mean Value
    public double LDAMV(List<double> datas)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += Math.Abs(datas[j+1] - datas[j]);
        }
        data /= datas.Count - 1;
        data = Math.Log(data);

        return data;
    }
    public List<double> LDAMV(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var ldamv = LDAMV(dataRange);
            newDatas.Add(ldamv);
        }

        return newDatas;
    }

    // Log Teager-Kaiser Energy Operator
    public double LTKEO(List<double> datas)
    {
        double data = 0;
        for (int j = 1; j < datas.Count - 1; j++)
        {
            data += Math.Pow(datas[j], 2) - datas[j-1] * datas[j+1];
        }
        data /= datas.Count - 2;
        data = Math.Log(data);

        return data;
    }
    public List<double> LTKEO(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var ltkeo = LTKEO(dataRange);
            newDatas.Add(ltkeo);
        }

        return newDatas;
    }

    // Standard Deviation
    public List<double> SD(List<double> datas, int length)
    {
        var newDatas = new List<double>();

        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var mean = Mean(dataRange);
            double data = 0;
            foreach (var dat in dataRange)
            {
                data += Math.Pow(dat - mean, 2);
            }
            data /= length - 1;
            data = Math.Sqrt(data);
            newDatas.Add(data);
        }

        return newDatas;
    }

    // Myopulse Percentage Rate
    public double MYOP(List<double> datas, double threshold)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += FunMYOP(dat, threshold);
        }
        data /= datas.Count;

        return data;
    }
    public List<double> MYOP(List<double> datas, int length, double threshold)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var myop = MYOP(dataRange, threshold);
            newDatas.Add(myop);
        }

        return newDatas;
    }

    // Maximum Fractal Length
    public double MFL(List<double> datas)
    {
        double data = 0;
        for (int j = 0; j < datas.Count - 1; j++)
        {
            data += Math.Pow(datas[j+1] - datas[j], 2);
        }
        data = Math.Log(Math.Sqrt(data));

        return data;
    }
    public List<double> MFL(List<double> datas, int length)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var mfl = MFL(dataRange);
            newDatas.Add(mfl);
        }

        return newDatas;
    }

    // Modified Mean Absolute Value
    public double MMAV(List<double> datas, int? num = null)
    {
        double data = 0;
        for (int j = 0; j < datas.Count; j++)
        {
            data += FunWMMAV(j, datas.Count) * Math.Abs(datas[j]);
        }
        data /= datas.Count;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> MMAV(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var mmav = MMAV(dataRange, num);
            newDatas.Add(mmav);
        }

        return newDatas;
    }

    // Mean Absolute Deviation
    public double MAD(List<double> datas, int? num = null)
    {
        var mean = Mean(datas);
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Abs(dat - mean);
        }
        data /= datas.Count;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> MAD(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var mad = MAD(dataRange, num);
            newDatas.Add(mad);
        }

        return newDatas;
    }

    // V-Order
    public double VO(List<double> datas, int order, int? num = null)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Pow(dat, order);
        }
        data /= datas.Count;
        data = Math.Pow(data, 1.0/order);
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> VO(List<double> datas, int length, int order, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var vo = VO(dataRange, order, num);
            newDatas.Add(vo);
        }

        return newDatas;
    }

    // Average Amplitude Change
    public double AAC(List<double> datas, int? num = null)
    {
        double data = 0;
        for (int i = 0; i < datas.Count - 1; i++)
        {
            data += Math.Abs(datas[i+1] - datas[i]);
        }
        data /= datas.Count;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> AAC(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var aac = AAC(dataRange, num);
            newDatas.Add(aac);
        }

        return newDatas;
    }

    // Difference Mean Absolute Value
    public double DMAV(List<double> datas, int? num = null)
    {
        double data = 0;
        for (int i = 0; i < datas.Count - 1; i++)
        {
            data += Math.Abs(datas[i+1] - datas[i]);
        }
        data /= datas.Count - 1;
        if (num != null)
            data = Simplification(data, (int)num);

        return data;
    }
    public List<double> DMAV(List<double> datas, int length, int? num = null)
    {
        var newDatas = new List<double>();
        for (int i = 0; i < datas.Count; i += length)
        {
            var dataRange = datas.GetRange(i, length);
            var dmav = DMAV(dataRange, num);
            newDatas.Add(dmav);
        }

        return newDatas;
    }


    /******************************
    ** ADD. METHODS
    *******************************/

    // Add Method: Signal Zero Crossing
    private int SgnZC(double x)
    {
        return x > 0 ? 1 : 0;
    }

    // Add Method: Signal Zero Crossing with Threshold
    private int SgnZCT(double x1, double x2, double t)
    {
        return ((x1 > 0 && x2 < 0) || (x1 < 0 && x2 > 0)) && Math.Abs(x1 - x2) >= t ? 1 : 0;
    }

    // Add Method: Function Willison Amplitude
    private int FunWA(double x, double t)
    {
        return x >= t ? 1 : 0;
    }

    // Add Method: Function Slope Sign Change
    private int FunSSC(double x)
    {
        return x < 0 ? 1 : 0;
    }
    
    // Add Method: Function Slope Sign Change with Threshold
    private int FunSSCT(double x0, double x1, double x2, double t)
    {
        return ((x1 > x0 && x1 > x2) || (x1 < x0 && x1 < x2)) && ((Math.Abs(x1 - x2) >= t) || (Math.Abs(x1 - x0) >= t)) ? 1 : 0;
    }

    // Add Method: Function Weight Modified Mean Absolute Value
    private double FunWMMAV(int i, int n)
    {
        if (i >= 0.25 * n && i <= 0.75 * n)
            return 1;
        else if (i < n * 0.25)
            return 4 * i / n;
        else
            return 4 * (i - n) / n;
    }

    // Add Method: Function Myopulse Percentage Rate
    private int FunMYOP(double x, double t)
    {
        return Math.Abs(x) > t ? 1 : 0;
    }

    // Add Method: Mean
    private double Mean(List<double> datas)
    {
        double data = 0;
        foreach (var dat in datas)
        {
            data += dat;
        }
        data /= datas.Count;

        return data;
    }

    // Add Method: Standard Deviation
    public double Deviation(List<double> datas)
    {
        var mean = Mean(datas);
        double data = 0;
        foreach (var dat in datas)
        {
            data += Math.Pow(dat - mean, 2);
        }
        data /= datas.Count - 1;
        data = Math.Sqrt(data);

        return data;
    }
    
    // Simplification Datas
    private double Simplification(double data, int num)
    {
        double newVal = data / Math.Pow(10, num);

        return newVal;
    }
    private List<double> Simplification(List<double> datas, int num)
    {
        var newDatas = new List<double>();
        foreach(var data in datas)
        {
            double newVal = data / Math.Pow(10, num);
            newDatas.Add(newVal);
        }

        return newDatas;
    }
    private List<double> Simplification(List<double> datas)
    {
        var newDatas = new List<double>();
        double maxVal = datas.Max();
        int logVal = Convert.ToInt32(Math.Log10(maxVal));
        foreach(var data in datas)
        {
            double newVal = data / Math.Pow(10, logVal);
            newDatas.Add(newVal);
        }

        return newDatas;
    }

}

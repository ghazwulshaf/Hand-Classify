using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMGData : MonoBehaviour
{
    public static double value;
    public static bool isStreaming = false;
    public static List<double> DataStream = new();
}

using UnityEngine.Android;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class BluetoothManager : MonoBehaviour
{
    // ui
    public Text dataToSend;
    public Text display;
    public Text deviceAdd;
    public Text connectionStatus;

    public Text sensorObj;
    public Text electrodeObj;
    public Text positionObj;
    public Text dataGroupObj;
    public Text dataSizeObj;
    public Text dataModelObj;
    public Text recordProgress;

    public InputField dataReceived;

    // field
    private bool isConnected;
    private bool isReading;
    private bool isStreaming;

    // data handler
    public Text feLength;
    private List<string> datas;
    private string dataSensor;
    private int dataSize;
    private int dataIter;
    private List<double> dataStream;

    // method
    private static AndroidJavaClass unity3dbluetoothplugin;
    private static AndroidJavaObject BluetoothConnector;
    private CSVHandler CSVHandler;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // init method
        InitBluetooth();
        CSVHandler = new CSVHandler();

        // init field
        isConnected = false;
        isReading = false;
        isStreaming = false;
    }

    // creating an instance of the bluetooth class from the plugin 
    public void InitBluetooth()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        // Check BT and location permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
            || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADMIN")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
        {
            Permission.RequestUserPermissions(new string[] {
                Permission.CoarseLocation,
                Permission.FineLocation,
                "android.permission.BLUETOOTH_ADMIN",
                "android.permission.BLUETOOTH",
                "android.permission.BLUETOOTH_SCAN",
                "android.permission.BLUETOOTH_ADVERTISE",
                "android.permission.BLUETOOTH_CONNECT"
            });
        }

        unity3dbluetoothplugin = new AndroidJavaClass("com.example.unity3dbluetoothplugin.BluetoothConnector");
        BluetoothConnector = unity3dbluetoothplugin.CallStatic<AndroidJavaObject>("getInstance");
    }

    // Start device scan
    public void StartScanDevices()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        // do something

        BluetoothConnector.CallStatic("StartScanDevices");
    }

    // Stop device scan
    public void StopScanDevices()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        BluetoothConnector.CallStatic("StopScanDevices");
    }

    // This function will be called by Java class to update the scan status,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ScanStatus(string status)
    {
        Toast("Scan Status: " + status);
    }

    // This function will be called by Java class whenever a new device is found,
    // and delivers the new devices as a string data="MAC+NAME"
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void NewDeviceFound(string data)
    {
        // do something
    }

    // Get paired devices from BT settings
    public void GetPairedDevices()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        // This function when called returns an array of PairedDevices as "MAC+Name" for each device found
        string[] data = BluetoothConnector.CallStatic<string[]>("GetPairedDevices"); ;

        // do something
    }

    public void ConnectDevice()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        

        StartConnection();
    }

    // Start BT connect using device MAC address "deviceAdd"
    public void StartConnection()
    {
        display.text += $"Wait for connecting to BT Device {deviceAdd.text}...\n";
        BluetoothConnector.CallStatic("StartConnection", deviceAdd.text.ToUpper());

        if (isConnected)
            display.text += "Device BT connect success\n";
        else
            display.text += "Device BT connect failed\n";
    }

    public void DisconnectDevice()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        
        if (isConnected)
            StopConnection();
    }

    // Stop BT connetion
    public void StopConnection()
    {
        display.text += "Wair for device BT disconnecting...";
        BluetoothConnector.CallStatic("StopConnection");

        if (!isConnected)
            display.text += "Device BT disconnect success\n";
        else
            display.text += "Device BT disconnect failed\n";
    }

    // This function will be called by Java class to update BT connection status,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ConnectionStatus(string status)
    {
        Toast("Connection Status: " + status);
        isConnected = status == "connected";
        connectionStatus.text = status;
    }

    // Set sensor
    public void SetSensor()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        if (isConnected)
        {
            isStreaming = false;
            string sensor = sensorObj.text;
            BluetoothConnector.CallStatic("WriteData", "#" + sensor + "#\n");
            display.text += "Change sensor to: " + sensor + "\n";
            dataSensor = sensor;
        }
    }

    // Read data from connected BT
    public void StartRead()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        if (!isStreaming)
        {
            try
            {
                string sensorVal = sensorObj.text;

                datas = new List<string>();
                dataSensor = sensorVal;
                dataSize = 15000;
                dataIter = 0;

                if (isConnected)
                {
                    display.text += "Wait for reading...\n";

                    isReading = true;
                    BluetoothConnector.CallStatic("WriteData", "#Start#\n");
                }
            }
            catch (Exception ex)
            {
                display.text += ex.Message + "\n";
            }
        }
    }

    // Stop read data from connected BT
    public void StopRead()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        if (isConnected)
        {
            if (isReading)
            {
                isReading = false;
                BluetoothConnector.CallStatic("WriteData", "#Stop#\n");
                display.text += "Data reading stopped\n";
            }
        }
    }

    // Stream data from connected BT
    public void StartStream()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
            
        if (isConnected)
        {
            if (!isReading)
            {
                dataStream = new();
                isStreaming = true;
                EMGData.isStreaming = true;
                BluetoothConnector.CallStatic("WriteData", "#Start#\n");
            }
        }
    }

    // Stop data streaming
    public void StopStream()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        
        if (isConnected)
        {
            if (isStreaming)
            {
                isStreaming = false;
                EMGData.isStreaming = false;
                BluetoothConnector.CallStatic("WriteData", "#Stop#\n");
            }
        }
    }

    // This function will be called by Java class whenever BT data is received,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ReadData(string data)
    {
        Debug.Log("BT Stream: " + data);
        
        // data reading
        if (isReading)
        {
            datas.Add(data);
            dataIter++;
            float recordProgressVal = (float) dataIter / (float) dataSize;
            recordProgress.text = recordProgressVal.ToString();

            if (dataIter == dataSize)
            {
                isReading = false;
                BluetoothConnector.CallStatic("WriteData", "#Stop#\n");
                display.text += "Sensor data reading completed\n";
                StoreData();
            }
        }

        // data streaming
        if (isStreaming)
        {
            // dataReceived.text = data;
            dataStream.Add(double.Parse(data));
            if (dataStream.Count == int.Parse(feLength.text))
            {
                EMGData.DataStream = dataStream;
                dataStream.Clear();
            }
        }
    }

    // Write data to the connected BT device
    public void WriteData()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        if (isConnected)
            BluetoothConnector.CallStatic("WriteData", dataToSend.text.ToString());
    }

    // Store data to csv file
    public void StoreData()
    {
        display.text += "Wait for data storing...\n";
        try
        {
            string dataElectrode = electrodeObj.text;
            string dataPosition = positionObj.text;
            string dataModel = dataModelObj.text;
            string dataGroup = dataGroupObj.text;
            string path = Application.persistentDataPath + "/EMG_Raw_" + dataElectrode + "_" + dataPosition + "_" + dataGroup + "_" + dataModel + ".csv";
            CSVHandler.WriteCSV(path, datas, true);
            display.text += "Recording data " + dataModel + " completed\n";
        }
        catch (Exception ex)
        {
            display.text += ex.Message + "\n";
        }
    }

    // This function will be called by Java class to send Log messages,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ReadLog(string data)
    {
        Debug.Log(data);
    }


    // Function to display an Android Toast message
    public void Toast(string data)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        BluetoothConnector.CallStatic("Toast", data);
    }
}

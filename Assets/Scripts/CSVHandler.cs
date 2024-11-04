using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

public class CSVHandler
{
    public string _headers;

    private NumberFormatInfo provider;

    public CSVHandler()
    {
        provider = new NumberFormatInfo
        {
            NumberGroupSeparator = ",",
            NumberDecimalSeparator = "."
        };
    }

    public List<string> ReadCSV(string path, bool header = false)
    {
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        var datas = new List<string>();

        using (var reader = new StreamReader(stream))
        {
            if (header)
            {
                _headers = reader.ReadLine();
            }

            bool endOfFile = false;
            while (!endOfFile)
            {
                string dataString = reader.ReadLine();
                
                if (dataString == null)
                {
                    endOfFile = true;
                    break;
                }
                datas.Add(dataString);
            }
        }

        return datas;
    }

    public async Task ReadCSVFile(string path, List<string> datas, bool header = false)
    {
        await Task.Run(() =>
        {
            StreamReader streamReader = new StreamReader(path);

            if (header)
            {
                _headers = streamReader.ReadLine();
            }

            bool endOfFile = false;
            while (!endOfFile)
            {
                string dataString = streamReader.ReadLine();
                
                if (dataString == null)
                {
                    endOfFile = true;
                    break;
                }
                datas.Add(dataString);
            }
        });
    }

    public void WriteCSV(string path, List<string> datas, bool newFile = false, bool header = false, string headers = null)
    {
        FileStream stream;

        if (newFile)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        }
        else
            stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            if (header)
            {
                writer.WriteLine(headers);
            }

            foreach (var data in datas)
            {
                writer.WriteLine(data);
            }
        }
    }

    public void WriteCSV(string path, string[] datas, bool newFile = false, bool header = false, string[] headers = null)
    {
        FileStream stream;

        if (newFile)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
        }
        else
            stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            if (header)
            {
                string head = "";
                for (int i = 0; i < headers.Length; i++)
                {
                    head += headers[i];
                    if (i != headers.Length - 1)
                    {
                        head += ",";
                    }
                }
                writer.WriteLine(head);
            }

            string text = "";
            for (int i = 0; i < datas.Length; i++)
            {
                text += datas[i];
                if (i != datas.Length - 1)
                {
                    text += ",";
                }
            }
            writer.WriteLine(text);
        }
    }

    public void WriteCSV(string path, List<double> datas, bool newFile = false, bool header = false, string headers = null)
    {
        FileStream stream;

        if (newFile)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
        }
        else
            stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            if (header)
            {
                writer.WriteLine(headers);
            }

            foreach (var data in datas)
            {
                writer.WriteLine(data.ToString(provider));
            }
        }
    }

    public void WriteCSV(string path, string[,] datas, bool newFile = false, bool header = false, string[] headers = null)
    {
        FileStream stream;

        if (newFile)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
        }
        else
            stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

        var row = datas.GetLength(0);
        var col = datas.GetLength(1);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            if (header)
            {
                var length = headers.Length;
                string text = "";
                for (int i = 0; i < length; i++)
                {
                    text += headers[i];
                    if (i != length - 1)
                    {
                        text += ",";
                    }
                }
                writer.WriteLine(text);
            }

            for (int i = 0; i < row; i ++)
            {
                string text = "";
                for (int j = 0; j < col; j++)
                {
                    text += datas[i,j].ToString(provider);
                    if (j != col - 1)
                    {
                        text += ",";
                    }
                }
                writer.WriteLine(text);
            }
        }
    }

    public void WriteCSV(string path, double[] datas, bool newFile = false, bool header = false, string headers = null)
    {
        FileStream stream;

        if (newFile)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
        }
        else
            stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

        var row = datas.Length;

        using (StreamWriter writer = new StreamWriter(stream))
        {
            if (header)
            {
                writer.WriteLine(headers);
            }

            for (int i = 0; i < row; i ++)
            {
                writer.WriteLine(datas[i].ToString(provider));
            }
        }
    }

    public void WriteCSV(string path, double[,] datas, bool newFile = false, bool header = false, string[] headers = null)
    {
        FileStream stream;

        if (newFile)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
        }
        else
            stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

        var row = datas.GetLength(0);
        var col = datas.GetLength(1);

        using (StreamWriter writer = new StreamWriter(stream))
        {
            if (header)
            {
                var length = headers.Length;
                string text = "";
                for (int i = 0; i < length; i++)
                {
                    text += headers[i];
                    if (i != length - 1)
                    {
                        text += ",";
                    }
                }
                writer.WriteLine(text);
            }

            for (int i = 0; i < row; i ++)
            {
                string text = "";
                for (int j = 0; j < col; j++)
                {
                    text += datas[i,j].ToString(provider);
                    if (j != col - 1)
                    {
                        text += ",";
                    }
                }
                writer.WriteLine(text);
            }
        }
    }

    public async Task WriteCSVFile(string path, List<string> datas, bool newFile = false, bool header = false, string headers = null)
    {
        await Task.Run(() =>
        {
            FileStream stream;

            if (newFile)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            }
            else
                stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (header)
                {
                    writer.WriteLine(headers);
                }

                foreach (var data in datas)
                {
                    writer.WriteLine(data);
                }
            }
        });
    }

    public async Task WriteCSVFile(string path, List<double> datas, bool newFile = false, bool header = false, string headers = null)
    {
        await Task.Run(() =>
        {
            FileStream stream;

            if (newFile)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            }
            else
                stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (header)
                {
                    writer.WriteLine(headers);
                }

                foreach (var data in datas)
                {
                    writer.WriteLine(data);
                }
            }
        });
    }

    public async Task WriteCSVFile(string path, double[] datas, bool newFile = false, bool header = false, string headers = null)
    {
        await Task.Run(() =>
        {
            FileStream stream;

            if (newFile)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            }
            else stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            var row = datas.Length;

            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (header)
                {
                    writer.WriteLine(headers);
                }

                for (int i = 0; i < row; i ++)
                {
                    writer.WriteLine(datas[i]);
                }
            }
        });
    }

    public async Task WriteCSVFile(string path, double[,] datas, bool newFile = false, bool header = false, string[] headers = null)
    {
        await Task.Run(() =>
        {
            FileStream stream;

            if (newFile)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            }
            else stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            var row = datas.GetLength(0);
            var col = datas.GetLength(1);

            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (header)
                {
                    var length = headers.Length;
                    string text = "";
                    for (int i = 0; i < length; i++)
                    {
                        text += headers[i];
                        if (i != length - 1)
                        {
                            text += ",";
                        }
                    }
                    writer.WriteLine(text);
                }

                for (int i = 0; i < row; i ++)
                {
                    string text = "";
                    for (int j = 0; j < col; j++)
                    {
                        text += datas[i,j];
                        if (j != col - 1)
                        {
                            text += ",";
                        }
                    }
                    writer.WriteLine(text);
                }
            }
        });
    }
}

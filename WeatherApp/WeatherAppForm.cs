using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WeatherApp
{
    public partial class WeatherAppForm : Form
    {
        public WeatherAppForm()
        {
            InitializeComponent();
        }

        public WeatherDataInfo GetData()
        {
            WeatherDataInfo data = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    string url = string.Format("https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&hourly=temperature_2m", latitudeTextBox.Text, longTextBox.Text);
                    var json = client.DownloadString(url);
                    data = JsonConvert.DeserializeObject<WeatherDataInfo>(json);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return data;
        }

        public void GetWeatherData(WeatherDataInfo data)
        {
            for (int i = 0; i < data.HourlyProps.Times.Count; i++)
            {
                DateTime dt = DateTime.ParseExact(data.HourlyProps.Times[i], "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture);
                string result = dt.ToString("d/M/yyyy HH:mm");
                timeAndTempList.Items.Add(result + "   " + data.HourlyProps.Temperatures[i].ToString() + " " + ChangeEncoding(data.HourlyUnits.TemperatureUnit));
            }
        }

        public void GetCalculatedData(WeatherDataInfo data)
        {
            var CalcData = CalculateAverageTemp(data);

            foreach (var element in CalcData)
            {
                DateTime dt = DateTime.ParseExact(element.Key, "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture);
                string result = dt.ToString("d/M/yyyy");
                avgListBox.Items.Add(result + "  " + element.Value.ToString() + " " + ChangeEncoding(data.HourlyUnits.TemperatureUnit));
            }
        }

        public string ChangeEncoding(string toEncode)
        {
            byte[] bytes = Encoding.Default.GetBytes(toEncode);
            string afterEncode = Encoding.UTF8.GetString(bytes);

            return afterEncode;
        }

        private void getDataButton_Click(object sender, EventArgs e)
        {
            Regex validateText = new Regex(@"^[0-9.]+$");
            

            if (timeAndTempList.Items.Count != 0 && avgListBox.Items.Count != 0)
            {
                saveToFileButton.Enabled = true;
                timeAndTempList.Items.Clear();
                avgListBox.Items.Clear();
            }

            if (validateText.IsMatch(latitudeTextBox.Text) && validateText.IsMatch(longTextBox.Text)
                && latitudeTextBox.Text.Length != 0 && longTextBox.Text.Length != 0)
            {
                WeatherDataInfo data = GetData();
                GetWeatherData(data);
                GetCalculatedData(data);
                validationLabel.Visible = false;
                saveToFileButton.Enabled = true;
            }
            else
            {
                validationLabel.Visible = true;
            }

        }

        public Dictionary<string, float> CalculateAverageTemp(WeatherDataInfo data)
        {
            var dayAndAvgTemp = new Dictionary<string, float>();
            var temperatures = data.HourlyProps.Temperatures;
            var times = data.HourlyProps.Times;

            if (temperatures.Count < 24 || temperatures.Count != times.Count)
            {
                throw new ArgumentException("Mismatched data lengths or insufficient data.");
            }

            for (int i = 0; i <= temperatures.Count - 24; i += 24)
            {
                float sumOfTemps = 0;
                for (int j = i; j < i + 24; j++)
                {
                    sumOfTemps += temperatures[j];
                }

                float averageTemp = sumOfTemps / 24;
                var roundedValue = (float)Math.Round(averageTemp, 2, MidpointRounding.ToEven);
                dayAndAvgTemp.Add(times[i], roundedValue);
            }

            return dayAndAvgTemp;
        }

        private void saveToFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = @"C:\";
            saveFileDialog.Title = "Save file";
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = File.Create(saveFileDialog.FileName))
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.WriteLine("Hourly temperature for each day: ");
                    foreach (var listItem in timeAndTempList.Items)
                    {
                        streamWriter.WriteLine(listItem.ToString());
                    }
                    streamWriter.WriteLine("Average temperature for each day: ");

                    foreach (var avgListItem in avgListBox.Items)
                    {
                        streamWriter.WriteLine(avgListItem.ToString());
                    }
                }

            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (Application.MessageLoop)
            {
                Application.Exit();
            }
        }
    }
}

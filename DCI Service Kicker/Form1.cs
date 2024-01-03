using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Linq.Expressions;

namespace DCI_Service_Kicker
{
    public partial class Form1 : Form
    {
        public List<ApiConfig> config; // Declare config as a class-level variable
        public int every = 0;
        public int SetEvery = 0;
        public bool cron_every = false;
        public bool cron = false;
        public class ApiConfig
        {
            public string api { get; set; }
            public string Bearer { get; set; }

            public string time { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            // Define the Bangkok (Thailand) time zone
            TimeZoneInfo bangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Get the current time in the Bangkok time zone
            DateTime bangkokTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, bangkokTimeZone);

            // Display the current time in the desired format
            toolStripStatusLabel2.Text = bangkokTime.ToString("HH:mm:ss");

            if (cron == true) {
                foreach (var item in config)
                {
                    if (bangkokTime.ToString("HH:mm:ss") == item.time)
                    {
                        textBox2.AppendText("Cron of "+item.api+" Start");
                        textBox2.AppendText(Environment.NewLine);
                        await CallApiAsync(item); // Call the API request function when the condition is met
                    }
                    
                }
            }
            if (cron_every ==  true)
            {
                label3.Text = every.ToString();
                if (every == 0)
                {
                    foreach (var item in config)
                    {
                        await CallApiAsync(item); // Call the API request function when the condition is met

                    }
                    
                    every = SetEvery;
                }
                every--;
            }

  
        }
        private async Task CallApiAsync(ApiConfig item)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the authorization header with the bearer token
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + item.Bearer);

                try
                {
                    // Make a GET request to the API endpoint
                    HttpResponseMessage response = await httpClient.GetAsync(item.api);

                    // Check if the response is successful (status code 200)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and display the response content
                        string responseContent = await response.Content.ReadAsStringAsync();
                        textBox2.AppendText(Environment.NewLine);
                        textBox2.AppendText("API Response: " + responseContent);
                        textBox2.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        textBox2.AppendText(Environment.NewLine);
                        textBox2.AppendText("API Request Failed with status code: " + response.StatusCode);
                        textBox2.AppendText(Environment.NewLine);
                        
                    }
                }
                catch (Exception ex)
                {
                    textBox2.AppendText(Environment.NewLine);
                    textBox2.AppendText("API Request Failed with Exception: " + ex.Message);
                    textBox2.AppendText(Environment.NewLine);
                    
                }
            }
            Thread.Sleep(1000);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string filePath = "config.json"; // Replace with the path to your file

            if (!File.Exists(filePath))
            {
                MessageBox.Show("config.json does not exist.","config.json missing");
                Application.Exit();
                this.Close();
                return;
            }

            timer1.Start();
            // Read the JSON configuration from the file
            string json = File.ReadAllText("config.json");

            // Deserialize the JSON into a list of ApiConfig objects
            config = JsonConvert.DeserializeObject<List<ApiConfig>>(json);

            comboBox1.SelectedIndex = 0;

            // Display the JSON data in textBox1 as multi-line text
            if (config != null)
            {
                textBox1.Multiline = true;
                textBox1.ScrollBars = ScrollBars.Both;
                textBox1.WordWrap = false;

                foreach (var item in config)
                {
                    if (item.Bearer == null) 
                    {
                        item.Bearer = "null";
                    }
                    textBox1.AppendText(item.api+" : "+ item.Bearer);
                    textBox1.AppendText(Environment.NewLine); // Add a new line after each item
                }
            }
            else
            {
                MessageBox.Show("No items in the JSON array.");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(config[0].api);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Config File")
            {
                cron = true;
                label2.Text = "ON";
                textBox2.AppendText("CronJob Started with Config file.");
                textBox2.AppendText(Environment.NewLine); // Add a new line after each item
            }
            else
            {
                textBox2.AppendText("CronJob Started with second tracking.");
                textBox2.AppendText(Environment.NewLine); // Add a new line after each item
                cron_every = true;
                label2.Text = "ON";
                switch (comboBox1.SelectedItem.ToString())
                {
                    case "Every 15 sec":
                        SetEvery = 15;
                        every = 15;
                        break;
                    case "Every 30 sec":
                        SetEvery = 30;
                        every = 30;
                        break;
                    case "Every 45 sec":
                        SetEvery = 45;
                        every = 45;
                        break;
                    case "Every 60 sec":
                        SetEvery = 60;
                        every = 60;
                        break;
                    case "Every 90 sec":
                        SetEvery = 90;
                        every = 90;
                        break;
                    case "Every 120 sec":
                        SetEvery = 120;
                        every = 120;
                        break;
                    default:
                        SetEvery = 15;
                        every = 15;
                        break;
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {

            cron_every = false;
            cron = false;
            label2.Text = "OFF";
            SetEvery = 0;
            every = 0;
            label3.Text = "";
            textBox2.AppendText("CronJob Stoped.");
            textBox2.AppendText(Environment.NewLine); // Add a new line after each item
        }
    }
}

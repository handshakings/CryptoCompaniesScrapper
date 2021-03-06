using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace CryptoCompaniesScrapper
{
    public delegate void MyDelegate(string progress);
    public delegate void MyDelegate1(int count);
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateLabel(string progress)
        {
            Invoke(new Action(() =>
            {
                label4.Invalidate();
                label4.Refresh();
                label4.Text = progress;
            }));   
        }
        private void UpdateLabel1(int count)
        {
            Invoke(new Action(() =>
            {
                label3.Invalidate();
                label3.Refresh();
                label3.Text = "Progress "+count.ToString();
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string src = await new HttpClient().GetStringAsync("https://www.coingecko.com/en/coins/gdrt");
            //string src = await new HttpClient().GetStringAsync("https://www.coingecko.com/en/coins/vres");
            

            string url = comboBox1.Text;
            MyDelegate myDelegate = new MyDelegate(UpdateLabel);
            MyDelegate1 myDelegate1 = new MyDelegate1(UpdateLabel1);

            CoinMarketcapScrapper coinMarketcapScrapper;
            CoinGeckoScrapper coinGeckoScrapper;

            if (comboBox1.SelectedIndex == 0)
            {
                FileStream fileStream = new FileStream("coinmarketcap.csv", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fileStream);
                sw.WriteLine("Company Name, Company Link, Company Website ,Email");
                coinMarketcapScrapper = new CoinMarketcapScrapper(myDelegate,myDelegate1);
                List<CompanyData> companiesData = coinMarketcapScrapper.GetCompanyData(url);
                int count = 0;
                foreach(CompanyData company in companiesData)
                {
                    sw.WriteLine(company.CompanyName + "," + company.CompanyLink + "," + company.CompanyWebsite + "," + "\"" + company.CompanyEmail + "\"");
                    sw.Flush();
                    fileStream.Flush();
                    myDelegate1.DynamicInvoke(++count);
                    myDelegate.DynamicInvoke("Writing in coinmarketcap.csv");
                }
                sw.Close();
                fileStream.Close();
                myDelegate.DynamicInvoke("Completed");
                KillAllDrivers(new string[] { "geckodriver.exe", "chromedriver.exe", "firefix.exe" });
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                FileStream fileStream = new FileStream("coingecho.csv", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fileStream);
                sw.WriteLine("Company Name, Company Link, Company Website ,Email");
                
                coinGeckoScrapper = new CoinGeckoScrapper(myDelegate,myDelegate1);
                int pageCounter = 1;
                while(true)
                {
                    string pageUrl = comboBox1.Text + "?page=" + pageCounter;
                    List<CompanyData> companyData = coinGeckoScrapper.GetCompanyData(pageUrl);
                    if(companyData == null)
                    {
                        sw.Flush();
                        fileStream.Flush();
                        break;
                    }
                    else
                    {
                        foreach(CompanyData company in companyData)
                        {
                            sw.WriteLine(company.CompanyName + "," + company.CompanyLink + "," + company.CompanyWebsite + "," + "\"" + company.CompanyEmail + "\"");
                            sw.Flush();
                            fileStream.Flush();
                        }  
                    }
                    pageCounter++;
                }
                sw.Close();
                fileStream.Close();
            }

            
            myDelegate.DynamicInvoke("Completed");
            KillAllDrivers(new string[] { "geckodriver.exe", "chromedriver.exe", "firefix.exe" });
        }
        

        private void KillAllDrivers(string[] drivers)
        {
            foreach(string driver in drivers)
            {
                Process cmd = new Process();
                ProcessStartInfo cmdInfo = new ProcessStartInfo();
                cmdInfo.FileName = "cmd.exe";
                cmdInfo.Arguments = @"/c taskkill /IM "+driver+" /F";
                cmdInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmdInfo.CreateNoWindow = true;
                cmd.StartInfo = cmdInfo;
                cmd.Start();
                cmd.Close();
            } 
        }

        



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }
}

using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CryptoCompaniesScrapper
{
    public delegate void MyDelegate(string progress);
    
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

        private void button2_Click(object sender, EventArgs e)
        {
            string url = comboBox1.Text;
            MyDelegate myDelegate = new MyDelegate(UpdateLabel);

            CoinMarketcapScrapper coinMarketcapScrapper;
            CoinGeckoScrapper coinGeckoScrapper;

            if (comboBox1.SelectedIndex == 0)
            {
                FileStream fileStream = new FileStream("coinmarketcap.csv", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fileStream);
                sw.WriteLine("Company Name, Company Link, Company Website ,Email");
                //coinMarketcapScrapper = new CoinMarketcapScrapper(myDelegate);
                //companiesData = coinMarketcapScrapper.GetCompanyData(url);
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                FileStream fileStream = new FileStream("coingecho.csv", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fileStream);
                sw.WriteLine("Company Name, Company Link, Company Website ,Email");

                coinGeckoScrapper = new CoinGeckoScrapper(myDelegate);
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

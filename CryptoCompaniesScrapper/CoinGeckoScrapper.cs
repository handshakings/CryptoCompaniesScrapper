using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace CryptoCompaniesScrapper
{
    public class CoinGeckoScrapper
    {
        SeleniumClass SeleniumClass = new SeleniumClass();
        Delegate progressDelegate;
        Delegate countDelegate;
        IWebDriver chromeDriver;
        IWebDriver firefoxDriver;
        List<CompanyData> companiesData = null;
        EmailFinder emailFinder = new EmailFinder();
        int count = 0;

        public CoinGeckoScrapper(Delegate myDelegate, Delegate myDelegate1)
        {
            progressDelegate = myDelegate;
            countDelegate = myDelegate1;
            progressDelegate.DynamicInvoke("Preparing Setup for Scrapping...");
            companiesData = new List<CompanyData>();
            chromeDriver = SeleniumClass.CreateChromeDriver(true, null);
            firefoxDriver = SeleniumClass.CreateFirefoxDriver(true, null);
        }

        public List<CompanyData> GetCompanyData(string pageUrl)
        {
            progressDelegate.DynamicInvoke("Navigating to " + pageUrl);
            GetCompany(pageUrl);
            if(companiesData == null)
            {
                chromeDriver.Dispose();
                firefoxDriver.Dispose();
            }
            return companiesData;
        }

        private void GetCompany(string pageUrl)
        {
            string[] userAgents = File.ReadAllText(Directory.GetCurrentDirectory() + "/user-agents.txt").Split('\n');
            int userAgentsCounter = 0;

            SeleniumClass.Navigate(chromeDriver, pageUrl);
  
            var elements = chromeDriver.FindElements(By.XPath("/html/body/div[4]/div[6]/div[3]/div[2]/div/table/tbody/tr")).ToList();
            if (elements.Count != 0)
            {
                int switcher = 1;
                foreach (var element in elements)
                {
                    progressDelegate.DynamicInvoke("Finding Company Name and Link at : " + pageUrl);
                    try
                    {
                        string companyLink = element.FindElement(By.XPath("td[3]/div/div[2]/div/a[1]")).GetAttribute("href").Trim();
                        string companyName = element.FindElement(By.XPath("td[3]/div/div[2]/div/a[1]")).GetAttribute("text").Trim();
                        CompanyData companyData = new CompanyData { CompanyLink = companyLink, CompanyName = companyName };
                        List<string> compWebsireAndEmail = GetWebsiteAndEmail(companyLink, userAgents[userAgentsCounter]);
                        if(compWebsireAndEmail != null)
                        {
                            companyData.CompanyWebsite = compWebsireAndEmail.ElementAt(0);
                            companyData.CompanyEmail = compWebsireAndEmail.ElementAt(1);
                        }
                        companiesData.Add(companyData);
                        switcher++;
                        userAgentsCounter = userAgentsCounter < 999 ? userAgentsCounter++ : 0;
                        countDelegate.DynamicInvoke(count++);
                    }
                    catch (Exception)
                    {
                        countDelegate.DynamicInvoke(count++);
                        switcher++;
                        userAgentsCounter = userAgentsCounter < 999 ? userAgentsCounter++ : 0;
                        continue;
                    }  
                }
            }
        }
        private List<string> GetWebsiteAndEmail(string url, string userAgent)
        {
            try
            {
                progressDelegate.DynamicInvoke("Finding Company Website and Email at : " + url);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("user-agnet", userAgent);
                string src = client.GetStringAsync(url).Result;
                src = src.Substring(src.IndexOf("Website"));
                int i = src.IndexOf("</a>") - 100;
                src = src.Substring(i, 100).Split('>')[0];
                string companyWebsite = src.Substring(src.IndexOf("http")).Replace("\"", "").Trim();
                string pageSource = client.GetStringAsync(companyWebsite).Result;
                string email = string.Join("\n", emailFinder.SearchEmail(pageSource));
                return new List<string> { companyWebsite,email };
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void GetWebsites(int switcher, CompanyData companyData)
        {
            //try
            //{
            //    EmailFinder emailFinder = new EmailFinder();
            //    string companyWebsite = null;
            //    string companyEmail = null;
            //    //chrome
            //    if (switcher % 2 == 0)
            //    {
            //        SeleniumClass.Navigate(chromeDriver, companyData.CompanyLink);
            //        try
            //        {
            //            companyWebsite = chromeDriver.FindElement(By.XPath("/html/body/div[5]/div[5]/div[2]/div[2]/div[3]/div/a")).GetAttribute("href");
            //        }
            //        catch (Exception)
            //        {
            //            companyWebsite = chromeDriver.FindElement(By.XPath("/html/body/div[5]/div[4]/div[2]/div[2]/div[3]/div/a")).GetAttribute("href");
            //        }
            //        List<string> emails = emailFinder.SearchEmail(chromeDriver.PageSource);
            //        companyEmail = string.Join("\n", emails);
            //    }
            //    //firefox
            //    else
            //    {
            //        SeleniumClass.Navigate(firefoxDriver, companyData.CompanyLink);
            //        try
            //        {
            //            string pageSource = firefoxDriver.PageSource.ToLower().Substring(firefoxDriver.PageSource
            //                .IndexOf("website"));
            //            companyWebsite = firefoxDriver.FindElement(By.XPath("/html/body/div[5]/div[5]/div[2]/div[2]/div[3]/div/a")).GetAttribute("href");
            //        }
            //        catch (Exception)
            //        {
            //            companyWebsite = firefoxDriver.FindElement(By.XPath("/html/body/div[5]/div[4]/div[2]/div[2]/div[3]/div/a")).GetAttribute("href");
            //        }
            //        List<string> emails = emailFinder.SearchEmail(firefoxDriver.PageSource);
            //        string a = firefoxDriver.PageSource;
            //        companyEmail = string.Join("\n", emails);
            //    }
            //    companyData.CompanyWebsite = companyWebsite;
            //    companyData.CompanyEmail = companyEmail;
            //    companiesData.Add(companyData);
            //    progressDelegate.DynamicInvoke("Finding website and email of (" + companyData.CompanyName + ") ");
            //}
            //catch (Exception ex)
            //{
            //}
        }
        private List<string> FindAllUrlsAndEmail(List<IWebElement> links)
        {
            List<string> websiteUrls = new List<string>();
            foreach (var link in links)
            {
                try
                {
                    string url = link.GetAttribute("href");
                    if (url != null && !url.Contains("github"))
                    {
                        websiteUrls.Add(url);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return websiteUrls.Distinct().ToList();

            //int uaCount = 0;
            //foreach (var link in lks)
            //{
            //    HttpClient client = new HttpClient();
            //    client.DefaultRequestHeaders.Add("user-agent",userAgents[uaCount++]);
            //    string ps = await client.GetStringAsync(link);
            //    pageSource += ps;
            //}

        }


    }
}

using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CryptoCompaniesScrapper
{
    public class CoinGeckoScrapper
    {
        SeleniumClass SeleniumClass = new SeleniumClass();
        Delegate progressDelegate;
        IWebDriver chromeDriver;
        IWebDriver firefoxDriver;
        List<CompanyData> companiesData = null;

        public CoinGeckoScrapper(Delegate myDelegate)
        {
            progressDelegate = myDelegate;
        }

        public List<CompanyData> GetCompanyData(string pageUrl)
        {
            progressDelegate.DynamicInvoke("Preparing Setup for Scrapping...");
            companiesData = new List<CompanyData>();
            chromeDriver = SeleniumClass.CreateChromeDriver(true, null);
            progressDelegate.DynamicInvoke("Navigating to " + pageUrl);
            firefoxDriver = SeleniumClass.CreateFirefoxDriver(true, null);

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

            SeleniumClass.Navigate(chromeDriver, pageUrl);
  
            var elements = chromeDriver.FindElements(By.XPath("/html/body/div[4]/div[6]/div[3]/div[2]/div/table/tbody/tr")).ToList();
            if (elements.Count != 0)
            {
                int switcher = 1;
                foreach (var element in elements)
                {
                    progressDelegate.DynamicInvoke("Finding Company Name and Link at : " + pageUrl);
                    string companyLink = element.FindElement(By.XPath("td[3]/div/div[2]/div/a[1]")).GetAttribute("href").Trim();
                    string companyName = element.FindElement(By.XPath("td[3]/div/div[2]/div/a[1]")).GetAttribute("text").Trim();
                    CompanyData companyData = new CompanyData { CompanyLink = companyLink, CompanyName = companyName};
                    GetWebsites(switcher, companyData);
                    switcher++;
                }
            }
        }
        private void GetWebsites(int switcher, CompanyData companyData)
        {
            try
            {
                EmailFinder emailFinder = new EmailFinder();
                string companyWebsite = null;
                string companyEmail = null;
                //chrome
                if (switcher % 2 == 0)
                {
                    SeleniumClass.Navigate(chromeDriver, companyData.CompanyLink);
                    companyWebsite = chromeDriver.FindElement(By.XPath("/html/body/div[5]/div[5]/div[2]/div[2]/div[3]/div/a[0]")).GetAttribute("href");
                    List<string> emails = emailFinder.SearchEmail(chromeDriver.PageSource);
                    companyEmail = string.Join("\n", emails);
                }
                //firefox
                else
                {
                    SeleniumClass.Navigate(firefoxDriver, companyData.CompanyLink);
                    companyWebsite = firefoxDriver.FindElement(By.ClassName("/html/body/div[5]/div[5]/div[2]/div[2]/div[3]/div/a[0]")).GetAttribute("href");
                    List<string> emails = emailFinder.SearchEmail(chromeDriver.PageSource);
                    companyEmail = string.Join("\n", emails);
                }
                companyData.CompanyWebsite = companyWebsite;
                companyData.CompanyEmail = companyEmail;
                companiesData.Add(companyData);
                progressDelegate.DynamicInvoke("Finding website and email of (" + companyData.CompanyName + ") ");
            }
            catch (Exception)
            {
            }
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

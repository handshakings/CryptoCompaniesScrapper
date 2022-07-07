using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CryptoCompaniesScrapper
{
    public class CoinMarketcapScrapper
    {
        SeleniumClass SeleniumClass = new SeleniumClass();
        Delegate progressDelegate;
        Delegate countDelegate;
        IWebDriver chromeDriver;
        IWebDriver firefoxDriver;
        List<CompanyData> companiesData = null;
        EmailFinder emailFinder = new EmailFinder();
        int count = 0;

        public CoinMarketcapScrapper(Delegate myDelegate, Delegate myDelegate1)
        {
            progressDelegate = myDelegate;
            countDelegate = myDelegate1;
            progressDelegate.DynamicInvoke("Preparing Setup for Scrapping...");
            companiesData = new List<CompanyData>();
            chromeDriver = SeleniumClass.CreateChromeDriver(true, null);
            firefoxDriver = SeleniumClass.CreateFirefoxDriver(true, null);
        }

        public List<CompanyData> GetCompanyData(string url)
        {
            progressDelegate.DynamicInvoke("Navigating to " + url);

            List<CompanyData> companiesData = GetCompanies(chromeDriver, url);
            companiesData = GetWebsites(companiesData, chromeDriver, firefoxDriver);

            EmailFinder emailFinder = new EmailFinder();
            int count = 0;
            foreach (var data in companiesData)
            {
                SeleniumClass.Navigate(chromeDriver, data.CompanyWebsite);
                Thread.Sleep(3000);

                //getting all urls of website
                List<IWebElement> websitesUrls = chromeDriver.FindElements(By.TagName("a")).ToList();
                websitesUrls = websitesUrls.Distinct().ToList();

                List<string> emails = emailFinder.SearchEmail(chromeDriver.PageSource);
                string email = string.Join("\n", emails);
                data.CompanyEmail = email;
                progressDelegate.DynamicInvoke("Finding Email of (" + data.CompanyName + ")");
                countDelegate.DynamicInvoke(++count);
            }
            chromeDriver.Dispose();
            firefoxDriver.Dispose();
            return companiesData;
        }

        private List<CompanyData> GetCompanies(IWebDriver driver, string url)
        {

            SeleniumClass.Navigate(driver, url);
            List<CompanyData> companyData = new List<CompanyData>();
            var companies = driver.FindElements(By.XPath("//*[@id='__next']/div/div[1]/div[2]/div/div[2]/table/tbody/tr")).ToList();

            int count;
            for (count = 0; count < companies.Count; count++)
            {
                string link = companies[count].FindElement(By.XPath("td[3]/a")).GetAttribute("href");
                string company = companies[count].FindElement(By.XPath("td[3]/a/div/div/p")).Text;
                companyData.Add(new CompanyData { CompanyLink = link, CompanyName = company });
                progressDelegate.DynamicInvoke("Finding Company Name and Link");
                countDelegate.DynamicInvoke(count);
            }
            return companyData;
        }
        private List<CompanyData> GetWebsites(List<CompanyData> companyData, IWebDriver chromeDriver, IWebDriver firefoxDriver)
        {
            int driverSwitcher = 0;
            int count = 0;
            foreach (var data in companyData)
            {
                try
                {
                    //chrome
                    if (driverSwitcher % 2 == 0)
                    {
                        SeleniumClass.Navigate(chromeDriver, data.CompanyLink);
                        var websiteLink = chromeDriver.FindElements(By.ClassName("link-button"))[0].GetAttribute("href");
                        if (websiteLink != null)
                            data.CompanyWebsite = websiteLink;
                    }
                    //firefox
                    else
                    {
                        SeleniumClass.Navigate(firefoxDriver, data.CompanyLink);
                        var websiteLink = firefoxDriver.FindElements(By.ClassName("link-button"))[0].GetAttribute("href");
                        if (websiteLink != null)
                            data.CompanyWebsite = websiteLink;
                    }
                    driverSwitcher++;
                    countDelegate.DynamicInvoke(++count);
                    progressDelegate.DynamicInvoke("Finding website of (" + data.CompanyName + ")");
                    Thread.Sleep(200);
                }
                catch (Exception)
                {
                    driverSwitcher++;
                    continue;
                }
            }
            return companyData;
        }
        private List<string> FindAllUrlsAndEmail(List<IWebElement> links)
        {
            List<string> websiteUrls = new List<string>();
            int count = 0;
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

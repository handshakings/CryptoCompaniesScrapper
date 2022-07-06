using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;

namespace CryptoCompaniesScrapper
{
    public class SeleniumClass
    {
        public bool Navigate(IWebDriver driver, string url)
        {
            int i = 0;
            while (i < 3)
            {
                try
                {
                    driver.Navigate().GoToUrl(url);
                    break;
                }
                catch (Exception)
                {
                    i++;
                    continue;
                }
            }
            return i < 3 ? true : false;
        }

        public ChromeDriver CreateChromeDriver(bool hideBrowser, string defaultUrl)
        {
            ChromeOptions options = new ChromeOptions();
            if (hideBrowser)
            {
                options.AddArgument("--headless");
            }
            options.AddArgument("--no-sandbox");
            options.AddExcludedArgument("enable-automation");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeDriver driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(100));
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(50);
            if (defaultUrl != null)
                driver.Url = defaultUrl;
            if (hideBrowser)
            {
                driver.Manage().Window.Minimize();
            }
            else
            {
                driver.Manage().Window.Maximize();
            }
            return driver;
        }

        public FirefoxDriver CreateFirefoxDriver(bool hideBrowser, string defaultUrl)
        {
            FirefoxOptions options = new FirefoxOptions();
            if (hideBrowser)
            {
                options.AddArgument("--headless");
            }
            options.AddArgument("--no-sandbox");
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            FirefoxDriver driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(100));
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(50);
            if (defaultUrl != null)
                driver.Url = defaultUrl;
            if (hideBrowser)
            {
                driver.Manage().Window.Minimize();
            }
            else
            {
                driver.Manage().Window.Maximize();
            }
            return driver;
        }

    }
}

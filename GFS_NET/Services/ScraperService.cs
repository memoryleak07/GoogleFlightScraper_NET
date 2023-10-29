using GFS_NET.Interfaces;
using GFS_NET.Objects;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GFS_NET.Services
{
    public class ScraperService : IScraper
    {
        Random _rnd = new ();
        private IWebDriver _driver;
        private readonly ChromeSettings _chrOpt;
        private WebDriverWait _wait;


        public ScraperService(IOptions<ChromeSettings> chromeOption)
        {
            // Get chrome options from settings file
            _chrOpt = chromeOption.Value;

            // Init ChromeOptions instance
            ChromeOptions options = new ChromeOptions();

            // Add argument to chrome driver
            foreach (string driverOpt in _chrOpt.DriverOptions)
            {
                options.AddArgument(driverOpt);
            }
            // Randomize User Agent
            if (_chrOpt.RandomizeUserAgent)
            {
                options.AddArgument($"--user-agent={_chrOpt.UserAgents[_rnd.Next(0, _chrOpt.UserAgents.Count)]}");
            }
            // Randomize Referer
            if (_chrOpt.RandomizeReferer)
            {
                options.AddArgument($"--referer={_chrOpt.Referers[_rnd.Next(0, _chrOpt.Referers.Count)]}");
            }

            // New instance of ChromeDriver with options
            _driver = new ChromeDriver(options);
            //_driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(options));

            // Set the wait time
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(_chrOpt.Timeout));

            // Open url and accept cookie policy
            AcceptCookiePolicy(_chrOpt.GoogleBaseUrl, _chrOpt.AcceptCookieBtn);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        public string? GetElement(string url, string xpath)
        {
            if (!string.IsNullOrEmpty(url)) { _driver.Navigate().GoToUrl(url); }

            try
            {
                var element = _wait.Until(d =>
                {
                    try
                    {
                        return d.FindElement(By.XPath(xpath));
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                });

                if (element != null)
                {
                    return element.Text;
                }
            }
            catch (Exception ex) { Console.WriteLine("An exception occurred: " + ex.Message); }

            return null;
        }


        public List<string> GetElementsFromXPathList(string url, List<string> xpathList)
        {
            List<string> elementTextList = new();

            if (!string.IsNullOrEmpty(url)) { _driver.Navigate().GoToUrl(url); }

            try
            {
                foreach (string xpath in xpathList)
                {
                    var element = _wait.Until(drv =>
                    {
                        try
                        {
                            return drv.FindElement(By.XPath(xpath.ToString()));
                        }
                        catch (NoSuchElementException) { return null; }
                    });

                    if (element != null)
                    {
                        elementTextList.Add(element.Text);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("An exception occurred: " + ex.Message); }

            return elementTextList;
        }

        private void AcceptCookiePolicy(string url, string btnXpath)
        {
            // Go to url
            _driver.Navigate().GoToUrl(url);

            // Search element for xpath "AcceptCookieBtn"
            By elementLocator = By.XPath(btnXpath); 
            var element = _wait.Until(drv =>
            {
                try
                {
                    drv.FindElement(elementLocator).Click(); // Click element
                    return true; 
                }
                catch (NoSuchElementException) { return false; }
            });
        }
    }
}

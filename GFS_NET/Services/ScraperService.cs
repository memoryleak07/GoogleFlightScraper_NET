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
        Random _rnd = new();
        private readonly ILogger _logger;
        private readonly IWebDriver _driver;
        private readonly ChromeSettings _chrOpt;
        private readonly WebDriverWait _waitTime;

        public ScraperService(IOptions<ChromeSettings> chromeOption, ILogger logger)
        {
            _logger = logger;
            _chrOpt = chromeOption.Value;

            // Init ChromeOptions instance
            ChromeOptions options = new();
            foreach (string driverOpt in _chrOpt.DriverOptions)
            {
                options.AddArgument(driverOpt);
            }
            if (_chrOpt.RandomizeUserAgent)
            {
                options.AddArgument($"--user-agent={_chrOpt.UserAgents[_rnd.Next(0, _chrOpt.UserAgents.Count)]}");
            }
            if (_chrOpt.RandomizeReferer)
            {
                options.AddArgument($"--referer={_chrOpt.Referers[_rnd.Next(0, _chrOpt.Referers.Count)]}");
            }

            _driver = new ChromeDriver(options);

            _waitTime = new WebDriverWait(_driver, TimeSpan.FromSeconds(_chrOpt.Timeout));

            AcceptCookiePolicy(_chrOpt.GoogleBaseUrl, _chrOpt.AcceptCookieBtn);
        }

        private void AcceptCookiePolicy(string url, string btnXpath)
        {
            _driver.Navigate().GoToUrl(url);
            By elementLocator = By.XPath(btnXpath);
            var element = _waitTime.Until(drv =>
            {
                try
                {
                    drv.FindElement(elementLocator).Click();
                    _logger.Information("Cookie Policy successfully accepted.");
                    return true;
                }
                catch (NoSuchElementException) { return false; }
            });
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        public List<string> GetElementsFromXpathDict(string url, Dictionary<string, string> xpaths)
        {
            List<string> elementTextList = [];
            try
            {
                if (!string.IsNullOrEmpty(url)) _driver.Navigate().GoToUrl(url);

                foreach (var kvp in xpaths)
                {
                    var element = _waitTime.Until(drv =>
                    {
                        try
                        {
                            return drv.FindElement(By.XPath(kvp.Value));
                        }
                        catch (NoSuchElementException) 
                        {
                            return null; 
                        }
                    });

                    if (element == null)
                    {
                        _logger.Warning($"Can't find any element for xpath {kvp.Key}, continue...");
                        continue;
                    }

                    elementTextList.Add(element.Text);
                }
            }
            catch (Exception ex) 
            { 
                _logger.Error("An exception occurred: " + ex.Message); 
            }

            return elementTextList;
        }
    }
}

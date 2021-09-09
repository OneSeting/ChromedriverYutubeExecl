using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Selenium
{
    public class SeleniumHeiper
    {
        public (RemoteWebDriver, string) LoginByAccount(string url, string user, string password, string useragent, bool isShowChrome = false)
        {
            var message = string.Empty;
            ChromeOptions chromeOptions = new ChromeOptions();
            List<string> ls = new List<string>();
            ls.Add("enable-automation");
            chromeOptions.AddExcludedArguments(ls);
            chromeOptions.AddArguments("start-maximized");
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);  //禁止加载图片 可用
            chromeOptions.AddArguments("--no-sandbox");
            chromeOptions.AddArgument("disable-gpu");
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument($"user-agent='{useragent}'");

            if (!isShowChrome)
                chromeOptions.AddArgument("--headless"); //后台运行模式
            RemoteWebDriver webDriver = null;
            try
            {
                webDriver = new ChromeDriver(chromeOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            webDriver.Url = url;
            if (true)
            {
                var page = webDriver.PageSource;
                Console.WriteLine(page);
            }
            try
            {
                //pc端
                //*[@id="loginForm"]/div[1]/div[3]/div/label/input      //*[@id="loginForm"]/div[1]/div[4]/div/label/input   //*[@id="loginForm"]/div[1]/div[6]/button/div
                webDriver.FindElementByXPath(@"//*[@id='loginForm']/div/div[1]/div/label/input").SendKeys(user);
                webDriver.FindElementByXPath(@"//*[@id='loginForm']/div/div[2]/div/label/input").SendKeys(password);
                webDriver.FindElementByXPath(@"//*[@id='loginForm']/div/div[3]").Click();
                //手机端
                //webDriver.FindElementByXPath(@"//*[@id='loginForm']/div[1]/div[3]/div/label/input").SendKeys(user);
                //webDriver.FindElementByXPath(@"//*[@id='loginForm']/div[1]/div[4]/div/label/input").SendKeys(password);
                //webDriver.FindElementByXPath(@"//*[@id='loginForm']/div[1]/div[6]/button/div").Click();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            message = CheckIsLogin(message, webDriver);
            return (webDriver, message);
        }
        private string CheckIsLogin(string message, RemoteWebDriver driver)
        {
            try
            {
                if (FindElementsByXPath(driver, "//*[@id='loginForm']/div/div[3]/button/div").Count > 0)
                {
                    message = "登录失败,请检查cookie重试";
                    driver.Quit();
                }
                else if (FindElementsByXPath(driver, "//*[@id='react-root']/section/div/div/div[2]/form/span/button").Count > 0)
                {
                    message = "账户已锁,请解锁后重试";
                    driver.Quit();
                }
                else if (FindElementsByXPath(driver, "//*[@id='react-root']/section/div/div/div[3]/form/span/button").Count > 0)
                {
                    message = "账户已锁,请解锁后重试";
                    driver.Quit();
                }
                else
                {
                    Console.WriteLine("登陆成功");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }
        public ReadOnlyCollection<IWebElement> FindElementsByXPath(RemoteWebDriver driver, string xpath)
        {
            return driver.FindElementsByXPath(xpath);
        }
    }
}

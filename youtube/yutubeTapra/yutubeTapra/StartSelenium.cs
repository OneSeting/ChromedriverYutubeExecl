using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace yutubeTapra
{
    public class StartSelenium
    {
        //private static RemoteWebDriver driver;

        /// <summary>
        /// 开始抓取
        /// </summary>
        /// <returns></returns>
        public static string StartSeleniumGo(string link) 
        {
            try
            {
                IWebDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl(link);
                System.Threading.Thread.Sleep(2 * 1000);
                driver.FindElement(By.Id("img")).Click();

                string ursl = driver.Url;
                if (ursl.Substring(0, 32) != "https://www.youtube.com/channel/")
                {
                    driver.Navigate().Refresh();
                    //driver.FindElement(By.Id("firstname-placeholder")).SendKeys(Keys.F5);
                    ursl = driver.Url;
                }
                Console.WriteLine(ursl);
                driver.Close();
                return ursl;
            }
            catch (Exception ex)
            {
                return "no";
            }
            
        }
    }
}

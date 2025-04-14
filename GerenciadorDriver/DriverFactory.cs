using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using System.Runtime.InteropServices;
using System.IO;

namespace FrameVioti.GerenciadorDriver
{
    public class DriverFactory
    {
        private static IWebDriver? driver;

        private DriverFactory() { }

           public static IWebDriver GetDriver(BrowserType browserType = BrowserType.Chrome)
        // public static IWebDriver GetDriver(BrowserType browserType = BrowserType.Firefox)
       //  public static IWebDriver GetDriver(BrowserType browserType = BrowserType.Edge)
        {
            if (driver == null)
            {
                switch (browserType)
                {
                    case BrowserType.Chrome:
                         new DriverManager().SetUpDriver(new ChromeConfig(), "135.0.7049.42");

                        ChromeOptions chromeOptions = new ChromeOptions();

                        string basePath = AppContext.BaseDirectory;
                        string chromeBinaryPath = string.Empty;

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            //chromeBinaryPath = Path.Combine(basePath, "ChromeForTesting", "chrome-mac-x64", "Google Chrome for Testing");
                            chromeBinaryPath = Path.Combine(basePath, "ChromeForTesting", "chrome-mac-x64", "Google Chrome for Testing.app", "Contents", "MacOS", "Google Chrome for Testing");


                         //   basepath/ChromeForTesting/chrome-mac-x64/Google Chrome for Testing.app

                            // Adiciona flags para evitar erro "DevToolsActivePort"
                            chromeOptions.AddArguments("--no-sandbox");
                            chromeOptions.AddArguments("--disable-dev-shm-usage");
                            chromeOptions.AddArguments("--remote-debugging-port=9222");
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            chromeBinaryPath = Path.Combine(basePath, "ChromeForTesting", "chrome-win64", "chrome.exe");
                        }

                        if (File.Exists(chromeBinaryPath))
                        {
                            chromeOptions.BinaryLocation = chromeBinaryPath;
                        }
                        else
                        {
                            throw new FileNotFoundException("Chrome for Testing não encontrado no caminho esperado: " + chromeBinaryPath);
                        }

                        driver = new ChromeDriver(chromeOptions);
                        break;

                    case BrowserType.Edge:
                        new DriverManager().SetUpDriver(new EdgeConfig());
                        driver = new EdgeDriver();
                        break;

                    case BrowserType.Firefox:
                        new DriverManager().SetUpDriver(new FirefoxConfig());
                        driver = new FirefoxDriver();
                        break;

                    default:
                        throw new WebDriverException("Invalid browser type provided.");
                }

                driver.Manage().Window.Maximize();
            }

            return driver;
        }

        public static void KillDriver()
        {
            if (driver != null)
            {
                driver.Quit();
                driver = null;
            }
        }
    }

    public enum BrowserType
    {
        Chrome,
        Edge,
        Firefox
    }
}
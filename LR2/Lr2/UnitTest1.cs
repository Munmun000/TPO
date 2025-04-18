using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Xml.Linq;

namespace Lr2
{
    [TestFixture]
    public class Tests
    {
        private IWebDriver driver;
        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();//new FirefoxDriver(service, options);
            driver.Url = "https://everskies.com/";
            //driver.Url = "https://www.playground.ru/";
        }

        [Test]
        public void MainTitle()
        {
            Assert.That(driver.Title, Is.EqualTo("Everskies - Everskies"));

        }

        [Test]
        public void PressButton()
        {
            IWebElement button = driver.FindElement(By.XPath("//*[@id=\"top-bar-wrapper\"]/div/x-top-bar-menu/div/div[2]/a"));
            button.Click();
        }

        [Test]
        public void LogoDisplayed()
        {
            IWebElement element = driver.FindElement(By.XPath("//*[@id=\"top-bar-wrapper\"]/div/x-top-bar-menu/div/div[2]/a"));
            bool status = element.Displayed;
        }

        [Test]
        public void NavigationMenu_ContainLinks()
        {
            var menuItems = driver.FindElements(By.XPath("//*[@id=\"top-bar-wrapper\"]/div/x-top-bar-menu/div"));
            Assert.That(menuItems.Count, Is.GreaterThan(0));
        }


        [Test]
        public void Search()
        {
            IWebElement searchInput = driver.FindElement(By.XPath("//*[@id=\"top-bar-wrapper\"]/div/x-top-bar-menu/div/div[4]/x-top-bar-search/div/div[1]/div/input"));
            searchInput.Click();
            searchInput.SendKeys("clothes");
            var result = driver.FindElements(By.XPath("//*[@id=\"top-bar-wrapper\"]/div/x-top-bar-menu/div/div[4]/x-top-bar-search/div/div[2]/x-tabs/div/div[2]/div[1]/div/x-custombar/div[1]/div[2]/div/div/div"));
            Assert.That(result.Count, Is.GreaterThan(0));
        }

        [Test]
        public void LinkNavigation_ShouldRedirectToCorrectPage()
        {
            string originalWindow = driver.CurrentWindowHandle;

            IWebElement link = driver.FindElement(By.XPath("//a[@href='https://www.youtube.com/channel/UCHJNK1GsyHDCRXIEydE3c8w']"));
            link.Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.WindowHandles.Count > 1);
    
            foreach (string window in driver.WindowHandles)
            {
                if (window != originalWindow)
                {
                    driver.SwitchTo().Window(window);
                    break;
                }
            }

            Assert.That(driver.Url, Is.EqualTo("https://www.youtube.com/channel/UCHJNK1GsyHDCRXIEydE3c8w"),
                "URL после перехода не соответствует ожидаемому");
        }

        [Test]
        public void TextInput()
        {
            IWebElement searchInput = driver.FindElement(By.XPath("//*[@id=\"top-bar-wrapper\"]/div/x-top-bar-menu/div/div[4]/x-top-bar-search/div/div[1]/div/input"));
            searchInput.Click();
            searchInput.SendKeys("clothes");
        }

        [Test]
        public void HidePreviews_Checkbox()
        {
            driver.Navigate().GoToUrl("https://everskies.com/club/free-gifting");
            driver.FindElement(By.XPath("/html/body/x-root/div/x-club/div/div/div[1]/x-tabs/div/div[3]/div[1]/x-community-feed/div/div[2]/div[2]/x-button-menu/div/div/div")).Click();

            IWebElement hidePreviews = driver.FindElement(By.XPath("//*[@id=\"cdk-overlay-1\"]/x-tooltip/div/div[2]/x-button-menu-content/div/div[2]/div[2]"));

            bool isCheckedBefore = hidePreviews.Selected;

            var feedInnerElementsBefore = driver.FindElements(By.ClassName("feed-inner"));
            CollectionAssert.IsNotEmpty(feedInnerElementsBefore);


            hidePreviews.Click();

            System.Threading.Thread.Sleep(1000); 

            bool isCheckedAfter = hidePreviews.Selected;

            var feedInnerElementsAfter = driver.FindElements(By.ClassName("feed-inner"));
            CollectionAssert.IsEmpty(feedInnerElementsAfter);
        }

        [Test]
        public void ChangePageTheme()
        {
            driver.Navigate().GoToUrl("https://everskies.com/user/login");

            var body = driver.FindElement(By.TagName("body"));
            string initialThemeClass = body.GetAttribute("class");

            var themeSwitcher = driver.FindElement(By.XPath("/html/body/x-root/div/x-guest-page/div/div/div[1]/i"));
            themeSwitcher.Click();

            Thread.Sleep(1000);

            string newThemeClass = body.GetAttribute("class");

            if (initialThemeClass.Contains("light"))
            {
                Assert.That(newThemeClass, Does.Contain("dark"));
            }
            else
            {
                Assert.That(newThemeClass, Does.Contain("light"));
            }
        }

        [Test]
        public void ClubsAvatars_HomePage()
        {
            driver.Navigate().GoToUrl("https://everskies.com/club");

            var clubs = driver.FindElements(By.CssSelector("x-club-widget .club"));
            Assert.That(clubs.Count, Is.GreaterThan(0), "Ќа странице нет клубов");

            foreach (var club in clubs)
            {
                try
                {
                    IWebElement avatar = club.FindElement(By.CssSelector("img.rounded.logo-img"));

                    Assert.That(avatar.Displayed, Is.True, "јватарка клуба не отображаетс€");
                }
                catch (NoSuchElementException)
                {
                    Assert.Fail("Ќе найдена аватарка дл€ одного из клубов");
                }
            }
        }

        [TearDown]
        public void TestEnd()
        {
            driver.Close();
            driver?.Quit();
            driver?.Dispose(); // ќсвобождаем ресурсы
        }
    }
}
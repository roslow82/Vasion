using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added classes
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using System.Diagnostics;

namespace PrinterLogicsTests
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    [TestFixture(typeof(EdgeDriver))]
    //[TestFixture(typeof(InternetExplorerDriver))]

    class PrinterLogicTests<TWebDriver> where TWebDriver : IWebDriver, new()
    {
        #region Variable Decleration
        // Declare driver
        private IWebDriver driver;
       
        // Declare text to use in tests
        static string url = @"https://russc.printercloud.com/admin/";   // URL used for testing
        static string privacyPolicyUrl = @"https://www.printerlogic.com/privacy-policy/";
        static string passwordResetUrl = @"https://russc.printercloud.com/admin/password/reset/";

        static string usernameId = "relogin_user";          // Expected ID of username field on the page
        static string passwordId = "relogin_password";      // Expected ID of password field on the page
        static string loginButtonId = "admin-login-btn";    // Expected ID of login button on the page
        static string loginTextID = "logintext";            // Expected ID of login text placeholder on the page
        static string passwordResetFormID = "forgot-password-form";

        static string noUserOrPassword = "Please Enter your Username and Password:";        // Expected text when no user or password is entered
        static string invalidUserOrPassword = "Invalid username or password.";              // Expected text when incorrect user or password is entered
        static string launchingService = "Launching service";                               // Expected text when correct user and password is entered

        static string passwordResetLinkText = "Lost Password";  // Expected text for password reset link
        static string privacyPolicyLinkText = "Privacy Policy"; // Expected text for privacy policy link

        static string correctUsername = "RussClegg";    // Correct username
        static string correctPassword = "MyPassword1";  // Correct password
        static string passwordResetEmail = "russelljclegg@gmail.com";

        // Declare helper variables
        WebDriverWait waitForElement;
        IList<IWebElement> links;
        #endregion

        #region Setup
        [SetUp]
        public void InitializeBrowser()
        {           
            driver = new TWebDriver();  // assign new WebDriver
            driver.Url = url;           // assign url

            // setup waits and timeouts
            waitForElement = new WebDriverWait(driver, TimeSpan.FromMilliseconds(500)); // used to wait for specific elements to change state
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(200);   // used to wait in general for items/pages to appear
        }
        #endregion

        #region Tests
        /// <summary>
        /// Test the correct login information
        /// </summary>
        [Test]
        public void LoginCorrect()
        {
            // get the username input field and enter the correct username
            IWebElement usernameElement = driver.FindElement(By.Id(usernameId));
            usernameElement.SendKeys(correctUsername);

            // get the password input field and enter the correct password
            IWebElement passwordElement = driver.FindElement(By.Id(passwordId));
            passwordElement.SendKeys(correctPassword);

            // find the login button and click it
            driver.FindElement(By.Id(loginButtonId)).Click();

            try
            {
                #region Pass Condition
                // When the div with tabset class assigned is shown
                Assert.That(driver.FindElement(By.ClassName("tabset")).Displayed);
                #endregion
            }
            catch (Exception)
            {
                // determine if the test failed due to incorrect or missing login information
                IWebElement loginText = driver.FindElement(By.Id(loginTextID)); // get the login text element for pass/fail checks
                waitForElement.Until(d => loginText.Text.Length > 0);

                // possible fail conditions
                if (loginText.Text.ToLower().Contains(noUserOrPassword.ToLower()))      // check for no username or password
                    Assert.Fail(noUserOrPassword.ToLower());
                if (loginText.Text.ToLower().Contains(invalidUserOrPassword.ToLower())) // check for incorrect username or password
                    Assert.Fail(invalidUserOrPassword.ToLower());
            }
        }

        [Test]
        public void LoginIncorrect()
        {
            // get the username input field and enter the incorrect username
            IWebElement usernameElement = driver.FindElement(By.Id(usernameId));
            usernameElement.SendKeys(correctUsername + "1");

            // get the password input field and enter the incorrect password
            IWebElement passwordElement = driver.FindElement(By.Id(passwordId));
            passwordElement.SendKeys(correctPassword + "1");

            // find the login button and click it
            driver.FindElement(By.Id(loginButtonId)).Click();

            // get the login text element for pass/fail checks
            IWebElement loginText = driver.FindElement(By.Id(loginTextID));
            waitForElement.Until(d => loginText.Text.Length > 0);

            #region Pass Condition
            // The login text for invalid user is shown
            if (loginText.Text.ToLower().Contains(invalidUserOrPassword.ToLower()))
                Assert.Pass(invalidUserOrPassword.ToLower());
            #endregion

            // possible fail conditions
            if (loginText.Text.ToLower().Contains(noUserOrPassword.ToLower()))  // check for no username or password
                Assert.Fail(noUserOrPassword.ToLower());
            if (loginText.Text.ToLower().Contains(launchingService.ToLower()))  // check for a successful login
                Assert.Fail(launchingService.ToLower());
        }
        
        [Test]
        public void LoginNoUser()
        {
            // only get the password input field and enter the correct password
            IWebElement passwordElement = driver.FindElement(By.Id(passwordId));
            passwordElement.SendKeys(correctPassword);

            // find the login button and click it
            driver.FindElement(By.Id(loginButtonId)).Click();

            // get the login text element for pass/fail checks
            IWebElement loginText = driver.FindElement(By.Id(loginTextID));
            waitForElement.Until(d => loginText.Text.Length > 0);


            #region Pass Condition
            // When the text for no username or password is shown            
            if (loginText.Text.ToLower().Contains(noUserOrPassword.ToLower()))
                Assert.Pass(noUserOrPassword.ToLower());
            #endregion


            // possible fail conditions
            if (loginText.Text.ToLower().Contains(invalidUserOrPassword.ToLower())) // check for invalid username or password text
                Assert.Fail(invalidUserOrPassword.ToLower());
            if (loginText.Text.ToLower().Contains(launchingService.ToLower()))      // check for a successful login
                Assert.Fail(launchingService.ToLower());

        }

        /// <summary>
        /// Test login with no Password
        /// </summary>
        [Test]
        public void LoginNoPassword()
        {
            // only get the username input field and enter the correct username
            IWebElement usernameElement = driver.FindElement(By.Id(usernameId));
            usernameElement.SendKeys(correctUsername);

            // find the login button and click it
            driver.FindElement(By.Id(loginButtonId)).Click();

            // get the login text element for pass/fail checks
            IWebElement loginText = driver.FindElement(By.Id(loginTextID));
            waitForElement.Until(d => loginText.Text.Length > 0);

            #region Pass Condition
            // When the text for no username or password is shown
            if (loginText.Text.ToLower().Contains(noUserOrPassword.ToLower()))
                Assert.Pass(noUserOrPassword.ToLower());
            #endregion

            // possible fail conditions
            if (loginText.Text.ToLower().Contains(invalidUserOrPassword.ToLower())) // check for invalid username or password text
                Assert.Fail(invalidUserOrPassword.ToLower());
            if (loginText.Text.ToLower().Contains(launchingService.ToLower()))      // check for a successful login
                Assert.Fail(launchingService.ToLower());
        }

        /// <summary>
        /// Test all the links shown on the login page
        /// </summary>
        [Test]
        public void TestLinks()
        {
            // populate the links list with all links on the page
            links = driver.FindElements(By.TagName("a"));

            // loop through all the links
            for(int i=0;i<links.Count;i++)
            {
                //make sure the link is visible first
                if (links[i].Displayed)
                {
                    // check the link for password reset
                    if (links[i].Text.ToLower() == passwordResetLinkText.ToLower())
                    {
                        // click the link
                        links[i].Click();

                        #region Pass Condition
                        // When the text for no username or password is shown
                        Assert.That(driver.FindElement(By.Id(passwordResetFormID)).Displayed);
                        #endregion

                        driver.Navigate().Back();
                        links = driver.FindElements(By.TagName("a"));
                    }
                    //check the link for privacy policy
                    else if (links[i].Text.ToLower() == privacyPolicyLinkText.ToLower())
                    {
                        // click the link
                        links[i].Click();
                        
                        // switch to the open window
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        
                        // wait for the new window to open
                        waitForElement.Until(d => driver.Url == privacyPolicyUrl);
                        
                        #region Pass Condition
                        // When the URL matches the privacy policy url
                        Assert.That(driver.Url == privacyPolicyUrl);
                        #endregion

                        links = driver.FindElements(By.TagName("a"));
                    }
                }
            }
        }

        /// <summary>
        /// Password reset test. Tests: [Blank, Correct, Incorrect]
        /// </summary>
        /// <param name="email"></param>
        [Test]
        [TestCase("")] // blank email test
        [TestCase("correct")] // correct email format test
        [TestCase("incorrect")] //incorrect email format test
        public void TestPasswordReset(string email)
        {
            // navigate to the password reset url
            driver.Url = passwordResetUrl;

            // if testing for a correct email assign the correct email
            if (email == "correct")
                email = passwordResetEmail;
            
            // create a local variable to flag valid email format
            bool validEmail = true;
            try
            {
                // check for testing a blank email
                if (email.Length > 0)
                {
                    // use the MailAddress class to validate email format
                    System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(email);
                }
            }
            catch (Exception)
            {
                // flag the email as invalid
                validEmail = false;
            }

            
            if (!validEmail)
                // if it wasn't valid assert a warning that the form's validation would have failed
                Assert.Warn("Email wasn't a valid email, form validation failed");
            else
            {
                // get the email input and type in the email address
                IWebElement resetPasswod = driver.FindElement(By.Id("email")); 
                resetPasswod.SendKeys(email);
                
                // find the submit button and click it
                driver.FindElement(By.XPath("//*[@type='submit']")).Click();

                try
                {
                    #region Pass Condition
                    // When the success text is displayed
                    Assert.That(driver.FindElement(By.ClassName("success")).Displayed);
                    #endregion
                }
                catch (Exception)
                {
                    #region Pass Condition
                    // When the error text is displayed
                    Assert.That(driver.FindElement(By.ClassName("error")).Displayed);
                    #endregion
                }
            }
        }
        #endregion

        #region TearDown
        [TearDown]
        public void ShutDownBrowser()
        {    
            // close and quit the driver to prevent the tasks from persisting after tests
            driver.Close();
            driver.Quit();
        }
        #endregion
    }
}

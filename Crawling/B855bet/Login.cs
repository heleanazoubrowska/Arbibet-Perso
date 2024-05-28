using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class B855bet : CasinoAPI
	{
		public override void Login(int prm_WaitTime)
		{
			try
			{
                chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(prm_WaitTime);
                IWebElement form = chromeDriver.FindElementByClassName("login_box");

                // GET IMAGE TEXT
                //IWebElement imgVerif = chromeDriver.FindElementById("imgcode");
                //Screenshot sc = ((ITakesScreenshot)chromeDriver).GetScreenshot();
                //var img = Image.FromStream(new MemoryStream(sc.AsByteArray)) as Bitmap;
                //var imgCrop = img.Clone(new Rectangle(imgVerif.Location, imgVerif.Size), img.PixelFormat);

                //imgCrop.Save("check.png", System.Drawing.Imaging.ImageFormat.Png);

                var base64string = chromeDriver.ExecuteScript(@"
                    var c = document.createElement('canvas');
                    var ctx = c.getContext('2d');
                    var img = document.getElementById('imgcode');
                    c.height=img.naturalHeight;
                    c.width=img.naturalWidth;
                    ctx.drawImage(img, 0, 0,img.naturalWidth, img.naturalHeight);
                    var base64String = c.toDataURL();
                    return base64String;
                    ") as string;

                var base64 = base64string.Split(',').Last();

                using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    using (var bitmap = new Bitmap(stream))
                    {
                        var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "check.png");
                        bitmap.Save("check.png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                }


                //using (OCRProcessor processor = new OCRProcessor(@"TesseractBinaries\"))
                //{
                //    //loading the input image
                //    FileStream stream = new FileStream(@"check.png", FileMode.Open);

                //    Bitmap image = new Bitmap(stream);

                //    //Set OCR language to process
                //    processor.Settings.Language = Languages.English;

                //    //Process OCR by providing the bitmap image, data dictionary, and language
                //    string ocrText = processor.PerformOCR(image, @"tessdata\");

                //}

                //Console.WriteLine("LOLOL 1");
                //    Bitmap imgCheckSaved = new Bitmap("check.png");
                //    Console.WriteLine("LOLOL 2");
                //    TesseractEngine engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
                //    Console.WriteLine("LOLOL 3");
                //    Page page = engine.Process(imgCheckSaved, PageSegMode.Auto);
                //    Console.WriteLine("LOLOL 4");
                //    string resultImg = page.GetText();
                //    Console.WriteLine("Result image : "+ resultImg);


                // Connect From Login Page
                form.FindElement(By.Id("txtUsername")).SendKeys(accountAPI.AccountUsername);
                form.FindElement(By.Id("txtPassword")).SendKeys(accountAPI.AccountPassword);

                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                // Check if we need to accept agreement
                if (chromeDriver.FindElements(By.Id("Form1")).Count > 0)
                {
                    NLogger.Log(EventLevel.Debug, "We detect the agreement");
                    //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    //chromeDriver.SwitchTo().Frame(0);
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnOK")));
                    chromeDriver.FindElement(By.Id("btnOK")).Click();
                }
                else
                {
                    NLogger.Log(EventLevel.Debug, "NO detect the agreement");
                }
            }
			catch (Exception prv_Exception)
			{
				NLogger.Log(EventLevel.Error, $"{base.accountAPI.AccountBookmakerName} failed to log in");
				NLogger.LogError(prv_Exception);
			}
		}
	}
}

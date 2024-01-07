using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.ComponentModel;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KreamCrawling
{
    public partial class Form1 : Form
    {
        string kream_id = "123123";
        string kream_pass = "123123";
        ChromeDriverService service;
        ChromeOptions options;
        ChromeDriver driver;

        public class Product
        {
            public string productCateName;
            public string productEngName; //
            public string productKorName; //
            public string productRelease; //
            public string productBrandEngName;
            public string productBrandKorName;
            public string productBrandImgUrl;
            public string productColor;
            public string productModelNum;
            public string productDate;
            public bool productState = true;
            public List<string> thumbLinks = new List<string>();
            public List<string> sizeList = new List<string>();

            public override string ToString() => 
                "productCateName:" + productCateName + "\r\n" +
                "productEngName:" + productEngName + "\r\n" +
                "productKorName:" + productKorName + "\r\n" +
                "productRelease:" + productRelease + "\r\n" +
                "productEngBrand:" + productBrandEngName + "\r\n" +
                "productKorBrand:" + productBrandKorName + "\r\n" +
                "productColor:" + productColor + "\r\n" +
                "productModelNum:" + productModelNum + "\r\n" +
                "productDate:" + productDate + "\r\n" +
                "productState:" + productState + "\r\n" +
                "thumbLinks:" + thumbLinks;
        }
        public Form1()
        {
            InitializeComponent();
        }

        public void login(string returnUrl)
        {
            try
            {
                IWebElement emailInput = driver.FindElement(By.CssSelector("input[type='email']"));
                IWebElement passInput = driver.FindElement(By.CssSelector("input[type='password']"));
                emailInput.SendKeys(kream_id);
                passInput.SendKeys(kream_pass);
                IWebElement loginButton = driver.FindElement(By.XPath(".//a[@class='btn full solid']"));
                loginButton.Click();
                Thread.Sleep(3000);

                //driver.Navigate().GoToUrl(returnUrl);
            }
            catch
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            service = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory(), "chromedriver.exe");
            options = new ChromeOptions();
            //options.AddArgument("--headless"); // GUI 없이 실행
            driver = new ChromeDriver(service, options);
            driver.Manage().Window.Maximize();
            Product p;
            List<Product> pList = new List<Product>();
            HtmlDocument doc = new HtmlDocument();

            for (int i = Convert.ToInt32(textBox2.Text); i <= Convert.ToInt32(textBox3.Text); i++)
            {
                try
                {

                    string currentUrl = "https://kream.co.kr/products/" + i.ToString();
                    driver.Navigate().GoToUrl(currentUrl);
                    doc.LoadHtml(driver.PageSource);
                    HtmlNode title = doc.DocumentNode.SelectSingleNode("//p[@class='title']");
                    if (title == null) continue;
                    HtmlNode subTitle = doc.DocumentNode.SelectSingleNode("//p[@class='sub-title']");
                    HtmlNode details = doc.DocumentNode.SelectSingleNode("//dl[contains(@class,'detail-product-container')]");
                    HtmlNodeCollection detailProductWrap = details.SelectNodes(".//div[contains(@class, 'product_info')]");
                    HtmlNode brand = doc.DocumentNode.SelectSingleNode("//div[@class='left-container']");
                    HtmlNode brandImg = brand.SelectSingleNode(".//img");
                    HtmlNode brandEngName = brand.SelectSingleNode(".//div[@class='title']");
                    HtmlNode brandKorName = brand.SelectSingleNode(".//div[@class='subtitle']");
                    HtmlNode pic = doc.DocumentNode.SelectSingleNode("//picture[@class='picture product_img']");

                    p = new Product();

                    p.productEngName = title.InnerText.Trim();
                    p.productKorName = subTitle.InnerText.Trim();

                    p.productRelease = Regex.Replace(detailProductWrap[0].InnerText, @"\([^()]*\)", "").Trim();
                    p.productModelNum = detailProductWrap[1].InnerText.Trim();
                    p.productDate = detailProductWrap[2].InnerText.Trim();
                    p.productColor = detailProductWrap[3].InnerText.Trim();
                    p.productBrandEngName = brandEngName.InnerText.Trim();
                    p.productBrandKorName = brandKorName.InnerText.Trim();
                    p.productBrandImgUrl = brandImg.GetAttributeValue("src", "");

                    foreach (HtmlNode n in pic.ChildNodes)
                    {
                        if (n.Name == "source")
                        {
                            p.thumbLinks.Add(n.GetAttributeValue("srcset", ""));
                        }
                        else if (n.Name == "img")
                        {
                            p.thumbLinks.Add(n.GetAttributeValue("src", ""));
                        }
                    }


                    try
                    {
                        IWebElement sizeButton = driver.FindElement(By.XPath("//div[@class='detail-size']"));
                        if (sizeButton != null) sizeButton.Click();
                        Thread.Sleep(500);

                        IWebElement loginDiv = null;
                        try
                        {
                            loginDiv = driver.FindElement(By.XPath("//div[@class='login_btn_box']"));
                        }
                        catch
                        {

                        }
                        if (loginDiv != null)
                        {
                            login(currentUrl);
                            sizeButton = driver.FindElement(By.XPath("//div[@class='detail-size']"));
                            if (sizeButton != null) sizeButton.Click();
                        }
                        doc.LoadHtml(driver.PageSource);
                        HtmlNode list = doc.DocumentNode.SelectSingleNode("//ul[contains(@class,'select_list')]");
                        foreach (HtmlNode li in list.ChildNodes)
                        {
                            p.sizeList.Add(li.SelectSingleNode(".//span[@class='size']").InnerText.Trim());
                        }
                    }
                    catch
                    {
                    }

                    string searchUrl = "https://kream.co.kr/search?keyword=" + p.productEngName + "&tab=products";
                    driver.Navigate().GoToUrl(searchUrl);
                    Thread.Sleep(500);
                    doc.LoadHtml(driver.PageSource);
                    HtmlNode cate = doc.DocumentNode.SelectSingleNode("//a[@class='menu_link']");
                    p.productCateName = cate.InnerText.Trim();
                    pList.Add(p);
                }
                catch
                {
                    continue;
                }

            }

            string json = JsonConvert.SerializeObject(pList);

            string batang = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            System.IO.File.WriteAllText(Path.Combine(batang, "kreamDummy.json"), json);
        }
    }
}
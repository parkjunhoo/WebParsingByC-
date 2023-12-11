using HtmlAgilityPack;
using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KreamCrawling
{
    public partial class Form1 : Form
    {
        public class Product
        {
            public string productCateId;
            public string productEngName;
            public string productKorName;
            public string productRelease;
            public string productBrand;
            public string productColor;
            public string productSize = "";
            public string productModelNum;
            public string productDate;
            public bool productState = true;

            public override string ToString() => 
                "productCateId:" + productCateId + "\r\n" +
                "productEngName:" + productEngName + "\r\n" +
                "productKorName:" + productKorName + "\r\n" +
                "productRelease:" + productRelease + "\r\n" +
                "productBrand:" + productBrand + "\r\n" +
                "productColor:" + productColor + "\r\n" +
                "productSize:" + productSize + "\r\n" +
                "productModelNum:" + productModelNum + "\r\n" +
                "productDate:" + productDate + "\r\n" +
                "productState:" + productState;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Product p;

            for (int i=1; i<=1000; i++)
            {
                string url = "https://kream.co.kr/products/" + i.ToString();

                HtmlWeb web = new HtmlWeb();
                HtmlDocument htmlDoc = web.Load(url);
                HtmlNode title = htmlDoc.DocumentNode.SelectSingleNode("//p[@class='title']");
                if (title == null) continue;
                HtmlNode subTitle = htmlDoc.DocumentNode.SelectSingleNode("//p[@class='sub-title']");
                HtmlNode details = htmlDoc.DocumentNode.SelectSingleNode("//dl[@class='detail-product-container']");
                HtmlNodeCollection detailProductWrap = details.SelectNodes(".//div[contains(@class, 'product_info')]");
                HtmlNode brand = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='left-container']");
                HtmlNode brandName = brand.SelectSingleNode("//div[@class='title']");

                p = new Product();

                p.productEngName = title.InnerText;
                p.productKorName = subTitle.InnerText;

                p.productRelease = Regex.Replace(detailProductWrap[0].InnerText, @"\([^()]*\)", "").Trim();
                p.productModelNum = detailProductWrap[1].InnerText;
                p.productDate = detailProductWrap[2].InnerText;
                p.productColor = detailProductWrap[3].InnerText;
                p.productBrand = brandName.InnerText;

                textBox1.Text += "=================";
                textBox1.Text += p.ToString();
            }
            

            

        }
    }
}
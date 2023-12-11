using HtmlAgilityPack;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KreamCrawling
{
    public partial class Form1 : Form
    {
        public class Product
        {
            public int productCateId = 0;
            public string productEngName;
            public string productKorName;
            public string productRelease;
            public string productBrand;
            public string productColor;
            public string productModelNum;
            public string productDate;
            public List<string> thumbLinks = new List<string>();
            public bool productState = true;

            public override string ToString() => 
                "productCateId:" + productCateId + "\r\n" +
                "productEngName:" + productEngName + "\r\n" +
                "productKorName:" + productKorName + "\r\n" +
                "productRelease:" + productRelease + "\r\n" +
                "productBrand:" + productBrand + "\r\n" +
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

        private void Form1_Load(object sender, EventArgs e)
        {
            Product p;
            List<Product> pList = new List<Product>();

            for (int i=1; i<=230000; i++)
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
                HtmlNode pic = htmlDoc.DocumentNode.SelectSingleNode("//picture[@class='picture product_img']");
                
                p = new Product();

                p.productEngName = title.InnerText;
                p.productKorName = subTitle.InnerText;

                p.productRelease = Regex.Replace(detailProductWrap[0].InnerText, @"\([^()]*\)", "").Trim();
                p.productModelNum = detailProductWrap[1].InnerText;
                p.productDate = detailProductWrap[2].InnerText;
                p.productColor = detailProductWrap[3].InnerText;
                p.productBrand = brandName.InnerText;

                

                foreach(HtmlNode n in pic.ChildNodes)
                {
                    if(n.Name == "source")
                    {
                        p.thumbLinks.Add(n.GetAttributeValue("srcset", ""));
                    } else if (n.Name == "img")
                    {
                        p.thumbLinks.Add(n.GetAttributeValue("src", ""));
                    }
                }
                pList.Add(p);


            }


            string json = JsonConvert.SerializeObject(pList);

            string batang = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            File.WriteAllText(Path.Combine(batang, "kreamDummy.json"), json);

        }
    }
}
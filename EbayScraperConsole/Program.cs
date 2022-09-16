using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace EbayScraperConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GetHtmlAsync();
            Console.ReadLine();
        }
        // as
        private static async void GetHtmlAsync()
        {
            var url = "https://www.ebay.pl/sch/i.html?_nkw=Playstation+5&_in_kw=1&_ex_kw=&_sacat=0&LH_Complete=1&_udlo=&_udhi=&_samilow=&_samihi=&_sadis=10&_fpos=&LH_SALE_CURRENCY=0&_sop=12&_dmd=1&_ipg=200&_fosrp=1";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
                    
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml= htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();


            var ProductListItems = ProductsHtml[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id", "")
                .Contains("item")).ToList();

            ProductsHtml.Sort(Regex.Match(
                    ProductListItems.Descendants("li")
                        .Where(node => node.GetAttributeValue("class", "")
                            .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t', '?')
                    , @"\d+.\d+") + " PLN"
            );
                
            
            foreach (var ProductListItem in ProductListItems)
            {
                // Get Id
                Console.WriteLine(ProductListItem.GetAttributeValue("listingid",""));

                // Get name
                Console.WriteLine(ProductListItem.Descendants("h3")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvtitle")).FirstOrDefault().InnerText.Trim('\r','\n','\t','?')
                    );

                // Get price
                Console.WriteLine(
                    Regex.Match(
                    ProductListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t', '?')
                    ,@"\d+.\d+") + " PLN"
                     );

                // Get listing type
                Console.WriteLine(
                    ProductListItem.Descendants("li")
                      .Where(node => node.GetAttributeValue("class", "")
                      .Equals("lvformat")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t', '?')
                      );

                // Get url
                Console.WriteLine(
                    ProductListItem.Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Trim('\r', '\n', '\t', '?')
                    );


                Console.WriteLine();
            }

            Console.WriteLine(ProductListItems.Count());
            Console.WriteLine();
        }
    }
}

using HtmlAgilityPack;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services
{
    public class HTMLDriverFactory: IFactory<HtmlWeb>
    {
        public HTMLDriverFactory() { }

        public HtmlWeb Get()
        {
            var driver = new HtmlWeb();
            driver.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";

            return driver;
        }
    }
}

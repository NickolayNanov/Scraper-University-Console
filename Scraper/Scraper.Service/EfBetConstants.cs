namespace Scraper.Service.Constants
{
    public static class EfBetConstants
    {
        //public static string CountriessXpath = "//div[@class='level1']/div[{0}]/h2/a";
        public static string CountriessXpath = "//div[@class='level1']/div[{0}]/h2/a";
        public static string LeaguesXpath = "//div[@class='level1']/div[{0}]/div/div[2]/div[{1}]/h2/a[1]";
        public static string MatchXpath = "//div[@class='level1']/div[{0}]/div/div[2]/div[{1}]/div/div[2]/div/div/div/div[2]/table/tbody/tr[{2}]/td[{3}]";
        public static string CategoryXpath = "//ul[@id='AdvSportsNavComponent26-level-0']/li[{0}]/a";
    }
}

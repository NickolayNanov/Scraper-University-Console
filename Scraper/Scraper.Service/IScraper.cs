namespace Scraper.Service
{
    public interface IScraper<T>
    {
        EfBetModel Scrape(decimal maxCoef);
    }
}

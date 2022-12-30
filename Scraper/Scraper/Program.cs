using Scraper.Service;
using System.Text.Json;

IScraper<EfBetModel> scraper = new EfBetScraper();

Console.Write("Please enter max coef: ");
bool parsedInput = decimal.TryParse(Console.ReadLine(), out decimal userInput);

if (!parsedInput)
{
    Console.WriteLine("Please enter decimal number.");
    return;
}

EfBetModel data = scraper.Scrape(userInput);

string jsonString = JsonSerializer.Serialize(data);
File.WriteAllText("./result.json", jsonString);

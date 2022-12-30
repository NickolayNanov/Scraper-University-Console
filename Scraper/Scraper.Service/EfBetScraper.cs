using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Scraper.Service.Constants;
using System.Text;

namespace Scraper.Service
{
    public class EfBetScraper : IScraper<EfBetModel>
    {
        private readonly IWebDriver driver;
        private readonly StringBuilder stringBuilder;

        private int countryCounter = 2;
        private int leagueCounter = 1;
        private int matchCounter = 2;

        public string currentCategory;
        private readonly Dictionary<string, double> navigations;
        private readonly string[] categoriesNoDraw = { "Tennis", "Basketball", "Voleyball", "Baseball", "FightingSports", "TableTennis", "Darts" };

        private ChromeOptions GetChromeDriver()
        {
            var options = new ChromeOptions();

            options.AddArgument("headless");
            options.AddArgument("--silent-launch");
            //options.AddArgument("--disable-gpu");

            return options;
        }

        public EfBetScraper()
        {
            driver = new ChromeDriver(@"./", GetChromeDriver());

            stringBuilder = new StringBuilder();

            navigations = new Dictionary<string, double>()
            {
                { "Football", 282241},
                { "Fifa20", 472758},
                { "Tennis", 280361},
                { "Basketball", 281982},
                { "Voleyball", 280961},
                { "Baseball", 278929},
                { "FightingSports", 279421},
                { "TableTennis", 279021},
                { "Darts", 278926},
                { "IceHockay", 368361},
            };
        }

        public EfBetModel Scrape(decimal coef)
        {
            try
            {
                EfBetModel data = GetData();
                BuildString(data, coef);

                return data;
            }
            catch
            {
                return null;
            }
        }

        private void BuildString(EfBetModel data, decimal coef)
        {
            if (coef == 0)
            {
                foreach (var category in data.Categories)
                {
                    stringBuilder.AppendLine("--------------------------------------------------------------------------------------------------------------");
                    stringBuilder.AppendLine($"---------{category.Name}---------");
                    foreach (var country in category.Countries)
                    {
                        stringBuilder.AppendLine($"{country.Name} - {country.Leagues.Count} Leagues\n");
                        foreach (var league in country.Leagues)
                        {
                            stringBuilder.AppendLine($" *{league.LeagueName} - {league.Matches.Count} Matches\n");
                            foreach (var match in league.Matches)
                            {
                                stringBuilder.AppendLine($"     **{match.Teams}|{match.Date} - {match.FirstTeamCoef} - {(match.Draw == 0 ? "" : match.Draw.ToString())} - {match.SecondTeamCoef}\n");
                            }
                        }
                        stringBuilder.AppendLine();
                    }
                }
            }
            else
            {
                IEnumerable<EfBetMatch> matches = data.Categories.SelectMany(x => x.Countries).SelectMany(x => x.Leagues.SelectMany(l => l.Matches.Where(m => m.FirstTeamCoef <= coef || m.SecondTeamCoef <= coef)));
                foreach (var match in matches)
                {
                    stringBuilder.AppendLine($"{match.Category}/{match.Counrty}/{match.League}                              {match.Teams}|{match.Date} - {match.FirstTeamCoef} - {(match.Draw == 0 ? "" : match.Draw.ToString())} - {match.SecondTeamCoef}\n");
                }
            }
        }

        private EfBetModel GetData()
        {
            EfBetModel bet = new EfBetModel();

            try
            {
                var url = "https://www.efbet.com/BG/sports#bo-navigation={0}.1&action=market-group-list";
                Pause();
                foreach (var category in navigations)
                {
                    driver.Navigate().GoToUrl(string.Format(url, category.Value));
                    Thread.Sleep(2000);
                    var categoryModel = new EfBetCategory(category.Key);
                    currentCategory = category.Key;
                    if (category.Key == "Baseball")
                    {

                    }

                    categoryModel.Countries.AddRange(GetCountries(category.Key));
                    if (categoryModel.Countries.Count != 0)
                    {
                        bet.Categories.Add(categoryModel);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return bet;
        }

        private static void Pause()
        {
            Thread.Sleep(1500);
        }

        private List<EfBetCountry> GetCountries(string categoryName)
        {
            EfBetCountry country = null;
            countryCounter = 2;
            var countries = new List<EfBetCountry>();

            while ((country = GetCountry(categoryName)) != null)
            {
                countries.Add(country);
            }
            return countries;
        }

        private EfBetCountry GetCountry(string categoryName)
        {
            EfBetCountry country = null;
            try
            {
                var countryName = GetElement(string.Format(EfBetConstants.CountriessXpath, countryCounter));
                country = new EfBetCountry(countryName);
                leagueCounter = 1;
                while (true)
                {
                    try
                    {
                        EfBetLeague league = GetLeague(categoryName, countryName);

                        leagueCounter++;
                        country.Leagues.Add(league);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }
                countryCounter++;
                return country;
            }
            catch
            {
                return country;
            }
        }

        private EfBetLeague GetLeague(string categoryName, string countryName)
        {
            var leagueName = GetElement(string.Format(EfBetConstants.LeaguesXpath, countryCounter, leagueCounter));
            var league = new EfBetLeague(leagueName);
            matchCounter = 2;
            while (true)
            {
                try
                {
                    EfBetMatch match = GetMatch();

                    if (match == null)
                    {
                        break;
                    }
                    else
                    {
                        match.League = leagueName;
                        match.Counrty = countryName;
                        match.Category = categoryName;
                    }
                    matchCounter++;
                    league.Matches.Add(match);
                }
                catch (Exception ex)
                {
                    break;
                }
            }

            return league;
        }

        private EfBetMatch GetMatch()
        {
            EfBetMatch match = null;
            try
            {
                if (categoriesNoDraw.Contains(currentCategory))
                {
                    var matchDate = GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 1));
                    var matchTeams = GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 2));
                    var firstTeamCoef = decimal.Parse(GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 3)));
                    var secondTeamCoef = decimal.Parse(GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 4)));
                    match = new EfBetMatch(matchDate, matchTeams, firstTeamCoef, secondTeamCoef);
                }
                else
                {
                    var matchDate = GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 1));
                    var matchTeams = GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 2));
                    var firstTeamCoef = decimal.Parse(GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 3)));
                    var drawCoef = decimal.Parse(GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 4)));
                    var secondTeamCoef = decimal.Parse(GetElement(string.Format(EfBetConstants.MatchXpath, countryCounter, leagueCounter, matchCounter, 5)));
                    match = new EfBetMatch(matchDate, matchTeams, firstTeamCoef, drawCoef, secondTeamCoef);
                }
            }
            catch
            {
                match = null;
            }

            return match;
        }

        private string GetElement(string Xpath)
        {
            return driver.FindElement(By.XPath(Xpath)).Text;
        }
    }
}

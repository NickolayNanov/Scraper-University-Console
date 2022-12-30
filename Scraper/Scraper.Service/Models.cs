using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.Service
{
    public class EfBetModel
    {
        public EfBetModel()
        {
            this.Categories = new List<EfBetCategory>();
        }

        public List<EfBetCategory> Categories { get; set; }
    }

    public class EfBetCategory
    {
        public EfBetCategory(string name)
        {
            this.Name = name;
            this.Countries = new List<EfBetCountry>();
        }

        public string Name { get; set; }

        public List<EfBetCountry> Countries { get; set; }
    }

    public class EfBetCountry
    {
        public EfBetCountry(string name)
        {
            this.Name = name;
            this.Leagues = new List<EfBetLeague>();
        }

        public string Name { get; set; }

        public List<EfBetLeague> Leagues { get; set; }
    }

    public class EfBetLeague
    {
        public EfBetLeague(string leagueName)
        {
            this.LeagueName = leagueName;
            this.Matches = new List<EfBetMatch>();
        }

        public string LeagueName { get; set; }

        public List<EfBetMatch> Matches { get; set; }
    }

    public class EfBetMatch
    {
        public EfBetMatch(string date, string teams, decimal c1, decimal d, decimal c2)
        {
            this.Date = date;
            this.Teams = teams;
            this.FirstTeamCoef = c1;
            this.Draw = d;
            this.SecondTeamCoef = c2;
        }

        public EfBetMatch(string date, string teams, decimal c1, decimal c2)
        {
            this.Date = date;
            this.Teams = teams;
            this.FirstTeamCoef = c1;
            this.SecondTeamCoef = c2;
        }

        public string Category { get; set; }

        public string Counrty { get; set; }

        public string League { get; set; }

        public string Date { get; set; }

        public string Teams { get; set; }

        public decimal FirstTeamCoef { get; set; }

        public decimal Draw { get; set; }

        public decimal SecondTeamCoef { get; set; }
    }
}

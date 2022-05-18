using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gumayusi_Orbwalker.Core.League.Commons
{
    public class ApiScraper
    {
        public static string Scrape()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                string url = Uri.EscapeUriString("https://127.0.0.1:2999/liveclientdata/allgamedata");
                string doc = "";
                using (WebClient client = new WebClient())
                {
                    doc = client.DownloadString(url);
                }
                return doc;
            }
            catch
            {
                return "";
            }
        }
        private static string ScrapeActivePlayer()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                string url = Uri.EscapeUriString("https://127.0.0.1:2999/liveclientdata/activeplayer");
                string doc = "";
                using (WebClient client = new WebClient())
                {
                    doc = client.DownloadString(url);
                }
                return doc;
            }
            catch
            {
                return "";
            }
        }
        public static string PlayerCurrentHealth()
        {
            string temp = ParseResult(ScrapeActivePlayer(), "currentHealth\": ", ",");
            return temp.Replace(".", ",");
        }
        public static string PlayerMaxHealth()
        {
            string temp = ParseResult(ScrapeActivePlayer(), "maxHealth\": ", ",");
            return temp.Replace(".", ",");
        }

        public static string PlayerAttackSpeed()
        {
            string temp = ParseResult(Scrape(), "attackSpeed\": ", ",");
            return temp.Replace(".", ",");
        }

        public static string PlayerAttackRange()
        {
            return ParseResult(Scrape(), "attackRange\": ", ",");
        }

        public static string GameTime()
        {
            string temp = ParseResult(Scrape(), "gameTime\": ", ",");
            return temp.Replace(".", ",");
        }

        public static string PlayerChampionName()
        {
            string temp = ParseResult(Scrape(), "championName\": \"", "\"");
            return temp;
        }

        public static string PlayerCurrentMana()
        {
            string temp = ParseResult(Scrape(), "resourceValue\": ", ",");
            return temp.Replace(".", ","); ;
        }

        public static string ParseResult(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            return "";
        }
    }
}

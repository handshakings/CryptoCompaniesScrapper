using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CryptoCompaniesScrapper
{
    public class EmailFinder
    {
        public MatchCollection SearchEmailFromString0(string txt)
        {
            const string RegexPattern =
           @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
             + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
            MatchCollection matches = Regex.Matches(txt, RegexPattern, RegexOptions.IgnoreCase);
            return matches;
        }
        public MatchCollection SearchEmailFromString1(string txt)
        {
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            MatchCollection matches = emailRegex.Matches(txt);
            return matches;
        }
        public MatchCollection SearchEmailFromString2(string txt)
        {
            string RegexPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z]+(\.[a-zA-Z0-9]+)+";
            MatchCollection matches = Regex.Matches(txt, RegexPattern, RegexOptions.IgnoreCase);
            return matches;
        }
        public MatchCollection SearchEmailFromString3(string txt)
        {
            string RegexPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";
            MatchCollection matches = Regex.Matches(txt, RegexPattern, RegexOptions.IgnoreCase);
            return matches;
        }

        public List<string> SearchEmail(string content)
        {
            MatchCollection emails = SearchEmailFromString0(content);
            if (emails.Count == 0)
            {
                emails = SearchEmailFromString1(content);
            }
            else if (emails.Count == 0)
            {
                emails = SearchEmailFromString2(content);
            }
            else if (emails.Count == 0)
            {
                emails = SearchEmailFromString3(content);
            }
            List<string> emailList = new List<string>();

            foreach (Match match in emails)
            {
                emailList.Add(match.Value);
            }
            return emailList.Distinct().ToList();
        }


        public string SearchLinks(string content)
        {
            MatchCollection mc = Regex.Matches(content, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");

            string sLinks = string.Empty;
            foreach (Match link in mc)
            {
                if (!sLinks.Contains(link.Value))
                {
                    link.Value.Replace(',', ' ');
                    sLinks = sLinks + "\n" + link.Value;
                }
            }
            if (sLinks.Length > 2)
            {
                sLinks = sLinks.Remove(0, 1);
            }
            return sLinks;
        }
    }
}

using System.Collections.Generic;

namespace CryptoCompaniesScrapper
{
    public class Model
    {
        public List<string> WebsiteUrls { get; set; }
        public string Email { get; set; }
    }


    public class CompanyData
    {
        public string CompanyName { get; set; }
        public string CompanyLink { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyEmail { get; set; }


    }

}

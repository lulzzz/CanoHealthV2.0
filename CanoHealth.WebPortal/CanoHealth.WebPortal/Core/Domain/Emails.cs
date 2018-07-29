using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Emails
    {
        // properties
        public List<string> To { get; set; }

        public List<string> CC { get; set; }

        public List<string> BCC { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public Emails()
        {
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
        }
    }
}
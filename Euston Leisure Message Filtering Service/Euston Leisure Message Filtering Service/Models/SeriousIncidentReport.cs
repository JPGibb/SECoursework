using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euston_Leisure_Message_Filtering_Service.Exceptions;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class SeriousIncidentReport : Email
    {
        private string sportCentreCode = string.Empty;
        private string natureOfIncident = string.Empty;
        private string date = string.Empty;

        public SeriousIncidentReport(string messageId, string messageBody, string date)
        {
            this.messageId = messageId;
            string[] s = messageBody.Split('\n', '\r');

            this.sender = s[0];
            this.subject = s[2];
            this.date = date;
            this.sportCentreCode = s[4];
            this.natureOfIncident = s[6];

            for (int i = 7; i < s.Length; ++i)
            {
                this.messageBody += s[i];
            }

            if (!validateEmail())
            {
                throw new InvalidEmailException();
            }

            removeLinks();

            MessageBox.Show("Id : " + this.messageId + "\n" +
                "Sender " + this.sender + "\n" +
                "Subject " + this.subject + "\n" +
                "SCC " + this.sportCentreCode + "\n" + 
                "NOI " + this.natureOfIncident + "\n" +
                "Body " + this.messageBody);
        }
    }
}

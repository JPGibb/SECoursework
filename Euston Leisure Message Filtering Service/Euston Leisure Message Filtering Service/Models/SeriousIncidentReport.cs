using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euston_Leisure_Message_Filtering_Service.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class SeriousIncidentReport : Email
    {
        //Enum that represents the possible incidents that may be recorded
        public enum NatureOfIncident
        {
            
            theft_of_property,
            staff_attack,
            device_damage,
            raid,
            customer_attack,
            staff_abuse,
            bomb_threat,
            terrorism,
            suspicious_inicdent,
            sport_injury,
            personal_info_leak
        }

        public string sportCentreCode = string.Empty;
        public NatureOfIncident natureOfIncident;
        public DateTime date;

        public SeriousIncidentReport(string messageId, string messageBody, string date)
        {
            this.messageId = messageId;
            string[] s = messageBody.Split('\n', '\r');
           foreach(string x in s)
            {
                MessageBox.Show(x);
            }
            this.sender = s[0];
            this.subject = s[2];
            try
            {
                this.date = Convert.ToDateTime(date);               
            }
            catch
            {
                throw new FailedToCreateMessageException();
            }
            this.sportCentreCode = s[4];
            try
            {
                //string n = s[6].Replace(" ", "_").ToLower();
               // MessageBox.Show(s[6]);
                //this.natureOfIncident = (NatureOfIncident)Enum.Parse(typeof(NatureOfIncident), n);
                MessageBox.Show(this.natureOfIncident.ToString(), "Nature of incident");
            }
            catch
            {
                throw new FailedToCreateMessageException();
            }

            for (int i = 7; i < s.Length; ++i)
            {
                this.messageBody += s[i];
            }

            if (!validateEmail())
            {
                throw new InvalidEmailException();
            }

            removeLinks();
        }

        //Returns a string array conataining the nature of the sport centre code and nature of incident
        public string[] getSccNoc()
        {
            string[] s = new string[2];

            s[0] = sportCentreCode;
            s[1] = natureOfIncident.ToString();

            return s;
        }
    }
}

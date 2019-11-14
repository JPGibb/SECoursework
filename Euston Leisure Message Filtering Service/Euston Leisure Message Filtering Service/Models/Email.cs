using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Email : Message
    {
        private string subject = string.Empty;

        public Email(string messageId, string messageBody)
        {
            this.messageId = messageId;

            string[] s = messageBody.Split('\n');
            this.sender = s[0];
            this.subject = s[1];
            
            for(int i = 2; i < s.Length; ++i)
            {
                this.messageBody += s[i];
            }

            validateEmail();
            //MessageBox.Show("Id : " + this.messageId + "\n" +
            //    "Sender " + this.sender + "\n" + 
            //    "Subject " + this.subject + "\n" + 
            //    "Body " + this.messageBody);
        }

        private bool validateEmail()
        {
            return true;
        }
    }
}

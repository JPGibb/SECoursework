using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Email : Message
    {
        private string subject = string.Empty;

        public Email(string messageId, string messageBody)
        {
            this.messageId = messageId;

            string[] s = messageBody.Split('\n', '\r');
            
            this.sender = s[0];
            this.subject = s[1];
            
            for(int i = 2; i < s.Length; ++i)
            {
                this.messageBody += s[i];
            }

           bool success = validateEmail();
           
            //MessageBox.Show("Id : " + this.messageId + "\n" +
            //    "Sender " + this.sender + "\n" + 
            //    "Subject " + this.subject + "\n" + 
            //    "Body " + this.messageBody);
        }

        private bool validateEmail()
        {
            try
            {
                MailAddress address = new MailAddress(this.sender);
                //MessageBox.Show("Valid");
                return true; // Is a valid email address so return true
            }
            catch
            {
                //MessageBox.Show("Invalid: " + this.sender + " sldkghk");
                return false; // Is not a valid email address, so return false
            }
        }
    }
}

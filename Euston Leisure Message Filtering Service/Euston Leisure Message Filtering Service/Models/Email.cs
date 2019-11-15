using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Euston_Leisure_Message_Filtering_Service.Exceptions;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Email : Message
    {
        protected string subject = string.Empty;

        public Email() { }

        public Email(string messageId, string messageBody)
        {
            this.messageId = messageId;

            string[] s = messageBody.Split('\n', '\r');
            
            this.sender = s[0];
            this.subject = s[2];
            
            for(int i = 3; i < s.Length; ++i)
            {    
                this.messageBody += s[i];
            }

           if(!validateEmail())
            {
                throw new InvalidEmailException();
            }

            removeLinks();

            //MessageBox.Show("Id : " + this.messageId + "\n" +
            //    "Sender " + this.sender + "\n" +
            //    "Subject " + this.subject + "\n" +
            //    "Body " + this.messageBody);
        }

        protected bool validateEmail()
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

        protected void removeLinks()
        {
            string newMessage = string.Empty;
            foreach(string s in messageBody.Split(' ', '\n', '\r'))
            {
                if(!isUrl(s))
                {
                    newMessage += s + " ";
                }
                else
                {
                    newMessage += "<URL Quarantined> ";
                }
            }
            messageBody = newMessage;
        }

        protected bool isUrl(string s)
        {
            return Uri.IsWellFormedUriString(s, UriKind.Absolute );
        }
    }
}

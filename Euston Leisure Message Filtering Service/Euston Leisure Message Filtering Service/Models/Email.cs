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
    //Represents an email message
    class Email : Message
    {
        public string subject = string.Empty;

        public Email() { }

        public Email(string messageId, string messageBody)
        {
            this.messageId = messageId;

            string[] s = messageBody.Split('\n', '\r');
            
            this.sender = s[0].Replace("\r", string.Empty);
            this.subject = s[1];
            
            for(int i = 2; i < s.Length; ++i)
            {    
                this.messageBody += s[i];
            }

            if(this.sender.ToCharArray().Count() > 20 || this.messageBody.ToCharArray().Count() > 1024)
            {
                throw new ToManyCharactersException();
            }

            if(!validateEmail())
            {
                throw new InvalidEmailException();
            }

            removeLinks();
        }

        //Ensures the sender is valid email address
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

        //Removes any links from the message
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

        //Returns if a string is  valid URL
        protected bool isUrl(string s)
        {
            return Uri.IsWellFormedUriString(s, UriKind.Absolute );
        }

        //Returns a string representation of the message
        public string getDetails()
        {
            string s = string.Empty;

            s += this.sender + "\n";
            s += this.subject + "\n";
            s += this.messageBody;

            return s;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Sms : Message
    {
        public Sms(string message_id, string message_body, string sender, Dictionary<string,string> text_words)
        {
            this.messageId = message_id;
            this.messageBody = message_body;
            this.messageBody = expand(text_words);
            this.sender = sender;
        }

        private string expand(Dictionary<string,string> text_words)
        {
            string[] s = this.messageBody.Split('\n', '\r', ' ');
            string newMessageBody = string.Empty;

            foreach(string i in s)
            {
                newMessageBody += i + " ";

                if(text_words.ContainsKey(i))
                {
                    newMessageBody +=  '<' + text_words[i.ToLower()] + '>' + " ";
                }    
            }

            
            MessageBox.Show(newMessageBody);
            return newMessageBody;
        }
    }
}

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euston_Leisure_Message_Filtering_Service.Exceptions;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Tweet : Message
    {
        public Tweet(string MessageIdTextBox, string MessageBodyTextBox, Dictionary<string, string> text_words)
        {
            this.messageId = MessageIdTextBox;

            string[] s = MessageBodyTextBox.Split('\n');

            this.sender = s[0].Remove(0, 1);
            if (sender.Length > 15)
            {
                throw new FailedToCreateMessageException();
            }

            for(int i = 1; i < s.Length; ++i)
            {
                this.messageBody += s[i];
            }
            
            if(this.messageBody.Length > 140)
            {
                throw new FailedToCreateMessageException();
            }

            this.messageBody = expand(text_words);
        }

        //Expand the ant abbreviations in the message body
        private string expand(Dictionary<string, string> text_words)
        {
            string[] s = this.messageBody.Split('\n', '\r', ' ');
            string newMessageBody = string.Empty;

            foreach (string i in s)
            {
                newMessageBody += i + " ";

                if (text_words.ContainsKey(i))
                {
                    newMessageBody += '<' + text_words[i.ToLower()] + '>' + " ";
                }
            }

            MessageBox.Show(newMessageBody);
            return newMessageBody;
        }
    }
}

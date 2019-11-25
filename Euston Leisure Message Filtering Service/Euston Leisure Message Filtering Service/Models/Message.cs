using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    public class Message
    {
        public string messageId = string.Empty;
        public string messageBody = string.Empty;
        public string sender = string.Empty;

        public Message() { }

        public Message(string messageId, string messageBody)
        {
            this.messageId = messageId;
            this.messageBody = messageBody;
        }

        //Returns the id of the message
        public string getId()
        {
            return messageId;
        }

        //Returns the body of ht message
        public string getBody()
        {
            return messageBody;
        }

        //Returns a string representation of tthe message
        public string getDetails()
        {
            string s = string.Empty;

            s += sender + "\n";
            s += messageBody;
            return s;
        }
    }
}

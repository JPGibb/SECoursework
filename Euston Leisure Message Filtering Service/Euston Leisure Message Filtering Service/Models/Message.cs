using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    abstract class Message
    {
        //protected string messageId = string.Empty;
        //protected string messageBody = string.Empty;
        //protected string sender = string.Empty;

        public string messageId = string.Empty;
        public string messageBody = string.Empty;
        public string sender = string.Empty;

        public Message() { }

        public Message(string messageId, string messageBody)
        {
            this.messageId = messageId;
            this.messageBody = messageBody;
        }

        public string getId()
        {
            return messageId;
        }

        public string getBody()
        {
            return messageBody;
        }
    }
}

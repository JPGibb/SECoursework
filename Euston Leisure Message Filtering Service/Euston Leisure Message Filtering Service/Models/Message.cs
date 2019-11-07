using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Message
    {
        private string messageId = string.Empty;
        private string messageBody = string.Empty;

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

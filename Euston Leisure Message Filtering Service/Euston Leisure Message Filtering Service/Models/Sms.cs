using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Sms : Message
    {
        public Sms(string messageId, string messageBody)
        {
            this.messageId = messageId;
            this.messageBody = messageBody;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Euston_Leisure_Message_Filtering_Service.Commands;
using Euston_Leisure_Message_Filtering_Service.Models;

namespace Euston_Leisure_Message_Filtering_Service.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        //Text Blocks
        public  string MessageIdTextBlock { get; private set; }
        public string MessageBodyTextBlock { get; private set; }

        //Text bodies
        public string MessageIdTextBox { get; set; }
        public string MessageBodyTextBox { get; set; }

        //Buttons
        public string SubmitButtonText { get; set; }
        public ICommand SubmitButtonCommand { get; private set; }

        public MainViewModel()
        {
            MessageIdTextBlock = "Message ID";
            MessageBodyTextBlock = "Message Body";

            SubmitButtonText = "Submit";

            MessageIdTextBox = string.Empty;
            MessageBodyTextBox = string.Empty;

            SubmitButtonCommand = new RelayCommand(SubmitButtonClick);
        }

        private void SubmitButtonClick()
        {
            // MessageBox.Show("The submit Button has been clicked");

            if(MessageIdTextBox.Length != 10)
            {
                MessageBox.Show("Error You must enter a Message ID");
                return;
            }
            if(MessageBodyTextBox == string.Empty)
            {
                MessageBox.Show("Error you must enter a message body");
                return;
            }

            Message m = new Message(MessageIdTextBox, MessageBodyTextBox);

            MessageBox.Show($"Id: {m.getId()} \nBody {m.getBody()}");
        }

    }
}

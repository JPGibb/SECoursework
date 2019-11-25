using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    //The model that stores all the information in the program
    class Model
    {
        Dictionary<string, string> textWords = new Dictionary<string, string>();
        Dictionary<string, int> hashtags = new Dictionary<string, int>();
        Dictionary<string, int> mentions = new Dictionary<string, int>();
        
        List<Message> messages = new List<Message>();
        List<String[]> sirList = new List<String[]>();

        public Model()
        {
            loadTextWords();
        }

        //Opens the csv file that contains all the abreviations
        private void loadTextWords()
        {
            try
            {
                StreamReader input_file = new StreamReader(File.OpenRead("../../Resources/textwords.csv"));
                while (!input_file.EndOfStream)
                {
                    string line = input_file.ReadLine();
                    string[] s = line.Split(',');

                    textWords.Add(s[0].ToLower(), s[1]);
                }
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("Failed to open csv file");
            }
        }

        //Adds a message to the messages List
        public void addMessage(Message m)
        {
            messages.Add(m);
            //MessageBox.Show(messages.Count().ToString());
        }

        //returns the messages list
        public List<Message> getMessages()
        {
            return messages;
        }

        //Adds data to the sirList
        public void addSir(string[] s)
        {
            sirList.Add(s);
        }

        //Returns the sirList
        public List<String[]> getSirList()
        {
            return sirList;
        }

        //Returns the textWords dictionary
        public Dictionary<string,string> getTextWords()
        {
            return textWords;
        }

        //Returns the hashtags dictionary
        public Dictionary<string, int> getHashtags()
        {
            return hashtags;
        }

        //adds a hashtag to the hashtags Dictionary
        public void addHashtag(string h)
        {
            if(hashtags.ContainsKey(h))
            {
                hashtags[h]++;
            }
            else
            {
                hashtags.Add(h, 1);
            }
        }

        //Returns the mentions dictionary
        public Dictionary<string, int> getMentions()
        {
            return mentions;
        }

        //Adds a mention to the mentions dictionary
        public void addMention(string m)
        {
            if(mentions.ContainsKey(m))
            {
                mentions[m]++;           
            }
            else
            {
                mentions.Add(m, 1);
            }
        }
    }
}

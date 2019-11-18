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
    class Model
    {
        Dictionary<string, string> textWords = new Dictionary<string, string>();
        Dictionary<string, int> hashtags = new Dictionary<string, int>();
        Dictionary<string, int> mentions = new Dictionary<string, int>();
        
        List<Message> messages = new List<Message>();       

        public Model()
        {
            loadTextWords();
        }

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

        public void addMessage(Message m)
        {
            messages.Add(m);
            //MessageBox.Show(messages.Count().ToString());
        }

        public Dictionary<string,string> getTextWords()
        {
            return textWords;
        }

        public Dictionary<string, int> getHashtags()
        {
            return hashtags;
        }

        public void addHashtag(string h)
        {
            if(hashtags.ContainsKey(h))
            {
                hashtags[h]++;
                //MessageBox.Show("The hash tag " + h + " has appeared " + hashtags[h] + " times");
            }
            else
            {
                hashtags.Add(h, 1);
                //MessageBox.Show("Adding the hastag " + h);
            }
        }

        public Dictionary<string, int> getMentions()
        {
            return mentions;
        }

        public void addMention(string m)
        {
            if(mentions.ContainsKey(m))
            {
                mentions[m]++;
                //MessageBox.Show("The user " + m + " has appeared " + mentions[m] + " times");
            }
            else
            {
                mentions.Add(m, 1);
                //MessageBox.Show("Adding " + m + " to the mentions list");
            }
        }
    }
}

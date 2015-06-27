using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InformationRetrieval
{
    public class CompressedVocabulary
    {
        public CompressedVocabulary()
        {
            Vocabulary = new StringBuilder();
            Frequency = new List<int>();
            ListPointers = new List<int>();
            TermPointers = new List<int>();
        }

        public StringBuilder Vocabulary { get; set; }
        public List<int> Frequency { get; set; }
        public List<int> ListPointers { get; set; }
        public List<int> TermPointers { get; set; }

        public void Save()
        {
            var sw = new StreamWriter(Environment.CurrentDirectory + @"\compressedVocabulary.txt");
            sw.WriteLine(Vocabulary);
            sw.Flush();

            var sw2 = new StreamWriter(Environment.CurrentDirectory + @"\TermPointers.txt");
            foreach (int pointer in TermPointers)
            {
                sw2.WriteLine(pointer);
            }
            sw2.Flush();
        }
    }
}
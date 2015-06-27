using System;
using System.Collections.Generic;
using System.IO;

namespace InformationRetrieval
{
    internal class InvertedIndex
    {
        protected string FileName;
        protected DocumentParser docParser;
        protected SortedDictionary<string, List<int>> index;


        public InvertedIndex(string path)
        {
            index = new SortedDictionary<string, List<int>>();
            docParser = new DocumentParser(path);
            FileName = @"\InvIndex.txt";
        }


        public virtual void Create()
        {
            for (int docId = 0; docId < docParser.DocumentsCount; docId++)
            {
                string[] wordsInDoc = docParser.GetWordsFromEpub(docId);
                for (int i = 0; i < wordsInDoc.Length; i++)
                {
                    wordsInDoc[i] = wordsInDoc[i].ToLower();
                }
                AddWordsToIndex(wordsInDoc, docId);
                Console.WriteLine("docid " + docId);
            }
        }

        protected virtual void AddWordsToIndex(string[] wordsInDoc, int docId)
        {
            foreach (string word in wordsInDoc)
            {
                if (!index.ContainsKey(word))
                {
                    index.Add(word, new List<int>());
                }
                if (!index[word].Contains(docId))
                {
                    index[word].Add(docId);
                }
            }
        }

        public virtual void SaveToFile()
        {
            using (var streamWriter = new StreamWriter(Environment.CurrentDirectory + FileName))
            {
                foreach (var element in index)
                {
                    streamWriter.WriteLine(element.Key + "      " + element.Value.Count);
                    element.Value.Sort();
                    foreach (int docId in element.Value)
                    {
                        streamWriter.Write(docId + " ");
                    }
                    streamWriter.WriteLine();
                }
            }
        }
    }
}
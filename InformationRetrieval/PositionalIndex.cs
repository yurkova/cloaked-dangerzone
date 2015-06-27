using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InformationRetrieval
{
    internal class PositionalIndex : InvertedIndex
    {
        private readonly SortedDictionary<string, SortedDictionary<int, List<int>>> posIndex;

        public PositionalIndex(string path) : base(path)
        {
            FileName = @"\PositionalIndex.txt";
            posIndex = new SortedDictionary<string, SortedDictionary<int, List<int>>>();
        }

        protected override void AddWordsToIndex(string[] wordsInDoc, int docId)
        {
            for (int i = 0; i < wordsInDoc.Length - 1; i++)
            {
                string word = wordsInDoc[i];
                if (!posIndex.ContainsKey(word))
                {
                    posIndex.Add(word, new SortedDictionary<int, List<int>>());
                }
                if (!posIndex[word].ContainsKey(docId))
                {
                    posIndex[word].Add(docId, new List<int>());
                }
                posIndex[word][docId].Add(i);
            }
        }

        public override void SaveToFile()
        {
            using (var streamWriter = new StreamWriter(Environment.CurrentDirectory + FileName))
            {
                foreach (var term in posIndex)
                {
                    streamWriter.WriteLine(term.Key + " :      " + term.Value.Sum(x => x.Value.Count));
                    foreach (var posting in term.Value)
                    {
                        streamWriter.Write(posting.Key + " : ");
                        foreach (int position in posting.Value)
                        {
                            streamWriter.Write(position + " ");
                        }
                        streamWriter.WriteLine();
                    }
                    streamWriter.WriteLine();
                }
            }
        }
    }
}
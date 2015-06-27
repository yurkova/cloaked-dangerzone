using System.Collections.Generic;

namespace InformationRetrieval
{
    internal class BiwordIndex : InvertedIndex
    {
        public BiwordIndex(string path) : base(path)
        {
            FileName = @"\BiwordIndex.txt";
        }

        protected override void AddWordsToIndex(string[] wordsInDoc, int docId)
        {
            for (int i = 0; i < wordsInDoc.Length - 1; i++)
            {
                string biword = wordsInDoc[i] + " " + wordsInDoc[i + 1];
                if (!index.ContainsKey(biword))
                {
                    index.Add(biword, new List<int>());
                }
                if (!index[biword].Contains(docId))
                {
                    index[biword].Add(docId);
                }
            }
        }
    }
}
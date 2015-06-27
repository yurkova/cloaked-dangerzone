using System;
using System.Collections.Generic;
using System.IO;

namespace InformationRetrieval
{
    internal class IncidenceMatrix
    {
        private readonly DocumentParser docParser;
        private SortedDictionary<string, bool[]> incidenceMatrix;

        public IncidenceMatrix(string path)
        {
            docParser = new DocumentParser(path);
        }

        public void Create()
        {
            incidenceMatrix = new SortedDictionary<string, bool[]>();
            for (int docId = 0; docId < docParser.DocumentsCount; docId++)
            {
                string[] wordsInDoc = docParser.GetWordsFromEpub(docId);
                for (int i = 0; i < wordsInDoc.Length; i++)
                {
                    wordsInDoc[i] = wordsInDoc[i].ToLower();
                }
                foreach (string word in wordsInDoc)
                {
                    if (!incidenceMatrix.ContainsKey(word))
                    {
                        incidenceMatrix.Add(word, new bool[docParser.DocumentsCount]);
                    }
                    incidenceMatrix[word][docId] = true;
                }
            }
        }

        public void SaveToFile()
        {
            using (var streamWriter = new StreamWriter(Environment.CurrentDirectory + @"\IncMatrix.txt"))
            {
                for (int i = 0; i < docParser.DocumentsCount; i++)
                {
                    streamWriter.Write(i + "\t");
                }
                streamWriter.WriteLine();
                foreach (var element in incidenceMatrix)
                {
                    foreach (bool val in element.Value)
                    {
                        streamWriter.Write(val ? "1\t" : "0\t");
                    }
                    streamWriter.WriteLine(element.Key);
                }
            }
        }
    }
}
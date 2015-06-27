using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InformationRetrieval
{
    public static class Compression
    {
        public static CompressedVocabulary CompressVocabulary(string path)
        {
            var compressedVocabulary = new CompressedVocabulary();

            var postingLists = new List<BitArray>();

            var sr = new StreamReader(path);
            var sw = new StreamWriter(Environment.CurrentDirectory + @"\PostingLists.txt");
            string s;
            int wordsCounter = 0;
            int charCounter = 0;
            while ((s = sr.ReadLine()) != null)
            {
                string[] termAndFrequency = s.Trim().Split(new[] {' ', '\t'},
                    StringSplitOptions.RemoveEmptyEntries);
                compressedVocabulary.Vocabulary.Append(termAndFrequency[0]);
                compressedVocabulary.Frequency.Add(Convert.ToInt32(termAndFrequency[1]));
                compressedVocabulary.TermPointers.Add(charCounter);
                charCounter += termAndFrequency[0].Length;

                int[] posList = sr.ReadLine()
                    .Trim()
                    .Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => Convert.ToInt32(n) + 1)
                    .ToArray();


                compressedVocabulary.ListPointers.Add(wordsCounter);
                foreach (int el in posList)
                {
                    sw.Write(el + " ");
                }
                sw.WriteLine();
                postingLists.Add(CompressPostingList(posList));
                wordsCounter++;
            }
            sw.Close();
            SavePostingLists(postingLists);
            return compressedVocabulary;
        }


        private static BitArray CompressPostingList(int[] posList)
        {
            var bitsArrays = new BitArray[posList.Length];
            for (int i = 0; i < posList.Length; i++)
            {
                var binary = new List<int>();
                while (posList[i] != 0)
                {
                    binary.Add(posList[i]%2);
                    posList[i] /= 2;
                }
                binary.Reverse();

                if (binary.Count != 0)
                {
                    bitsArrays[i] = new BitArray(binary.Count*2 - 1);

                    for (int j = 0; j < binary.Count - 1; j++)
                    {
                        bitsArrays[i][j] = true;
                    }
                    bitsArrays[i][binary.Count - 1] = false;
                    for (int j = 1; j < binary.Count; j++)
                    {
                        bitsArrays[i][j + binary.Count - 1] = binary[j] == 1;
                    }
                }
            }

            int resultLength = bitsArrays.Select(n => n.Length).Sum();
            var result = new BitArray(resultLength);

            int k = 0;
            for (int i = 0; i < bitsArrays.Length; i++)
            {
                for (int j = 0; j < bitsArrays[i].Length; j++)
                {
                    result[k++] = bitsArrays[i][j];
                }
            }
            return result;
        }


        private static void SavePostingLists(List<BitArray> postingLists)
        {
            using (var bw = new BinaryWriter(File.OpenWrite(Environment.CurrentDirectory + @"\poslists.txt")))
            {
                foreach (BitArray postingList in postingLists)
                {
                    for (int i = 0; i < postingList.Length; i++)
                        bw.Write(postingList[i]);
                }
                bw.Flush();
            }
        }
    }
}
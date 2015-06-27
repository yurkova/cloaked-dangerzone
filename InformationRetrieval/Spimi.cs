using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace InformationRetrieval
{
    internal class Spimi : InvertedIndex
    {
        private const int BlockSize = 5000000;
        private const int BufferSize = 50000;
        private readonly string logPath = Environment.CurrentDirectory + @"\SpimiLog.txt";
        private readonly StreamWriter logStrWriter;
        private int blockCounter;
        private List<List<string>> buffers;
        private BinaryHeap<HeapElement> heap;
        private bool[] isBlockFinished;
        private List<StreamReader> strReaders;
        private StreamWriter streamWriter;

        public Spimi(string path) : base(path)
        {
            blockCounter = 0;
            logStrWriter = new StreamWriter(logPath);
        }


        public override void Create()
        {
            var words = new List<string>(BlockSize);
            var docIds = new List<int>(BlockSize);
            logStrWriter.WriteLine("Size of collection is " + docParser.CollectionSize/1024/1024 + " MB.");
            logStrWriter.WriteLine(docParser.DocumentsCount + " documents in collection.");
            var swatch = new Stopwatch();
            swatch.Start();
            for (int docId = 0; docId < docParser.DocumentsCount; docId++)
            {
                string[] wordsInDoc = docParser.GetWordsFromEpub(docId);
                for (int i = 0; i < wordsInDoc.Length; i++)
                {
                    if (words.Count == words.Capacity)
                    {
                        AddWordsToIndex(words, docIds);
                        words.Clear();
                        docIds.Clear();
                    }
                    words.Add(wordsInDoc[i].ToLower());
                    docIds.Add(docId);
                }
                Console.WriteLine("docid " + docId);
            }
            if (words.Count > 0)
            {
                AddWordsToIndex(words, docIds);
            }
            swatch.Stop();
            logStrWriter.WriteLine("\nTime elapsed for creating blocks: " + swatch.Elapsed);

            swatch.Reset();
            swatch.Start();
            MergeBlocks();
            streamWriter.Flush();
            streamWriter.Close();
            swatch.Stop();
            logStrWriter.WriteLine("Time elapsed for merging blocks: " + swatch.Elapsed);
            logStrWriter.Flush();
            logStrWriter.Close();
        }

        private void AddWordsToIndex(List<string> words, List<int> docIds)
        {
            for (int i = 0; i < words.Count; i++)
            {
                string word = words[i];
                if (!index.ContainsKey(word))
                {
                    index.Add(word, new List<int>());
                }
                if (!index[word].Contains(docIds[i]))
                {
                    index[word].Add(docIds[i]);
                }
            }
            WriteBlock();
        }

        private void WriteBlock()
        {
            FileName = String.Format(@"\Blocks\Block" + blockCounter + ".txt");
            Console.WriteLine("block " + blockCounter);
            SaveToFile();
            index.Clear();
            blockCounter++;
        }


        public void MergeBlocks()
        {
            buffers = new List<List<string>>();
            strReaders = new List<StreamReader>();
            isBlockFinished = new bool[blockCounter];
            streamWriter = new StreamWriter(Environment.CurrentDirectory + @"\InvertedIndex(SPIMI).txt");

            InitializeBuffers();
            InitializeHeap();

            for (; heap.HeapSize > 0;)
            {
                HeapElement maxElement = heap.GetMax();
                int maxElBlockIndex = maxElement.BlockNumber;
                while (heap.HeapSize > 0 && maxElement.CompareTo(heap.Max()) == 0)
                {
                    int secondElemIndex = heap.Max().BlockNumber;
                    maxElement.PostingList = maxElement.PostingList + heap.GetMax().PostingList;
                    heap.Regularize(0);
                    CheckBuffer(secondElemIndex);
                    CheckHeap(secondElemIndex);
                }

                WriteTerm(maxElement);

                CheckBuffer(maxElBlockIndex);
                CheckHeap(maxElBlockIndex);
            }
        }


        private void InitializeHeap()
        {
            heap = new BinaryHeap<HeapElement>();
            for (int i = 0; i < blockCounter; i++)
            {
                heap.Add(new HeapElement(buffers[i][0].Trim().Split(new[] {' ', '\t'})[0], buffers[i][1], i));
                buffers[i].RemoveRange(0, 2);
            }
        }

        private void InitializeBuffers()
        {
            var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + @"\Blocks");
            List<FileInfo> blocks = directoryInfo.GetFiles().ToList();
            for (int i = 0; i < blockCounter; i++)
            {
                strReaders.Add(new StreamReader(blocks[i].FullName));
                buffers.Add(new List<string>());
                if (!FillBuffer(buffers[i], strReaders[i]))
                {
                    isBlockFinished[i] = true;
                }
            }
        }

        private void CheckBuffer(int i)
        {
            if (!(buffers[i].Count > 0))
            {
                if (!FillBuffer(buffers[i], strReaders[i]))
                {
                    isBlockFinished[i] = true;
                }
            }
        }

        private void CheckHeap(int i)
        {
            if (buffers[i].Count > 0)
            {
                heap.Add(new HeapElement(buffers[i][0].Trim().Split(new[] {' ', '\t'})[0],
                    buffers[i][1], i));
                buffers[i].RemoveRange(0, 2);
            }
        }

        private void WriteTerm(HeapElement maxElement)
        {
            streamWriter.WriteLine(maxElement.Word + "      " + maxElement.PostingList.Length);
            string[] postingList = maxElement.PostingList.Trim().Split(new[] {' '});
            foreach (string s in postingList)
            {
                streamWriter.Write(s + " ");
            }
            streamWriter.WriteLine();
        }

        private bool FillBuffer(List<string> buffer, StreamReader sr)
        {
            //todo rewrite for reading bytes instead of strings
            if (sr.EndOfStream)
            {
                return false;
            }
            for (int j = 0; j < BufferSize; j++)
            {
                string s;
                if ((s = sr.ReadLine()) != null)
                    buffer.Add(s);
                else break;
            }
            return true;
        }
    }
}
using System;
using System.IO;
using System.Linq;
using eBdb.EpubReader;

namespace InformationRetrieval
{
    internal class DocumentParser
    {
        private readonly FileInfo[] collection;

        public DocumentParser(string path)
        {
            try
            {
                var dirInfo = new DirectoryInfo(path);
                collection = dirInfo.GetFiles();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            DocumentsCount = collection.Length;
            CollectionSize = collection.Sum(file => file.Length);
        }

        public int DocumentsCount { get; private set; }
        public long CollectionSize { get; private set; }


        public string[] GetWordsFromEpub(int docId)
        {
            try
            {
                var epub = new Epub(collection[docId].FullName);
                string plainText = epub.GetContentAsPlainText();
                string[] wordsInText = Parse(plainText);
                return wordsInText;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new string[0];
        }


        private static string[] Parse(string document)
        {
            const char nonBreakingSpace = '\u00A0';
            const char dots = '\x2026';
            char[] trimChars =
            {
                '.', ',', '!', '?', '(', ')', ' ', '\t', '\"', '\v', '\n', '=', '–', '\\',
                '«', '»', '—', ';', ':', '*', '/', '[', ']', '“', '„', '~', '$',
                nonBreakingSpace, dots
            };
            return document.Split(trimChars, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
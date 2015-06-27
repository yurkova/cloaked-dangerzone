using System;
using System.IO;

namespace InformationRetrieval
{
    internal class Vocabulary
    {
        public readonly DocumentParser DocParser;
        private string[] vocabulary;

        public Vocabulary(string path)
        {
            DocParser = new DocumentParser(path);
            WordsCounter = 0;
            vocabulary = new string[0];
        }

        public int WordsCounter { get; private set; }
        public int TermsCounter { get; private set; }

        public void Create()
        {
            for (int i = 0; i < DocParser.DocumentsCount; i++)
            {
                string[] wordsInDoc = DocParser.GetWordsFromEpub(i);
                WordsCounter += wordsInDoc.Length;
                AddToVocabulary(wordsInDoc);
            }
            SortWordsInVocabulary(vocabulary, 0, vocabulary.Length - 1);
            RemoveDuplicateWords();

            // TODO "In the distant future, in the galaxy far, far away..."
            // Stemming (or lemmatization) is obviously needed for the actual use.
            // It is also possible to add stop-words processing.
        }

        private void AddToVocabulary(string[] wordsInDoc)
        {
            int insertIndex = vocabulary.Length;
            Array.Resize(ref vocabulary, vocabulary.Length + wordsInDoc.Length);
            for (int i = 0; i < wordsInDoc.Length; i++)
            {
                vocabulary[insertIndex + i] = wordsInDoc[i].ToLower();
            }
        }

        private void SortWordsInVocabulary(string[] array, int start, int end)
        {
            var rand = new Random();
            string mid = array[rand.Next(start, end)];
            int first = start;
            int last = end;
            do
            {
                while (String.Compare(vocabulary[first], mid, StringComparison.CurrentCulture) < 0)
                    first++;
                while (String.Compare(vocabulary[last], mid, StringComparison.CurrentCulture) > 0)
                    last--;
                if (first <= last)
                {
                    string tempWord = array[first];
                    array[first] = array[last];
                    array[last] = tempWord;
                    first++;
                    last--;
                }
            } while (first <= last);
            if (start < last)
                SortWordsInVocabulary(vocabulary, start, last);
            if (first < end)
                SortWordsInVocabulary(vocabulary, first, end);
        }

        private void RemoveDuplicateWords()
        {
            var tempVocabulary = new string[vocabulary.Length];
            tempVocabulary[0] = vocabulary[0];
            TermsCounter = 1;
            for (int i = 1; i < vocabulary.Length; i++)
            {
                if (String.Compare(vocabulary[i], tempVocabulary[TermsCounter - 1],
                    StringComparison.CurrentCulture) != 0)
                {
                    tempVocabulary[TermsCounter] = vocabulary[i];
                    TermsCounter++;
                }
            }
            Array.Resize(ref vocabulary, TermsCounter);
            for (int i = 0; i < TermsCounter; i++)
            {
                vocabulary[i] = tempVocabulary[i];
            }
        }

        public void SaveToFile()
        {
            using (var streamWriter = new StreamWriter(Environment.CurrentDirectory +
                                                       @"\vocabulary.txt"))
            {
                foreach (string term in vocabulary)
                {
                    streamWriter.WriteLine(term);
                }
            }
        }
    }
}
using System;

namespace InformationRetrieval
{
    public static class Program
    {
        private static string path;

        public static void Main()
        {
            Console.WriteLine("Input the path to the epub text collection:");
            path = Console.ReadLine();

            CreateVocabulary();
            CreateInvertedIndex();
            CreateIncidenceMatrix();
            RunBooleanSearch();
            CreateBiwordIndex();
            CreatePositionalIndex();

            // SPIMI (for large text collections, index is stored on disk)
            CreateLargeIndex();

            Compression.CompressVocabulary(Environment.CurrentDirectory + @"\InvIndex.txt").Save();
        }

        private static void CreateVocabulary()
        {
            Console.WriteLine("Creating vocabulary...");
            var vocabulary = new Vocabulary(path);
            vocabulary.Create();
            vocabulary.SaveToFile();
            Console.WriteLine("Vocabulary has been created.\n");
            Console.WriteLine("Size of collection is {0} kB.", vocabulary.DocParser.CollectionSize/1024);
            Console.WriteLine("Number of words in collection: {0}", vocabulary.WordsCounter);
            Console.WriteLine("Number of terms in vocabulary: {0}\n", vocabulary.TermsCounter);
        }

        private static void CreateInvertedIndex()
        {
            var invertedIndex = new InvertedIndex(path);
            Console.WriteLine("Creating inverted index...");
            invertedIndex.Create();
            invertedIndex.SaveToFile();
            Console.WriteLine("Inverted index has been created.\n");
        }

        private static void CreateIncidenceMatrix()
        {
            var incidenceMatrix = new IncidenceMatrix(path);
            Console.WriteLine("Creating incidence matrix...");
            incidenceMatrix.Create();
            incidenceMatrix.SaveToFile();
            Console.WriteLine("Incidence matrix has been created.\n");
        }

        private static void RunBooleanSearch()
        {
            string query;
            do
            {
                Console.WriteLine("Input boolean query (q for exit):");
                query = Console.ReadLine();
                BooleanSearch.Run(query);
            } while (query != "q");
        }

        private static void CreateBiwordIndex()
        {
            var biwordIndex = new BiwordIndex(path);
            Console.WriteLine("Creating biword index...");
            biwordIndex.Create();
            biwordIndex.SaveToFile();
            Console.WriteLine("Biword index has been created.\n");
        }

        private static void CreatePositionalIndex()
        {
            var positionalIndex = new PositionalIndex(path);
            Console.WriteLine("Creating positional index...");
            positionalIndex.Create();
            positionalIndex.SaveToFile();
            Console.WriteLine("Positional index has been created.\n");
        }

        private static void CreateLargeIndex()
        {
            var invertedIndex = new Spimi(path);
            invertedIndex.Create();
        }
    }
}
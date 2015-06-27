using System;

namespace InformationRetrieval
{
    internal struct HeapElement : IComparable<HeapElement>
    {
        public HeapElement(string word, string postingList, int blockNumber) : this()
        {
            Word = word;
            PostingList = postingList;
            BlockNumber = blockNumber;
        }

        public string Word { get; private set; }
        public string PostingList { get; set; }
        public int BlockNumber { get; private set; }

        public int CompareTo(HeapElement other)
        {
            return String.Compare(other.Word, Word, StringComparison.CurrentCulture);
        }
    }
}
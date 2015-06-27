using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InformationRetrieval
{
    public static class BooleanSearch
    {
        private static List<int> result;
        private static readonly SortedDictionary<string, List<int>> InvertedIndex;
        private static readonly int MaxDocId;

        static BooleanSearch()
        {
            var streamReader = new StreamReader(Environment.CurrentDirectory + @"\InvIndex.txt");
            InvertedIndex = new SortedDictionary<string, List<int>>();
            while (!streamReader.EndOfStream)
            {
                string t = streamReader.ReadLine();
                if (t != null)
                {
                    t = t.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)[0];
                    string line = streamReader.ReadLine();
                    if (line != null)
                    {
                        string[] docIds = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        List<int> postingList = docIds.Select(Int32.Parse).ToList();
                        InvertedIndex.Add(t, postingList);
                    }
                }
            }
            MaxDocId = InvertedIndex.Values.Select(x => x.Max()).Max();
        }

        public static void Run(string query)
        {
            result = new List<int>();
            var tokens = new List<string>(query.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries));
            result = EvaluateExpression(tokens);
            PrintResult();
        }

        private static List<int> EvaluateExpression(List<string> tokensList)
        {
            int tokenIndex = 0;
            try
            {
                if (tokensList.Count == 1)
                {
                    return GetPostingList(GetToken(tokensList, tokenIndex));
                }
                if (tokensList.Count == 2)
                {
                    if (!IsNegation(GetToken(tokensList, tokenIndex)))
                    {
                        throw new Exception();
                    }
                    return Invert(GetPostingList(GetToken(tokensList, ++tokenIndex)));
                }
                bool isLeftOperandNegated = false;
                bool isRightOperandNegated = false;
                List<int> leftOperand;
                List<int> rightOperand;
                string firstArgument = GetToken(tokensList, tokenIndex);
                if (IsNegation(firstArgument))
                {
                    isLeftOperandNegated = true;
                    firstArgument = GetToken(tokensList, ++tokenIndex);
                }

                if (firstArgument == "(")
                {
                    int closingBraceIndex = tokensList.IndexOf(")", tokenIndex);
                    if (closingBraceIndex < tokenIndex)
                    {
                        throw new Exception();
                    }
                    leftOperand = EvaluateSubExpression(tokensList, tokenIndex + 1, closingBraceIndex - 1);
                    tokenIndex = closingBraceIndex;
                }
                else
                {
                    if (IsWord(firstArgument))
                    {
                        leftOperand = GetPostingList(firstArgument);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                string operation = GetToken(tokensList, ++tokenIndex);
                if (!IsOperator(operation))
                {
                    throw new Exception();
                }

                string secondArgument = GetToken(tokensList, ++tokenIndex);
                if (IsNegation(secondArgument))
                {
                    isRightOperandNegated = true;
                    secondArgument = GetToken(tokensList, ++tokenIndex);
                }
                if (secondArgument == "(")
                {
                    int closingBraceIndex = tokensList.IndexOf(")", tokenIndex);
                    if (closingBraceIndex < tokenIndex)
                    {
                        throw new Exception();
                    }
                    rightOperand = EvaluateSubExpression(tokensList, tokenIndex + 1, closingBraceIndex - 1);
                }
                else if (!IsWord(secondArgument))
                {
                    throw new Exception();
                }
                else
                {
                    rightOperand = GetPostingList(secondArgument);
                }

                if (operation.Equals("and"))
                {
                    return EvaluateConjunctiveQuery(leftOperand, rightOperand,
                        isLeftOperandNegated, isRightOperandNegated);
                }
                if (operation.Equals("or"))
                {
                    return EvaluateDisjunctiveQuery(leftOperand, rightOperand,
                        isLeftOperandNegated, isRightOperandNegated);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid query, please try again.");
            }
            return new List<int>();
        }


        private static List<int> EvaluateSubExpression(List<string> tokens, int startIndex, int endIndex)
        {
            List<string> subExpression = tokens.GetRange(startIndex, endIndex - startIndex + 1);
            List<int> t = EvaluateExpression(subExpression);
            return t;
        }

        private static List<int> EvaluateConjunctiveQuery(List<int> leftOperand, List<int> rightOperand,
            bool isLeftOperandNegated, bool isRightOperandNegated)
        {
            if (isLeftOperandNegated)
            {
                leftOperand = Invert(leftOperand);
            }
            if (isRightOperandNegated)
            {
                rightOperand = Invert(rightOperand);
            }
            return Intersect(leftOperand, rightOperand);
        }

        private static List<int> Intersect(List<int> leftOperand, List<int> rightOperand)
        {
            var answer = new List<int>();
            int i = 0;
            int j = 0;
            while (i < leftOperand.Count && j < rightOperand.Count)
            {
                if (leftOperand[i] == rightOperand[j])
                {
                    answer.Add(leftOperand[i]);
                    i++;
                    j++;
                }
                else if (leftOperand[i] < rightOperand[j])
                {
                    i++;
                }
                else
                {
                    j++;
                }
            }
            return answer;
        }

        private static List<int> EvaluateDisjunctiveQuery(List<int> leftOperand,
            List<int> rightOperand, bool isLeftOperandNegated, bool isRightOperandNegated)
        {
            if (isLeftOperandNegated && !isRightOperandNegated)
            {
                leftOperand = Invert(leftOperand);
            }
            else if (isRightOperandNegated && !isLeftOperandNegated)
            {
                rightOperand = Invert(rightOperand);
            }

            leftOperand.AddRange(rightOperand);
            leftOperand.Sort();
            leftOperand = leftOperand.Distinct().ToList();

            if (isLeftOperandNegated && isRightOperandNegated)
            {
                leftOperand = Invert(leftOperand);
            }
            return leftOperand;
        }

        private static List<int> Invert(List<int> leftOperand)
        {
            var newList = new List<int>();
            for (int i = 0; i < MaxDocId; i++)
            {
                if (!leftOperand.Contains(i))
                {
                    newList.Add(i);
                }
            }
            return newList;
        }


        private static List<int> GetPostingList(string term)
        {
            if (InvertedIndex.ContainsKey(term))
            {
                return InvertedIndex[term.ToLower()];
            }
            return new List<int>();
        }

        private static string GetToken(IEnumerable<string> tokensList, int index)
        {
            return tokensList.ElementAt(index);
        }

        private static bool IsWord(string token)
        {
            return !(IsOperator(token) || IsNegation(token));
        }

        private static bool IsOperator(string token)
        {
            return token.Equals("or") || token.Equals("and");
        }

        private static bool IsNegation(string token)
        {
            return token.Equals("not");
        }

        private static void PrintResult()
        {
            foreach (int docId in result)
            {
                Console.WriteLine(docId + " ");
            }
            Console.WriteLine();
        }
    }
}
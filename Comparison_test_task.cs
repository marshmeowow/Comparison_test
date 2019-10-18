using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;



namespace Comparison_test_task
{
    class Comparison_test_task
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    string path_source = args[0];
                    string path_target = args[1];


                    StreamReader text_source = new StreamReader(path_source);
                    StreamReader text_target = new StreamReader(path_target);

                    string string1 = text_source.ReadToEnd();
                    string string2 = text_target.ReadToEnd();


                    List<S_my_Diff> difsList = Levenshtein(string1, string2);
                    getDifs(difsList, string1, string2);

                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Incorect input");
            }

        }

        public static void getDifs(List<S_my_Diff> difsList,string X,string Y)
        { 

            List<string> resultList = new List<string>();
            foreach (S_my_Diff diff in difsList)
            {
                if (diff.OPERATION_TYPE == "match")
                {
                    resultList.Add($"{X[diff.x]}");
                }
                else if (diff.OPERATION_TYPE == "substitution")
                {
                    resultList.Add($"[{X[diff.x]}]({Y[diff.y]})");
                }
                else if (diff.OPERATION_TYPE == "deletion")
                {
                    resultList.Add($"[{X[diff.x]}]");
                }
                else if (diff.OPERATION_TYPE == "insertion")
                {
                    resultList.Add($"({Y[diff.y]})");
                }
            }
            string resultString = string.Join("", resultList);
            resultString = resultString.Replace("][", "");
            resultString = resultString.Replace(")(", "");

            WriteToFile(resultString);

        }

        static void WriteToFile(string toWrite)
        {
            string writePath = Directory.GetCurrentDirectory();
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath + "\\RESULT.txt"))
                {
                    sw.WriteLine(toWrite);
                    sw.Close();
                }

         
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        public static List<S_my_Diff> Levenshtein(string string1, string string2)
        {
            int[,] levDis = getMatrix(string1, string2);
            List<S_my_Diff> difsList;
            difsList = BackTrack(string1, string2, levDis);
            return difsList;

        }

        public static int[,] getMatrix(string string1, string string2)
        {
            if (string1 == null) throw new ArgumentNullException("string1");
            if (string2 == null) throw new ArgumentNullException("string2");
            int editCost;
            int[,] m = new int[string1.Length + 1, string2.Length + 1];

            for (int i = 0; i <= string1.Length; i++) { m[i, 0] = i; }
            for (int j = 0; j <= string2.Length; j++) { m[0, j] = j; }

            for (int i = 1; i <= string1.Length; i++)
            {
                for (int j = 1; j <= string2.Length; j++)
                {
                    editCost = (string1[i - 1] == string2[j - 1]) ? 0 : 1;

                    m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1,
                                             m[i, j - 1] + 1),
                                             m[i - 1, j - 1] + editCost);
                }
            }
            return m;
        }

        public struct S_my_Diff
        {
            
            public string OPERATION_TYPE;
            public int x;
            public int y;

            public S_my_Diff(string Operation, int x, int y)
            {              
                this.OPERATION_TYPE = Operation;
                this.x = x;
                this.y = y;
            }
        }

        public static List<S_my_Diff> BackTrack(string s1, string s2, int[,] matrix)
        {
            int i = s1.Length;
            int j = s2.Length;
            List < S_my_Diff > edits = new List<S_my_Diff>();
            while(!(i == 0 && j == 0))
            {
                int prev_cost = matrix[i, j];
                List<int> neighbors = new List<int>();
                if (i!= 0 && j!= 0)
                {
                    neighbors.Add(matrix[i - 1, j - 1]);
                }
                if (i != 0)
                {
                    neighbors.Add(matrix[i - 1, j]);
                }
                if (j != 0)
                {
                    neighbors.Add(matrix[i, j - 1]);
                }

                int minCost = neighbors.Min();

                if (minCost == prev_cost)
                {
                    i -= 1;
                    j -= 1;
                    edits.Add(new S_my_Diff("match", i, j));
                }
                else if (i != 0 && j != 0 && minCost == matrix[i-1, j-1])
                {
                    i -= 1;
                    j -= 1;
                    edits.Add(new S_my_Diff("substitution", i, j));
                }
                else if (i != 0 && minCost == matrix[i - 1, j])
                {
                    i -= 1;                    
                    edits.Add(new S_my_Diff("deletion", i, j));

                }
                else if (j != 0 && minCost == matrix[i, j - 1])
                {

                    j -= 1;
                    edits.Add(new S_my_Diff("insertion", i, j));

                }
            }
            edits.Reverse();
            return edits;
        }
    }
}

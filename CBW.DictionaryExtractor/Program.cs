using CBW.Core;
using CBW.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace CBW.DictionaryExtractor
{
    /// <summary>
    ///  Extractor will create two dictionaries.
    ///  1. All the word list (ID, word)
    ///  2. Relevance table (ID, ID, Relevance)
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            WordDictionary dict = ExtractWords("Data\\EnglishDictionary.txt");
            //RelevanceTable relTable = AnalyzeRelevance(dict);
            //BinarySerializer.Serialize(dict, "Data\\dict.bin");
            //BinarySerializer.Serialize(relTable, "Data\\rel.bin");
            Upload(dict);
        }

        public static WordDictionary ExtractWords(string path)
        {
            WordDictionary dict = new WordDictionary();
            int lineCount = 0;
            StringBuilder defBuilder = new StringBuilder();
            DictionaryEntry currentEntry = null;
            int id = 0;
            foreach (string line in File.ReadLines(path))
            {
                if (currentEntry == null)
                {
                    if (String.IsNullOrEmpty(line))
                    {
                        goto next;
                    }
                    foreach (char c in line)
                    {
                        if (c == ' ' || c == '.')
                        {
                            goto next;
                        }
                        if (!Char.IsUpper(c))
                        {
                            goto next;
                        }
                    }
                    defBuilder.Clear();
                    currentEntry = new DictionaryEntry();
                    currentEntry.Word = line.ToLowerInvariant();
                    lineCount = 0;
                }
                else
                {
                    // Extract word type information
                    if (lineCount == 1)
                    {
                        string[] parts = line.Split(',', '.');
                        currentEntry.Type = parts[1].Trim();
                    }
                    else
                    {
                        if (line.StartsWith("Defn: "))
                        {
                            defBuilder.Append(line.Substring(6));
                        }
                        else
                        {
                            if (line.Trim() == String.Empty && defBuilder.Length != 0)
                            {
                                currentEntry.Definition = defBuilder.ToString();
                                currentEntry.Id = id;
                                id++;
                                dict[currentEntry.Word] = currentEntry;
                                Console.WriteLine(currentEntry.Word);
                                currentEntry = null;
                            }
                            else if (line.Trim() != String.Empty && defBuilder.Length != 0)
                            {
                                defBuilder.Append(line);
                            }
                        }
                    }
                }
            next:
                id = id;
            }
            return dict;
        }

        public static RelevanceTable AnalyzeRelevance(WordDictionary dict)
        {
            RelevanceTable table = new RelevanceTable();
            foreach (string wordA in dict.Keys)
            {
                DictionaryEntry entryA = dict[wordA];
                foreach (string wordB in StringParser.Parse(entryA.Definition))
                {
                    DictionaryEntry entryB;
                    if (dict.TryGet(wordB, out entryB))
                    {
                        Console.WriteLine(wordA + ", " + wordB);
                        table.Add(entryA.Id, entryB.Id);
                    }
                }
            }
            return table;
        }

        public static void Upload(WordDictionary dict)
        {
            DictionaryProvider provider = new DictionaryProvider(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString);
            provider.UploadDictionary(dict);
            //provider.UploadRelevanceTable(table);
        }
    }
}

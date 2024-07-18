using System.Diagnostics;
using HyperVectorDB;
using RagSharp;

namespace RagsharpExample
{
    internal class Program
    {
        public static HyperVectorDB.HyperVectorDB? DB;
        public static ILLMProvider llm;

        static bool skippingBlock = false;
        private static string? CustomPreprocessor(string line, string? path, int? lineNumber)
        {
            if (string.IsNullOrWhiteSpace(line)) { return null; }

            if (path != null && path.ToUpperInvariant().EndsWith(".MD"))
            {
                if (line.Contains("---"))// Skip YAML frontmatter
                {
                    skippingBlock = false;
                    return null;
                }
                else if (line.Contains("```"))// Skip code blocks
                {
                    skippingBlock = !skippingBlock;
                    return null;
                }
                else
                {
                    if (line.EndsWith("aliases: ") ||
                        line.Contains("date created:") ||
                        line.Contains("date modified:") ||
                        (line.EndsWith(":") && !line.StartsWith("#"))
                    ) { return null; }
                }

                if (line.Contains("%%")) { return null; }//Skip annotation lines

                if (line.Trim().StartsWith("[[") && line.Trim().EndsWith("]]")) { return null; }//Skip index links

                if (skippingBlock) { return null; }

            }

            return line.Trim();
        }

        static void Main()
        {
            DiscordProvider discord = new DiscordProvider();

            discord.Setup();

            while(true)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Done, press enter to exit");
            Console.ReadLine();

        }
    }
}
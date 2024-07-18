using System;
using System.Collections.Generic;
using HyperVectorDB;
namespace RagSharp;


public class Conversation
{
    public HyperVectorDB.HyperVectorDB VectorDB;

    public List<InterfaceMessage> Messages = new();


    public string ID { get; set; }


    public Conversation(string? id = null)
    {
        if (id == null)
        {
            ID = $"{Random.Shared.Next(9999):D4}";
        }
        else
        {
            ID = id!;
        }
        VectorDB = new HyperVectorDB.HyperVectorDB(new HyperVectorDB.Embedder.LmStudio(), $"Conversation_{ID}", 32);
    }

    public void Add(InterfaceMessage message)
    {
        Messages.Add(message);
        VectorDB.IndexDocument(message.ToString());
    }

    public string GetContext(int maxTokens = 1024, string? prompt = null)
    {
        string result = "";

        if (prompt != null)
        {
            result += "THESE PAST MESSAGES MIGHT BE RELEVANT\n";
            var searchResults = VectorDB.QueryCosineSimilarity(prompt, 100);
            for (var i = 0; i < searchResults.Documents.Count; i++)
            {
                if (CountTokens(result) > maxTokens / 2) { break; }

                result += $"\n--------\n{searchResults.Documents[i].DocumentString}";
            }
        }

        return result;
    }

    private static int CountTokens(string text)
    {
        string cleanText = string.Join(' ', text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        string[] tokens = cleanText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return tokens.Length;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
namespace RagSharp;


public class LMStudio : ILLMProvider
{
    public string SystemPrompt {get; set;} = @"Answer to the best of your ability.";
    public string Model {get; set;} = @"MaziyarPanahi/Meta-Llama-3-70B-Instruct-GGUF";

    private string[] context = {""};

    public string Prompt(string prompt)
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:1234/v1/chat/completions");
        request.Content = new StringContent(BuildRequest(prompt, SystemPrompt));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = client.Send(request);
        response.EnsureSuccessStatusCode();
        var responseBody = response.Content.ReadAsStringAsync();
        responseBody.Wait();
        string responseText = responseBody.Result;

        //Console.WriteLine($"[RESPONSE] {TranslateResponse(responseText)}");

        return TranslateResponse(responseText);
    }

    public void SetContext(string[] context)
    {
        this.context = context;
    }

    public void SetSystemPrompt(string prompt)
    {
        SystemPrompt = prompt;
    }

    string BuildRequest(string prompt, string systemPrompt = "")
    {
        List<Message> messages = new List<Message>();
        string ContrextString = "";
        foreach(string s in context)
        {
            ContrextString += s + "\n";
        }
        ContrextString += "========\n";

        messages.Add(new Message()
        {
            role = "system",
            content = systemPrompt
        });

        messages.Add(new Message()
        {
            role = "user",
            content = ContrextString + " " + prompt
        });

        LlmRequest request = new()
        {
            model = Model,
            temperature = 0.7,
            max_tokens = -1,
            stream = false,
            messages = messages
        };

        string json = JsonSerializer.Serialize(request);
        Console.WriteLine(json);
        return json;
    }

    string TranslateResponse(string json)
    {
        LlmResponse response = JsonSerializer.Deserialize<LlmResponse>(json)!;

        string body = response.choices.FirstOrDefault()?.message.content!;

        return body;
    }
}

class Choice
{
    public int index { get; set; }
    public Message message { get; set; }
    public string finish_reason { get; set; }
}

class Message
{
    public string role { get; set; }
    public string content { get; set; }
}

class Usage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

class LlmRequest
{
    public string model { get; set; }
    public List<Message> messages { get; set; }
    public double temperature { get; set; }
    public int max_tokens { get; set; }
    public bool stream { get; set; }
}

class LlmResponse
{
    public string id { get; set; }
    //public string @object { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public List<Choice> choices { get; set; }
    public Usage usage { get; set; }
}
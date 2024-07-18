using Discord;
using Discord.WebSocket;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Discord.Interactions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace RagSharp;


public class DiscordProvider : IInterfaceProvider
{
    private DiscordSocketClient _client = new();
    private List<InterfaceMessage> messages = new();

    public InterfaceMessage[]? GetMessages()
    {
        throw new System.NotImplementedException();
    }

    public void Prompt(string prompt)
    {
        throw new System.NotImplementedException();
    }

    public void Setup()
    {
        // Config used by DiscordSocketClient
        // Define intents for the client
        // Note that GatewayIntents.MessageContent is a privileged intent, and requires extra setup in the developer portal.
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        // It is recommended to Dispose of a client when you are finished
        // using it, at the end of your app's lifetime.
        _client = new DiscordSocketClient(config);

        // Subscribing to client events, so that we may receive them whenever they're invoked.
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;


        DiscordLogin();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }

    private async void DiscordLogin()
    {
        // Tokens should be considered secret data, and never hard-coded.
        await _client.LoginAsync(TokenType.Bot, "TOKEN");
        // Different approaches to making your token a secret is by putting them in local .json, .yaml, .xml or .txt files, then reading them on startup.

        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    // The Ready event indicates that the client has opened a
    // connection and it is now safe to access the cache.
    private Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} is connected!");

        return Task.CompletedTask;
    }

    // This is not the recommended way to write a bot - consider
    // reading over the Commands Framework sample.
    private async Task MessageReceivedAsync(SocketMessage message)
    {
        // The bot should never respond to itself.
        if (message.Author.Id == _client.CurrentUser.Id)
            return;

        Console.WriteLine(message.Content);

        InterfaceMessage im = new();
        im.Content = message.Content;
        im.Sender = message.Author.Username;
        im.Recipient = null;
        Console.WriteLine(im.ToString());
        messages.Add(im);


        if (message.Content == "!ping")
        {
            // Create a new ComponentBuilder, in which dropdowns & buttons can be created.
            var cb = new ComponentBuilder()
                .WithButton("Click me!", "unique-id", ButtonStyle.Primary);

            // Send a message with content 'pong', including a button.
            // This button needs to be build by calling .Build() before being passed into the call.
            await message.Channel.SendMessageAsync(":pleading_face:", components: cb.Build());
        }
    }
}
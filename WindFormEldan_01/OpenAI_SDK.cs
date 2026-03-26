using DotNetEnv;
using OpenAI.Chat;
using System;
using System.Collections.Generic;

public class OpenAI_SDK
{
    private readonly ChatClient GPTModel;
    private readonly List<ChatMessage> history = new();

    public OpenAI_SDK(string model)
    {
        Env.TraversePath().Load();
        var ChatGPTApiKey = Environment.GetEnvironmentVariable("ChatGPTApiKey");
        if (string.IsNullOrEmpty(ChatGPTApiKey))
            throw new Exception("ChatGPTApiKey environment variable is not set or .env file not loaded.");
        GPTModel = new ChatClient(model, ChatGPTApiKey);
    }

    public async Task<string> Call(string userMessage)
    {
        history.Add(new UserChatMessage(userMessage));
        var completion = await GPTModel.CompleteChatAsync(history);
        var text = completion.Value.Content[0].Text;
        history.Add(new AssistantChatMessage(text));
        return text;
    }
}

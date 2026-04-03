using DotNetEnv;
using Google.GenAI;
using System;
using System.Threading.Tasks;

namespace Eldan_Exercise_02
{
    public class Gemini_SDK
    {
        public static async Task<string> Call(string userMessage, string selectedModel = "gemini-2.5-flash")
        {
            Env.TraversePath().Load();
            var geminiKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            // Map display name to API model name
            string model = selectedModel == "Gemini 2.5 Flash-Lite" ? "gemini-2.5-flash-lite" : "gemini-2.5-flash";

            var geminiModel = new Client(apiKey: geminiKey);

            var response = await geminiModel.Models.GenerateContentAsync(
                model: model,
                contents: userMessage
            );

            var text = response.Candidates[0].Content.Parts[0].Text;
            return text;
        }
    }
}

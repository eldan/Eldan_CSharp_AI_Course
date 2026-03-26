using DotNetEnv;
using Google.GenAI;


namespace Lesson_1_intro
{
    public class Gemini_SDK
    {
        public static async Task<string> Call(string userMessage)
        {
            Env.TraversePath().Load();
            var geminiKey = Environment.GetEnvironmentVariable("GeminiAPIKey");
            string model = "gemini-2.5-flash";

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

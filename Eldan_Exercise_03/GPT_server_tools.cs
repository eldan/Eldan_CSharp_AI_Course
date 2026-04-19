#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;

public class GPT_server_tools
{
    public async Task Run()
    {
        Env.TraversePath().Load();

        var systemPrompt = """
                Respond in the language of the question. if the subject is army, navy, air force, space force, marines, coast guard, or any other military branch, use the web search tool to find the most recent information. if the subject is not military, respond based on your training data.
                You may use the OpenAI server web search tool when needed.
                Use it especially for current events, recent news, and changing information.
                """;

        var webSearchTool = ResponseTool.CreateWebSearchTool();

        var openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: new List<ResponseTool> { webSearchTool });

        while (true)
        {
            Console.Write("Ask: ");
            var question = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(question))
            {
                return;
            }

            var response = await openai.Call(question);

            Console.WriteLine();
            Console.WriteLine(response.GetOutputText());
            Console.WriteLine();
        }
    }
}

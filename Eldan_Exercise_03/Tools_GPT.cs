#pragma warning disable OPENAI001
using OpenAI.Responses;
using System.Text.Json;
using Eldan_Exercise_03.AI_Tools;

public class Tools_GPT
{
  public async Task Run()
  {
    var systemPrompt = """
                You may call tools when needed.
                Use GetDate to get today's date.
                Use GetTime to get the current time.
                Use TavilySearch when you need to search the web.
                Use GetSchema to understand the SQL database structure.
                Use RetrieveTable to run SELECT queries on the SQL database.
                Use ExecuteNonQuery only when the user explicitly asks to change data in the SQL database.
                """;

    var dateTimeTools = new DateTimeTools();
    var armyPhrasesTools = ArmyPhrasesTool.Instance;


    var noParamsSchema = BinaryData.FromString("""
            {
                "type":"object", 
                "properties":{}, 
                "required":[], 
                "additionalProperties":false 
            }
            """);

    var getDateTool = ResponseTool.CreateFunctionTool(
        functionName: "GetDate",
        functionParameters: noParamsSchema,
        strictModeEnabled: true,
        functionDescription: "Get today's date"
        );

    var getTimeTool = ResponseTool.CreateFunctionTool(
        functionName: "GetTime",
        functionParameters: noParamsSchema,
        strictModeEnabled: true,
        functionDescription: "Get the current time");


    var getArmyPhrasesTool = ResponseTool.CreateFunctionTool(
       functionName: "GetArmyPhrases",
       functionParameters: noParamsSchema,
       strictModeEnabled: true,
       functionDescription: "Get funny army phrases");


    var openai = new OpenAI_Tools(
            model: "gpt-5.2",
            systemPrompt: systemPrompt,
            tools: new List<ResponseTool>
            {
                getDateTool,
                getTimeTool,
                getArmyPhrasesTool
            });

    while (true)
    {
      Console.Write("Ask your question: ");
      var message = Console.ReadLine();
      if (string.IsNullOrWhiteSpace(message)) return;

      const int maxSteps = 5;

      var response = await openai.Call(message);
      bool finalResponsePrinted = false;

      for (int step = 0; step < maxSteps; step++)
      {
        int count = 0;
        var toolOutputs = new List<ResponseItem>();

        foreach (var item in response.OutputItems)
        {
          if (item is FunctionCallResponseItem)
          {
            count++;
            var call = (FunctionCallResponseItem)item;
            string toolResult;

            try
            {
              if (call.FunctionName == "GetDate")
              {
                toolResult = dateTimeTools.GetDate();
              }
              else if (call.FunctionName == "GetTime")
              {
                toolResult = dateTimeTools.GetTime();
              }

              else if (call.FunctionName == "GetArmyPhrases")
              {
                toolResult = armyPhrasesTools.GetRandomPhrase();
              }
              else
              {
                toolResult = "Unknown tool: " + call.FunctionName;
              }
            }
            catch (Exception ex)
            {
              toolResult = "Tool error: " + ex.Message;
            }

            toolOutputs.Add(ResponseItem.CreateFunctionCallOutputItem(call.CallId, toolResult));
          }
        }

        if (count == 0)
        {
          Console.WriteLine(response.GetOutputText());
          finalResponsePrinted = true;
          break;
        }

        if (step == maxSteps - 1)
        {
          toolOutputs.Add(ResponseItem.CreateUserMessageItem(
              "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer using the information you already have."));

          response = await openai.Call(toolOutputs);
          Console.WriteLine(response.GetOutputText());
          finalResponsePrinted = true;
          break;
        }

        response = await openai.Call(toolOutputs);
      }

      if (!finalResponsePrinted)
      {
        Console.WriteLine("Max iterations reached.");
      }
    }
  }
}

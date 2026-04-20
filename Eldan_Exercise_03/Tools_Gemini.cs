using Google.GenAI;
using Google.GenAI.Types;
using System.Text;
using Eldan_Exercise_03.AI_Tools;

public class Tools_Gemini
{
  private readonly Client GeminiModel;
  private readonly List<Content> history = new();
  private readonly GenerateContentConfig config;
  private readonly DateTimeTools dateTimeTools;
  private readonly ArmyPhrasesTool armyPhrasesTools;
  private readonly Pupils pupils;
  private const int MAX_TOOL_STEPS = 5;

  public Tools_Gemini(string? systemPrompt = null)
  {
    GeminiModel = new Client();
    dateTimeTools = new DateTimeTools();
    armyPhrasesTools = ArmyPhrasesTool.Instance;
    pupils = Pupils.Instance;

    var noParamsSchema = new Schema { Type = Google.GenAI.Types.Type.Object };

    var twoStringsSchema = new Schema
    {
      Type = Google.GenAI.Types.Type.Object,
      Properties = new Dictionary<string, Schema>
      {
        ["nameA"] = new Schema { Type = Google.GenAI.Types.Type.String },
        ["nameB"] = new Schema { Type = Google.GenAI.Types.Type.String }
      },
      Required = new List<string> { "nameA", "nameB" }
    };

    var getDateTool = new FunctionDeclaration
    {
      Name = "GetDate",
      Description = "Get today's date",
      Parameters = noParamsSchema
    };

    var getTimeTool = new FunctionDeclaration
    {
      Name = "GetTime",
      Description = "Get the current time",
      Parameters = noParamsSchema
    };

    var getArmyPhrasesTool = new FunctionDeclaration
    {
      Name = "GetArmyPhrases",
      Description = "Get funny army phrases",
      Parameters = noParamsSchema
    };

    var canSitTogetherTool = new FunctionDeclaration
    {
      Name = "CanSitTogether",
      Description = "Check if two pupils can sit together based on their likes and dislikes, supply the reason they can or can't sit together",
      Parameters = twoStringsSchema
    };

    var tool = new Tool
    {
      FunctionDeclarations = [getDateTool, getTimeTool, getArmyPhrasesTool, canSitTogetherTool]
    };

    config = new GenerateContentConfig
    {
      Temperature = 0.7f,
      TopP = 0.95f,
      TopK = 40,
      CandidateCount = 1,
      Tools = [tool]
    };

    var defaultPrompt = """
                You may call tools when needed.
                Use GetDate to get today's date.
                Use GetTime to get the current time.
                Use GetArmyPhrases to get funny army phrases.
                Use CanSitTogether to check if two pupils can sit together.
                """;

    config.SystemInstruction = new Content { Parts = [new Part { Text = systemPrompt ?? defaultPrompt }] };
  }

  public async Task<string> Call(string userMessage)
  {
    history.Add(new Content { Role = "user", Parts = [new Part { Text = userMessage }] });

    var response = await ProcessToolCalls();
    return response.Candidates[0].Content.Parts.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Text))?.Text ?? string.Empty;
  }

  public async IAsyncEnumerable<string> CallStream(string userMessage)
  {
    var result = await Call(userMessage);
    yield return result;
  }

  private async Task<GenerateContentResponse> ProcessToolCalls()
  {
    GenerateContentResponse response = await GeminiModel.Models.GenerateContentAsync(
        model: "gemini-2.5-flash",
        contents: history,
        config: config
    );

    for (int step = 0; step < MAX_TOOL_STEPS; step++)
    {
        var parts = response.Candidates[0].Content.Parts;
        int toolCallCount = 0;
        var toolOutputContent = new Content { Role = "user", Parts = new List<Part>() };

        foreach (var part in parts)
        {
            if (part.FunctionCall is not null)
            {
                toolCallCount++;
                var call = part.FunctionCall;
                Dictionary<string, object> args =
                    call.Args?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value ?? string.Empty
                    ) ?? new Dictionary<string, object>();

                string toolResult = ExecuteTool(call.Name, args);

                toolOutputContent.Parts.Add(new Part
                {
                    FunctionResponse = new FunctionResponse
                    {
                        Name = call.Name,
                        Response = new Dictionary<string, object> { { "result", toolResult } }
                    }
                });
            }
        }

        if (toolCallCount == 0)
        {
            history.Add(response.Candidates[0].Content);
            break;
        }

        history.Add(response.Candidates[0].Content);
        history.Add(toolOutputContent);

        if (step == MAX_TOOL_STEPS - 1)
        {
            toolOutputContent.Parts.Add(new Part
            {
                Text = "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer."
            });
        }

        response = await GeminiModel.Models.GenerateContentAsync(
            model: "gemini-2.5-flash",
            contents: history,
            config: config
        );
    }

    return response;
  }

  private string ExecuteTool(string functionName, Dictionary<string, object> args)
  {
    try
    {
      return functionName switch
      {
        "GetDate" => dateTimeTools.GetDate(),
        "GetTime" => dateTimeTools.GetTime(),
        "GetArmyPhrases" => armyPhrasesTools.GetRandomPhrase(),
        "CanSitTogether" => ExecuteCanSitTogether(args),
        _ => "Unknown tool: " + functionName
      };
    }
    catch (Exception ex)
    {
      return "Tool error: " + ex.Message;
    }
  }

  private string ExecuteCanSitTogether(Dictionary<string, object> args)
  {
    var nameA = args["nameA"]?.ToString() ?? string.Empty;
    var nameB = args["nameB"]?.ToString() ?? string.Empty;

    var canSit = pupils.CanSitTogether(nameA, nameB);
    return canSit ? $"Yes, {nameA} and {nameB} can sit together." : $"No, {nameA} and {nameB} cannot sit together.";
  }
}

#pragma warning disable OPENAI001
using DotNetEnv;
using OpenAI.Responses;
using Eldan_Exercise_03.AI_Tools;

public class Tools_GPT
{
  private readonly ResponsesClient GPTModel;
  private readonly List<ResponseItem> history = new();
  private readonly CreateResponseOptions config;
  private readonly DateTimeTools dateTimeTools;
  private readonly ArmyPhrasesTool armyPhrasesTools;
  private readonly Pupils pupils;
  private const int MAX_TOOL_STEPS = 5;

  public Tools_GPT(string? systemPrompt = null)
  {

    Env.TraversePath().Load();
    var OpenAIKey = Environment.GetEnvironmentVariable("ChatGPTApiKey");
    GPTModel = new ResponsesClient(OpenAIKey);

    dateTimeTools = new DateTimeTools();
    armyPhrasesTools = ArmyPhrasesTool.Instance;
    pupils = Pupils.Instance;

    var noParamsSchema = BinaryData.FromString(""" 
            {
                "type":"object", 
                "properties":{}, 
                "required":[], 
                "additionalProperties":false 
            }
            """);

    var twoStringsSchema = BinaryData.FromString("""
            {
                "type":"object",
                "properties":{
                    "nameA":{"type":"string"},
                    "nameB":{"type":"string"}
                },
                "required":["nameA","nameB"],
                "additionalProperties":false
            }
            """);

    var getDateTool = ResponseTool.CreateFunctionTool(
        functionName: "GetDate",
        functionParameters: noParamsSchema,
        strictModeEnabled: true,
        functionDescription: "Get today's date");

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

    var canSitTogetherTool = ResponseTool.CreateFunctionTool(
        functionName: "CanSitTogether",
        functionParameters: twoStringsSchema,
        strictModeEnabled: true,
          functionDescription: "Check if two pupils can sit together based on their likes and dislikes");

    var canSitTogetherWithReasonTool = ResponseTool.CreateFunctionTool(
        functionName: "CanSitTogetherWithReason",
        functionParameters: twoStringsSchema,
        strictModeEnabled: true,
        functionDescription: "Check if two pupils can sit together and get the detailed reason including their likes");

    config = new CreateResponseOptions
    {
      Model = "gpt-5.2",
      TruncationMode = ResponseTruncationMode.Auto,
      EndUserId = "uid298jkasdf8Ad",
      Temperature = 0.7f,
      TopP = 0.95f
    };

    config.Tools.Add(getDateTool);
    config.Tools.Add(getTimeTool);
    config.Tools.Add(getArmyPhrasesTool);
    config.Tools.Add(canSitTogetherTool);
    config.Tools.Add(canSitTogetherWithReasonTool);

    var defaultPrompt = """
                You may call tools when needed.
                Use GetDate to get today's date.
                Use GetTime to get the current time.
                Use GetArmyPhrases to get funny army phrases.
                Use CanSitTogether to check if two pupils can sit together.
                Use CanSitTogetherWithReason to get detailed reasons why pupils can or cannot sit together.
                """;

    config.Instructions = systemPrompt ?? defaultPrompt;




  }

  public async Task<string> Call(string userMessage)
  {
    history.Add(ResponseItem.CreateUserMessageItem(userMessage));

    config.InputItems.Clear();
    foreach (var item in history)
    {
      config.InputItems.Add(item);
    }

    var response = await ProcessToolCalls();
    return response.GetOutputText();
  }

  public async IAsyncEnumerable<string> CallStream(string userMessage)
  {
    // Temporarily use Call() and yield the result
    var result = await Call(userMessage);
    yield return result;
  }

  private async Task<ResponseResult> ProcessToolCalls()
  {
    ResponseResult response = await GPTModel.CreateResponseAsync(config);

    for (int step = 0; step < MAX_TOOL_STEPS; step++)
    {
      int toolCallCount = 0;
      var toolOutputs = new List<ResponseItem>();

      foreach (var item in response.OutputItems)
      {
        if (item is FunctionCallResponseItem)
        {
          toolCallCount++;
          var call = (FunctionCallResponseItem)item;
          string toolResult = ExecuteTool(call.FunctionName, call.FunctionArguments);
          
          // Add the function call to history
          history.Add(call);
          
          // Create and add the tool output
          var toolOutput = ResponseItem.CreateFunctionCallOutputItem(call.CallId, toolResult);
          toolOutputs.Add(toolOutput);
          history.Add(toolOutput);
        }
      }

      if (toolCallCount == 0)
      {
        // No tool calls, add response items to history and break
        foreach (var item in response.OutputItems)
        {
          history.Add(item);
        }
        break;
      }

      if (step == MAX_TOOL_STEPS - 1)
      {
        history.Add(ResponseItem.CreateUserMessageItem(
            "Max tool steps reached. No more tool calls are allowed. Reply normally with your best final answer."));
      }

      // Rebuild InputItems with complete history
      config.InputItems.Clear();
      foreach (var item in history)
      {
        config.InputItems.Add(item);
      }

      response = await GPTModel.CreateResponseAsync(config);
    }

    return response;
  }

  private string ExecuteTool(string functionName, BinaryData arguments)
  {
    try
    {
      return functionName switch
      {
        "GetDate" => dateTimeTools.GetDate(),
        "GetTime" => dateTimeTools.GetTime(),
        "GetArmyPhrases" => armyPhrasesTools.GetRandomPhrase(),
        "CanSitTogether" => ExecuteCanSitTogether(arguments),
        "CanSitTogetherWithReason" => ExecuteCanSitTogetherWithReason(arguments),
        _ => "Unknown tool: " + functionName
      };
    }
    catch (Exception ex)
    {
      return "Tool error: " + ex.Message;
    }
  }

  private string ExecuteCanSitTogether(BinaryData arguments)
  {
    var json = System.Text.Json.JsonDocument.Parse(arguments.ToString());
    var root = json.RootElement;
    
    var nameA = root.GetProperty("nameA").GetString();
    var nameB = root.GetProperty("nameB").GetString();
    
    var canSit = pupils.CanSitTogether(nameA, nameB);
    return canSit ? $"Yes, {nameA} and {nameB} can sit together." : $"No, {nameA} and {nameB} cannot sit together.";
  }

  private string ExecuteCanSitTogetherWithReason(BinaryData arguments)
  {
    var json = System.Text.Json.JsonDocument.Parse(arguments.ToString());
    var root = json.RootElement;
    
    var nameA = root.GetProperty("nameA").GetString();
    var nameB = root.GetProperty("nameB").GetString();
    
    return pupils.CanSitTogetherWithReason(nameA, nameB);
  }
}

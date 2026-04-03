using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Eldan_Exercise_02.AppEnums
{

  public enum CompanyType
  {
    OpenAI,
    Gemini
  }

  public enum OpenAIModels
  {
    [Display(Name = "gpt-4.1-2025-04-14")]
    GPT4,

    [Display(Name = "gpt-4o-mini-2024-07-18")]
    GPT_4o_Mini
  }

  public enum GeminiModels
  {
    [Display(Name = "gemini-2.5-flash-lite")]
    Gemini25FlashLite,

    [Display(Name = "gemini-2.5-flash")]
    Gemini25Flash
  }

  // Helper method to get the Display(Name) value from an enum
  public static class EnumDisplayNameHelper
  {
    public static string GetEnumDisplayName<TEnum>(TEnum value) where TEnum : Enum
    {
      var member = typeof(TEnum).GetMember(value.ToString())[0];
      var displayAttr = (DisplayAttribute)Attribute.GetCustomAttribute(member, typeof(DisplayAttribute));
      return displayAttr?.Name ?? value.ToString();
    }
  }

}

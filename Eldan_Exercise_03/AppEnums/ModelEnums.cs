using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Eldan_Exercise_03.AppEnums
{

  public enum CompanyType
  {
    OpenAI,
    Gemini
  }

  public enum OpenAIModels
  {
    [Display(Name = "gpt-5.2")]
    GPT5dot2,

    [Display(Name = "gpt-4o-mini-2024-07-18")]
    GPT4oMini
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

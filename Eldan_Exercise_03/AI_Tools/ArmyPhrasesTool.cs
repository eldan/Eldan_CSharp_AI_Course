using System;
using System.Collections.Generic;

namespace Eldan_Exercise_03.AI_Tools
{
  public sealed class ArmyPhrasesTool
  {

    private static ArmyPhrasesTool _instance;

    private List<string> knowledgeBase = new List<string>();

    public static ArmyPhrasesTool Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new ArmyPhrasesTool();
        }
        return _instance;
      }
    }

    private ArmyPhrasesTool()
    {
      AddKnowledge();
    }

    private void AddKnowledge()
    {
      knowledgeBase.Add("יש לך פילים בקנה");
      knowledgeBase.Add("ציפיות יש רק בכריות");
      knowledgeBase.Add("פעם תותחן, מספיק");
      knowledgeBase.Add("אני חובש, משמע אתה קיים");
      knowledgeBase.Add("אחורה קפוץ");
      knowledgeBase.Add("אין לנו על מי לסמוך, אלא על אבינו שבמילואים");
      knowledgeBase.Add("שנתיים ושמונה זה רצח שנתיים זה נצח");
      knowledgeBase.Add("פעם טייס - תמיד תותחן");
    }

    public string GetRandomPhrase()
    {
      var rand = new Random();
      return knowledgeBase[rand.Next(knowledgeBase.Count)];
    }
  }
}

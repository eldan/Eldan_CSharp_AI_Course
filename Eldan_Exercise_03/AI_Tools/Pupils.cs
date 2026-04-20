using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan_Exercise_03.AI_Tools
{
  class Pupil
  {
    public string Name { get; set; }
    public List<string> Likes { get; set; } = new List<string>();
    public List<string> Dislikes { get; set; } = new List<string>();
    public List<string> Allergies { get; set; } = new List<string>();
  }
  class Pupils
  {
    static private Pupils _instance;
    Dictionary<string, Pupil> pupils;

    public static Pupils Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new Pupils();
        }
        return _instance;
      }
    }
    private Pupils()
    {
      pupils = new Dictionary<string, Pupil>
      {
        ["Dan"] = new Pupil { Name = "Dan", Likes = { "Emma", "Noah" }, Dislikes = { "Liam" }, Allergies = { "Chocolate" } },
        ["Emma"] = new Pupil { Name = "Emma", Likes = { "Olivia", "Dan" }, Dislikes = { "Sophia" }, Allergies = { "Peanuts" } },
        ["Noah"] = new Pupil { Name = "Noah", Likes = { "Mia" }, Dislikes = { "Lucas" }, Allergies = { "Milk" } },
        ["Olivia"] = new Pupil { Name = "Olivia", Likes = { "Emma", "Ava" }, Dislikes = { "Ethan" }, Allergies = { } },
        ["Liam"] = new Pupil { Name = "Liam", Likes = { "Sophia" }, Dislikes = { "Dan" }, Allergies = { "Gluten" } },
        ["Sophia"] = new Pupil { Name = "Sophia", Likes = { "Liam" }, Dislikes = { "Emma" }, Allergies = { } },
        ["Lucas"] = new Pupil { Name = "Lucas", Likes = { "Ella" }, Dislikes = { "Noah" }, Allergies = { "Eggs" } },
        ["Mia"] = new Pupil { Name = "Mia", Likes = { "Noah" }, Dislikes = { "Ava" }, Allergies = { } },
        ["Ethan"] = new Pupil { Name = "Ethan", Likes = { "James" }, Dislikes = { "Olivia" }, Allergies = { "Soy" } },
        ["Ava"] = new Pupil { Name = "Ava", Likes = { "Olivia" }, Dislikes = { "Mia" }, Allergies = { } },

        ["James"] = new Pupil { Name = "James", Likes = { "Ethan", "Henry" }, Dislikes = { "Leo" }, Allergies = { } },
        ["Ella"] = new Pupil { Name = "Ella", Likes = { "Lucas" }, Dislikes = { "Chloe" }, Allergies = { "Strawberries" } },
        ["Henry"] = new Pupil { Name = "Henry", Likes = { "James" }, Dislikes = { "Jack" }, Allergies = { } },
        ["Leo"] = new Pupil { Name = "Leo", Likes = { "Grace" }, Dislikes = { "James" }, Allergies = { } },
        ["Chloe"] = new Pupil { Name = "Chloe", Likes = { "Zoe" }, Dislikes = { "Ella" }, Allergies = { "Nuts" } },
        ["Jack"] = new Pupil { Name = "Jack", Likes = { "Lily" }, Dislikes = { "Henry" }, Allergies = { } },
        ["Grace"] = new Pupil { Name = "Grace", Likes = { "Leo" }, Dislikes = { "Aria" }, Allergies = { } },
        ["Zoe"] = new Pupil { Name = "Zoe", Likes = { "Chloe" }, Dislikes = { "Nora" }, Allergies = { } },
        ["Lily"] = new Pupil { Name = "Lily", Likes = { "Jack" }, Dislikes = { "Hannah" }, Allergies = { "Dairy" } },
        ["Aria"] = new Pupil { Name = "Aria", Likes = { "Nora" }, Dislikes = { "Grace" }, Allergies = { } },

        ["Nora"] = new Pupil { Name = "Nora", Likes = { "Aria" }, Dislikes = { "Zoe" }, Allergies = { } },
        ["Hannah"] = new Pupil { Name = "Hannah", Likes = { "Ella" }, Dislikes = { "Lily" }, Allergies = { } },
        ["Caleb"] = new Pupil { Name = "Caleb", Likes = { "Isaac" }, Dislikes = { "Aaron" }, Allergies = { "Fish" } },
        ["Isaac"] = new Pupil { Name = "Isaac", Likes = { "Caleb" }, Dislikes = { "Eli" }, Allergies = { } },
        ["Aaron"] = new Pupil { Name = "Aaron", Likes = { "Eli" }, Dislikes = { "Caleb" }, Allergies = { } },
        ["Eli"] = new Pupil { Name = "Eli", Likes = { "Aaron" }, Dislikes = { "Isaac" }, Allergies = { } },
        ["Owen"] = new Pupil { Name = "Owen", Likes = { "Wyatt" }, Dislikes = { "Logan" }, Allergies = { } },
        ["Wyatt"] = new Pupil { Name = "Wyatt", Likes = { "Owen" }, Dislikes = { "Mason" }, Allergies = { } },
        ["Logan"] = new Pupil { Name = "Logan", Likes = { "Mason" }, Dislikes = { "Owen" }, Allergies = { "Shellfish" } },
        ["Mason"] = new Pupil { Name = "Mason", Likes = { "Logan" }, Dislikes = { "Wyatt" }, Allergies = { } }
      };
    }

    public bool CanSitTogether(string nameA, string nameB)
    {
      // Make names case-insensitive by converting to proper case
      nameA = char.ToUpper(nameA[0]) + nameA.Substring(1).ToLower();
      nameB = char.ToUpper(nameB[0]) + nameB.Substring(1).ToLower();

      if (!pupils.ContainsKey(nameA) || !pupils.ContainsKey(nameB))
        throw new ArgumentException($"Pupil not found. Available pupils: {string.Join(", ", pupils.Keys)}");

      var a = pupils[nameA];
      var b = pupils[nameB];

      return !(a.Dislikes.Contains(b.Name) || b.Dislikes.Contains(a.Name));
    }

    public string CanSitTogetherWithReason(string nameA, string nameB)
    {
      nameA = char.ToUpper(nameA[0]) + nameA.Substring(1).ToLower();
      nameB = char.ToUpper(nameB[0]) + nameB.Substring(1).ToLower();

      if (!pupils.ContainsKey(nameA) || !pupils.ContainsKey(nameB))
        return $"Pupil not found. Available pupils: {string.Join(", ", pupils.Keys)}";

      var a = pupils[nameA];
      var b = pupils[nameB];

      if (a.Dislikes.Contains(b.Name))
        return $"No, {nameA} and {nameB} cannot sit together because {nameA} dislikes {nameB}.";
      if (b.Dislikes.Contains(a.Name))
        return $"No, {nameA} and {nameB} cannot sit together because {nameB} dislikes {nameA}.";

      var reasons = new List<string>();
      if (a.Likes.Contains(b.Name)) reasons.Add($"{nameA} likes {nameB}");
      if (b.Likes.Contains(a.Name)) reasons.Add($"{nameB} likes {nameA}");

      if (reasons.Count > 0)
        return $"Yes, {nameA} and {nameB} can sit together. {string.Join(" and ", reasons)}.";

      return $"Yes, {nameA} and {nameB} can sit together. They have no conflicts.";
    }

    public string GetPupilInfo(string name)
    {
      name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
      
      if (!pupils.ContainsKey(name))
        return $"Pupil not found.";

      var p = pupils[name];
      return $"{p.Name}: Likes=[{string.Join(", ", p.Likes)}], Dislikes=[{string.Join(", ", p.Dislikes)}], Allergies=[{string.Join(", ", p.Allergies)}]";
    }

    public string GetAllPupilsWithNoAllergies()
    {
      var noAllergies = pupils.Values.Where(p => p.Allergies.Count == 0).Select(p => p.Name);
      return $"Pupils with no allergies: {string.Join(", ", noAllergies)}";
    }

    public string FindCompatiblePairs()
    {
      var pairs = new List<string>();
      var processed = new HashSet<string>();

      foreach (var p1 in pupils.Values)
      {
        foreach (var p2 in pupils.Values)
        {
          if (p1.Name == p2.Name) continue;
          var key = string.Compare(p1.Name, p2.Name) < 0 ? $"{p1.Name}-{p2.Name}" : $"{p2.Name}-{p1.Name}";
          if (processed.Contains(key)) continue;

          if (!p1.Dislikes.Contains(p2.Name) && !p2.Dislikes.Contains(p1.Name))
          {
            pairs.Add($"{p1.Name} & {p2.Name}");
            processed.Add(key);
          }
        }
      }

      return $"Compatible pairs: {string.Join(", ", pairs)}";
    }
  }
}

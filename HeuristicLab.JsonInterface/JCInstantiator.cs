using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class JCInstantiator {

    private JToken Config { get; set; }
    private Dictionary<string, string> TypeList = new Dictionary<string, string>();

    public IAlgorithm Instantiate(string configFile) {
      Config = JToken.Parse(File.ReadAllText(configFile));
      TypeList = Config["Types"].ToObject<Dictionary<string, string>>();

      JsonItem algorithmData = GetData(Config["Metadata"]["Algorithm"].ToString());
      ResolveReferences(algorithmData);
      IAlgorithm algorithm = CreateObject<IAlgorithm>(algorithmData);
     
      JsonItem problemData = GetData(Config["Metadata"]["Problem"].ToString());
      ResolveReferences(problemData);
      IProblem problem = CreateObject<IProblem>(problemData);
      algorithm.Problem = problem;

      JsonItemConverter.Inject(algorithm, algorithmData);
      JsonItemConverter.Inject(algorithm, problemData);

      return algorithm;
    }

    private void ResolveReferences(JsonItem data) {
      foreach (var p in data.Parameters)
        if (p.Default is string && p.Reference == null)
          p.Reference = GetData(p.Default.Cast<string>());
    }

    private JsonItem GetData(string key)
    {
      foreach(JObject item in Config["Objects"])
      {
        JsonItem data = BuildJsonItem(item);
        if (data.Name == key) return data;
      }
      return null;
    }

    private T CreateObject<T>(JsonItem data) {
      if (TypeList.TryGetValue(data.Name, out string typeName)) {
        Type type = Type.GetType(typeName);
        return (T)Activator.CreateInstance(type);
      } else throw new TypeLoadException($"Cannot find AssemblyQualifiedName for {data.Name}.");
    }

    private JsonItem BuildJsonItem(JObject obj) =>
      new JsonItem() {
        Name = obj[nameof(JsonItem.Name)]?.ToString(),
        Default = obj[nameof(JsonItem.Default)]?.ToObject<object>(),
        Range = obj[nameof(JsonItem.Range)]?.ToObject<object[]>(),
        Type = obj[nameof(JsonItem.Type)]?.ToObject<string>(),
        Parameters = PopulateParameters(obj),
        Operators = PopulateOperators(obj)
      };

    private IList<JsonItem> PopulateParameters(JObject obj) {
      IList<JsonItem> list = new List<JsonItem>();
      if (obj["StaticParameters"] != null)
        foreach (JObject param in obj["StaticParameters"])
          list.Add(BuildJsonItem(param));

      if (obj["FreeParameters"] != null) {
        foreach (JObject param in obj["FreeParameters"]) {
          JsonItem tmp = BuildJsonItem(param);
          JsonItem comp = null;
          foreach (var p in list) // TODO: nicht notwendig, da immer alle params im static block sind
            if (p.Name == tmp.Name) comp = p;
          if (comp != null) 
            JsonItem.Merge(comp, tmp);
          else list.Add(tmp);
        }
      }
      return list;
    }

    private IList<JsonItem> PopulateOperators(JObject obj) {
      IList<JsonItem> list = new List<JsonItem>();
      if (obj[nameof(Operators)] != null)
        foreach (JObject sp in obj[nameof(Operators)]) {
          JsonItem tmp = BuildJsonItem(sp);
          list.Add(tmp);
        }
      return list;
    }
  }
}

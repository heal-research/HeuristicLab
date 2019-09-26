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

namespace HeuristicLab.Manufacture {
  public class JCInstantiator {

    private JToken Config { get; set; }
    private Dictionary<string, string> TypeList = new Dictionary<string, string>();

    public IAlgorithm Instantiate(string configFile) {
      Config = JToken.Parse(File.ReadAllText(configFile));
      TypeList = Config["Types"].ToObject<Dictionary<string, string>>();

      Component algorithmData = GetData(Config["Metadata"]["Algorithm"].ToString());
      ResolveReferences(algorithmData);
      IAlgorithm algorithm = CreateObject<IAlgorithm>(algorithmData);
     
      Component problemData = GetData(Config["Metadata"]["Problem"].ToString());
      ResolveReferences(problemData);
      IProblem problem = CreateObject<IProblem>(problemData);
      algorithm.Problem = problem;

      Transformer.Inject(algorithm, algorithmData);
      Transformer.Inject(algorithm, problemData);

      return algorithm;
    }

    private void ResolveReferences(Component data) {
      foreach (var p in data.Parameters)
        if (p.Default is string && p.Reference == null)
          p.Reference = GetData(p.Default.Cast<string>());
    }

    private Component GetData(string key)
    {
      foreach(JObject item in Config["Objects"])
      {
        Component data = item.ToObject<Component>();// Component.Build(item);
        if (data.Name == key) return data;
      }
      return null;
    }

    private T CreateObject<T>(Component data) {
      if (TypeList.TryGetValue(data.Name, out string typeName)) {
        Type type = Type.GetType(typeName);
        return (T)Activator.CreateInstance(type);
      } else throw new TypeLoadException($"Cannot find AssemblyQualifiedName for {data.Name}.");
    }

    private Component BuildComponent(JObject obj) =>
      new Component() {
        Name = obj[nameof(Component.Name)]?.ToString(),
        Default = obj[nameof(Component.Default)]?.ToObject<object>(),
        Range = obj[nameof(Component.Range)]?.ToObject<object[]>(),
        Type = obj[nameof(Component.Type)]?.ToObject<string>(),
        Parameters = PopulateParameters(obj),
        Operators = PopulateOperators(obj)
      };

    private IList<Component> PopulateParameters(JObject obj) {
      IList<Component> list = new List<Component>();
      if (obj["StaticParameters"] != null)
        foreach (JObject param in obj["StaticParameters"])
          list.Add(BuildComponent(param));

      if (obj["FreeParameters"] != null) {
        foreach (JObject param in obj["FreeParameters"]) {
          Component tmp = BuildComponent(param);
          Component comp = null;
          foreach (var p in list)
            if (p.Name == tmp.Name) comp = p;
          if (comp != null) 
            Component.Merge(comp, tmp);
          else list.Add(tmp);
        }
      }
      return list;
    }

    private IList<Component> PopulateOperators(JObject obj) {
      IList<Component> list = new List<Component>();
      if (obj[nameof(Operators)] != null)
        foreach (JObject sp in obj[nameof(Operators)]) {
          Component tmp = BuildComponent(sp);
          list.Add(tmp);
        }
      return list;
    }
  }
}

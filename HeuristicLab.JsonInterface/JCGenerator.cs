using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.TravelingSalesman;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace HeuristicLab.JsonInterface {
  public class JCGenerator {

    private JObject template = JObject.Parse(@"{
      'Metadata': {
        'Algorithm':'',
        'Problem':''
      },
      'Objects': [],
      'Types': {}
    }");

    private Dictionary<string, string> TypeList = new Dictionary<string, string>();

    JArray jsonItems = new JArray();
    private void PopulateJsonItems(JsonItem item) {
      if (item.Parameters != null) {
        if(item.Range == null)
          jsonItems.Add(Serialize(item));
        foreach (var p in item.Parameters)
          if(p.Parameters != null)
            PopulateJsonItems(p);
      }
    }

    private JObject Serialize(JsonItem item) {
      JObject obj = JObject.FromObject(item, Settings());
      obj["StaticParameters"] = obj["Parameters"];
      obj["FreeParameters"] = obj["Parameters"];

      obj.Property("Parameters")?.Remove();
      RefactorFreeParameters(obj, null);
      RefactorStaticParameters(obj);

      obj.Property("Default")?.Remove();
      obj.Property("Type")?.Remove();

      TypeList.Add(item.Path, item.Type);
      return obj;
    }

    public string GenerateTemplate(IAlgorithm algorithm, IProblem problem, params string[] freeParameters) {
      algorithm.Problem = problem;
      JsonItem algorithmData = JsonItemConverter.Extract(algorithm);
      JsonItem problemData = JsonItemConverter.Extract(problem);
      PopulateJsonItems(algorithmData);
      PopulateJsonItems(problemData);

      template["Metadata"]["Algorithm"] = algorithm.Name;
      template["Metadata"]["Problem"] = problem.Name;
      template["Objects"] = jsonItems;
      template["Types"] = JObject.FromObject(TypeList);

      return CustomJsonWriter.Serialize(template);
    }

    #region Helper
    private void RefactorFreeParameters(JToken token, string[] freeParameters) {


      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<JsonItem>();

        /*bool isSelected = false;
        string name = x["Name"].ToObject<string>();
        foreach (var selected in freeParameters)
          isSelected = (name == selected || isSelected);
        */
        if (/*!isSelected ||*/ p.Default == null || (p.Default != null && p.Default.GetType() == typeof(string) && p.Range == null)) {
          objToRemove.Add(x);
        } else {
          x.Property("Path")?.Remove();
          x.Property("Type")?.Remove();
          x.Property("Parameters")?.Remove();
        }
      }, token["FreeParameters"]);
      foreach (var x in objToRemove) x.Remove();

    }

    private void RefactorStaticParameters(JToken token) {
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<JsonItem>();
        x.Property("Range")?.Remove();
        x.Property("Operators")?.Remove();
        x.Property("Parameters")?.Remove();
        x.Property("Type")?.Remove();
        if (p.Default == null) objToRemove.Add(x);
      }, token["StaticParameters"]);
      foreach (var x in objToRemove) x.Remove();
    }

    private JsonSerializer Settings() => new JsonSerializer() {
      TypeNameHandling = TypeNameHandling.None,
      NullValueHandling = NullValueHandling.Ignore,
      ReferenceLoopHandling = ReferenceLoopHandling.Serialize
    };

    private void TransformNodes(Action<JObject> action, params JToken[] tokens) {
      foreach(JObject obj in tokens.SelectMany(x => x.Children<JObject>()))
        action(obj);
    }
    #endregion
  }
}

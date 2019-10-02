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

    private JObject template = JObject.Parse(Constants.Template);

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
      obj[Constants.StaticParameters] = obj[nameof(JsonItem.Parameters)];
      obj[Constants.FreeParameters] = obj[nameof(JsonItem.Parameters)];

      obj.Property(nameof(JsonItem.Parameters))?.Remove();
      RefactorFreeParameters(obj, null);
      RefactorStaticParameters(obj);

      obj.Property(nameof(JsonItem.Default))?.Remove();
      obj.Property(nameof(JsonItem.Type))?.Remove();

      TypeList.Add(item.Path, item.Type);
      return obj;
    }

    public string GenerateTemplate(IAlgorithm algorithm, IProblem problem, params string[] freeParameters) {
      algorithm.Problem = problem;
      JsonItem algorithmData = JsonItemConverter.Extract(algorithm);
      JsonItem problemData = JsonItemConverter.Extract(problem);
      PopulateJsonItems(algorithmData);
      PopulateJsonItems(problemData);

      template[Constants.Metadata][Constants.Algorithm] = algorithm.Name;
      template[Constants.Metadata][Constants.Problem] = problem.Name;
      template[Constants.Objects] = jsonItems;
      template[Constants.Types] = JObject.FromObject(TypeList);

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
          x.Property(nameof(JsonItem.Path))?.Remove();
          x.Property(nameof(JsonItem.Type))?.Remove();
          x.Property(nameof(JsonItem.Parameters))?.Remove();
        }
      }, token[Constants.FreeParameters]);
      foreach (var x in objToRemove) x.Remove();
    }

    private void RefactorStaticParameters(JToken token) {
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<JsonItem>();
        x.Property(nameof(JsonItem.Range))?.Remove();
        x.Property(nameof(JsonItem.Operators))?.Remove();
        x.Property(nameof(JsonItem.Parameters))?.Remove();
        x.Property(nameof(JsonItem.Type))?.Remove();
        if (p.Default == null) objToRemove.Add(x);
      }, token[Constants.StaticParameters]);
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

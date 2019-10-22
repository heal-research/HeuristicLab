using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class JCGenerator {
    private JObject template = JObject.Parse(Constants.Template);
    private Dictionary<string, string> TypeList = new Dictionary<string, string>();
    private JArray JsonItems { get; set; } = new JArray();

    public string GenerateTemplate(IAlgorithm algorithm) {
      JsonItems.Clear();
      TypeList.Clear();

      // 1.1. extract JsonItem, save the name in the metadata section of the 
      // template and save it an JArray incl. all parameters of the JsonItem, 
      // which have parameters aswell
      AddInstantiableIItem(Constants.Algorithm, algorithm);
      if (algorithm.Problem != null) // 1.2. only when an problem exists
        AddInstantiableIItem(Constants.Problem, algorithm.Problem);

      // 2. save the JArray with JsonItems (= IParameterizedItems)
      template[Constants.Objects] = JsonItems;
      // 3. save the types of the JsonItems (for instatiation)
      template[Constants.Types] = JObject.FromObject(TypeList);
      // 4. serialize template and return string
      return CustomJsonWriter.Serialize(template);
    }

    #region Helper
    private void AddInstantiableIItem(string metaDataTagName, IItem item) {
      JsonItem jsonItem = JsonItemConverter.Extract(item);
      template[Constants.Metadata][metaDataTagName] = item.ItemName;
      PopulateJsonItems(jsonItem);
    }

    private void PopulateJsonItems(JsonItem item) {
      if (item.Parameters != null) {
        if (item.Range == null)
          JsonItems.Add(Serialize(item));
        foreach (var p in item.Parameters)
          if (p.Parameters != null)
            PopulateJsonItems(p);
      }
    }

    private JObject Serialize(JsonItem item) {
      JObject obj = JObject.FromObject(item, Settings());
      obj[Constants.StaticParameters] = obj[nameof(JsonItem.Parameters)];
      obj[Constants.FreeParameters] = obj[nameof(JsonItem.Parameters)];

      obj.Property(nameof(JsonItem.Parameters))?.Remove();
      RefactorFreeParameters(obj);
      RefactorStaticParameters(obj);

      obj.Property(nameof(JsonItem.Value))?.Remove();
      obj.Property(nameof(JsonItem.Type))?.Remove();

      TypeList.Add(item.Path, item.Type);
      return obj;
    }

    private void RefactorFreeParameters(JToken token) {
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<JsonItem>();
        if ((p.Value == null || (p.Value != null && p.Value.GetType() == typeof(string) && p.Range == null) && p.ActualName == null)) {
          objToRemove.Add(x);
        } else {
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
        if (p.Value == null) objToRemove.Add(x);
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

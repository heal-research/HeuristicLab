using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Static class to generate json interface templates.
  /// </summary>
  public static class JCGenerator {
    private struct GenData {
      public JObject Template { get; set; }
      public IDictionary<string, string> TypeList { get; set; }
      public JArray JsonItems { get; set; }
    }
    
    public static string GenerateTemplate(IAlgorithm algorithm) {
      // data container
      GenData genData = new GenData() {
        Template = JObject.Parse(Constants.Template),
        TypeList = new Dictionary<string, string>(),
        JsonItems = new JArray()
      };

      // 1.1. extract JsonItem, save the name in the metadata section of the 
      // template and save it an JArray incl. all parameters of the JsonItem, 
      // which have parameters aswell
      AddInstantiableIItem(Constants.Algorithm, algorithm, genData);
      //IsConvertable(algorithm, true);
      if (algorithm.Problem != null) // 1.2. only when an problem exists
        AddInstantiableIItem(Constants.Problem, algorithm.Problem, genData);

      // 2. save the JArray with JsonItems (= IParameterizedItems)
      genData.Template[Constants.Objects] = genData.JsonItems;
      // 3. save the types of the JsonItems (for instatiation)
      genData.Template[Constants.Types] = JObject.FromObject(genData.TypeList);
      // 4. serialize template and return string
      return CustomJsonWriter.Serialize(genData.Template);
    }
    
    #region Helper
    private static bool IsConvertable(object obj, bool throwException = false) {
      bool tmp = ConvertableAttribute.IsConvertable(obj);
      if (throwException && tmp)
        throw new NotSupportedException($"Type {obj.GetType().GetPrettyName(false)} is not convertable!");
      return tmp;
    }

    private static void AddInstantiableIItem(string metaDataTagName, IItem item, GenData genData) {
      JsonItem jsonItem = JsonItemConverter.Extract(item);
      genData.Template[Constants.Metadata][metaDataTagName] = item.ItemName;
      PopulateJsonItems(jsonItem, genData);
    }

    // serializes ParameterizedItems and saves them in list "JsonItems".
    private static void PopulateJsonItems(JsonItem item, GenData genData) {
      if (item.Parameters != null) {
        if (item.Range == null)
          genData.JsonItems.Add(Serialize(item, genData));
        foreach (var p in item.Parameters)
          if (p.IsParameterizedItem)
            PopulateJsonItems(p, genData);
      }
    }

    private static JObject Serialize(JsonItem item, GenData genData) {
      JObject obj = JObject.FromObject(item, Settings());
      obj[Constants.StaticParameters] = obj[nameof(JsonItem.Parameters)];
      obj[Constants.FreeParameters] = obj[nameof(JsonItem.Parameters)];

      obj.Property(nameof(JsonItem.Parameters))?.Remove();
      RefactorFreeParameters(obj, genData);
      RefactorStaticParameters(obj, genData);

      obj.Property(nameof(JsonItem.Value))?.Remove();
      obj.Property(nameof(JsonItem.Type))?.Remove();

      if(!genData.TypeList.ContainsKey(item.Path))
        genData.TypeList.Add(item.Path, item.Type);
      return obj;
    }

    // deletes unnecessary properties for free parameters.
    private static void RefactorFreeParameters(JToken token, GenData genData) {
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = JsonItem.BuildJsonItem(x, genData.TypeList);
        x.Property(nameof(JsonItem.Type))?.Remove();
        x.Property(nameof(JsonItem.Parameters))?.Remove();
        /*
        if ((p.Value == null || (p.Value != null && p.Value.GetType() == typeof(string) && p.Range == null) && p.ActualName == null)) {
          objToRemove.Add(x);
        } else {
          x.Property(nameof(JsonItem.Type))?.Remove();
          x.Property(nameof(JsonItem.Parameters))?.Remove();
        }*/
      }, token[Constants.FreeParameters]);
      //foreach (var x in objToRemove) x.Remove();
    }

    // deletes unnecessary properties for static parameters.
    private static void RefactorStaticParameters(JToken token, GenData genData) {
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = JsonItem.BuildJsonItem(x, genData.TypeList);
        x.Property(nameof(JsonItem.Range))?.Remove();
        x.Property(nameof(JsonItem.Operators))?.Remove();
        x.Property(nameof(JsonItem.Parameters))?.Remove();
        x.Property(nameof(JsonItem.Type))?.Remove();
        //TODO: maybe allow JsonItems with Value==null in static parameters too?
        if (p.Value == null) objToRemove.Add(x); 
      }, token[Constants.StaticParameters]);
      //foreach (var x in objToRemove) x.Remove();
    }

    /// <summary>
    /// Settings for the json serialization.
    /// </summary>
    /// <returns>A configured JsonSerializer.</returns>
    private static JsonSerializer Settings() => new JsonSerializer() {
      TypeNameHandling = TypeNameHandling.None,
      NullValueHandling = NullValueHandling.Ignore,
      ReferenceLoopHandling = ReferenceLoopHandling.Serialize
    };

    private static void TransformNodes(Action<JObject> action, params JToken[] tokens) {
      foreach(JObject obj in tokens.SelectMany(x => x.Children<JObject>()))
        action(obj);
    }
    #endregion
  }
}

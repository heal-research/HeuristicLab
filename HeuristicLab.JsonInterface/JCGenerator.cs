using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Static class to generate json interface templates.
  /// </summary>
  public static class JCGenerator {
    private struct GenData {
      public JObject Template { get; set; }
      public JArray JsonItems { get; set; }
    }
    
    public static string GenerateTemplate(IOptimizer optimizer) {
      // data container
      GenData genData = new GenData() {
        Template = JObject.Parse(Constants.Template),
        JsonItems = new JArray()
      };

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(optimizer, @"C:\Workspace\template.hl");
      genData.Template[Constants.Metadata][Constants.HLFileLocation] = @"C:\Workspace\template.hl";

      // extract JsonItem, save the name in the metadata section of the 
      // template and save it an JArray incl. all parameters of the JsonItem, 
      // which have parameters aswell
      AddInstantiableIItem(Constants.Optimizer, optimizer, genData);

      // save the JArray with JsonItems (= IParameterizedItems)
      genData.Template[Constants.Parameters] = genData.JsonItems;
      // serialize template and return string
      return SingleLineArrayJsonWriter.Serialize(genData.Template);
    }
    
    #region Helper

    private static void AddInstantiableIItem(string metaDataTagName, IItem item, GenData genData) {
      JsonItem jsonItem = JsonItemConverter.Extract(item);
      
      genData.Template[Constants.Metadata][metaDataTagName] = item.ItemName;
      PopulateJsonItems(jsonItem, genData);
    }

    // serializes ParameterizedItems and saves them in list "JsonItems".
    private static void PopulateJsonItems(JsonItem item, GenData genData) {
      IEnumerable<JsonItem> tmpParameter = item.Children;

      if (item.Value != null || item.Range != null) {
        genData.JsonItems.Add(Serialize(item));
      }

      if (tmpParameter != null) {
        foreach (var p in tmpParameter) {
          PopulateJsonItems(p, genData);
        }
      }
    }

    private static JObject Serialize(JsonItem item) => 
      JObject.FromObject(item, Settings());

    /// <summary>
    /// Settings for the json serialization.
    /// </summary>
    /// <returns>A configured JsonSerializer.</returns>
    private static JsonSerializer Settings() => new JsonSerializer() {
      TypeNameHandling = TypeNameHandling.None,
      NullValueHandling = NullValueHandling.Ignore,
      ReferenceLoopHandling = ReferenceLoopHandling.Serialize
    };
    #endregion
  }
}

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
  /// Class to generate json interface templates.
  /// </summary>
  public class JCGenerator {
    private JObject Template { get; set; }
    private JArray JArrayItems { get; set; }
    private IList<IJsonItem> JsonItems { get; set; }
    private IOptimizer Optimizer { get; set; }

    
    public string GenerateTemplate(IOptimizer optimizer) {
      // data container
      JArrayItems = new JArray();
      JsonItems = new List<IJsonItem>();

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(optimizer, @"C:\Workspace\template.hl");
      Template[Constants.Metadata][Constants.HLFileLocation] = @"C:\Workspace\template.hl";

      // extract JsonItem, save the name in the metadata section of the 
      // template and save it an JArray incl. all parameters of the JsonItem, 
      // which have parameters aswell
      AddIItem(optimizer);
      // save the JArray with JsonItems (= IParameterizedItems)
      Template[Constants.Parameters] = JArrayItems;
      // serialize template and return string
      return SingleLineArrayJsonWriter.Serialize(Template);
    }

    public string GenerateTemplate(IEnumerable<IJsonItem> items) {
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(Optimizer, @"C:\Workspace\template.hl");
      Template[Constants.Metadata][Constants.HLFileLocation] = @"C:\Workspace\template.hl";

      JArray ResultItems = new JArray();

      JArrayItems = new JArray();
      foreach (var item in items) {
        if (item is ResultItem)
          ResultItems.Add(Serialize(item));
        else
          JArrayItems.Add(Serialize(item));
      }
      Template[Constants.Parameters] = JArrayItems;
      Template[Constants.ActivatedResults] = ResultItems;

      // serialize template and return string
      return SingleLineArrayJsonWriter.Serialize(Template);
    }

    public string GenerateTemplate(IJsonItem rootItem, IOptimizer optimizer) {
      // data container
      Template = JObject.Parse(Constants.Template);
      JsonItems = new List<IJsonItem>();

      // extract JsonItem, save the name in the metadata section of the 
      // template and save it an JArray incl. all parameters of the JsonItem, 
      // which have parameters aswell
      Template[Constants.Metadata][Constants.Optimizer] = optimizer.Name;
      PopulateJsonItems(rootItem);

      ProtoBufSerializer serializer = new ProtoBufSerializer();
      serializer.Serialize(Optimizer, @"C:\Workspace\template.hl");
      Template[Constants.Metadata][Constants.HLFileLocation] = @"C:\Workspace\template.hl";

      JArray ResultItems = new JArray();

      JArrayItems = new JArray();
      foreach (var item in JsonItems) {
        if (item is ResultItem)
          ResultItems.Add(Serialize(item));
        else
          JArrayItems.Add(Serialize(item));
      }
      Template[Constants.Parameters] = JArrayItems;
      Template[Constants.ActivatedResults] = ResultItems;

      // serialize template and return string
      return SingleLineArrayJsonWriter.Serialize(Template);
    }

    public IEnumerable<IJsonItem> FetchJsonItems(IOptimizer optimizer) {
      // data container
      Template = JObject.Parse(Constants.Template);
      JsonItems = new List<IJsonItem>();
      Optimizer = optimizer;

      // extract JsonItem, save the name in the metadata section of the 
      // template and save it an JArray incl. all parameters of the JsonItem, 
      // which have parameters aswell
      AddIItem(optimizer);

      // serialize template and return string
      return JsonItems;
    }
    
    #region Helper

    private void AddIItem(IItem item) {
      IJsonItem jsonItem = JsonItemConverter.Extract(item);
      Template[Constants.Metadata][Constants.Optimizer] = item.ItemName;
      PopulateJsonItems(jsonItem);
    }

    // serializes ParameterizedItems and saves them in list "JsonItems".
    private void PopulateJsonItems(IJsonItem item) {
      IEnumerable<IJsonItem> tmpParameter = item.Children;
      
      if (item.Value != null || item.Range != null || item is ResultItem || item.ActualName != null) {
        JsonItems.Add(item);
      }

      if (tmpParameter != null) {
        foreach (var p in tmpParameter) {
          PopulateJsonItems(p);
        }
      }
    }

    private static JObject Serialize(IJsonItem item) => 
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

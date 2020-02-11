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
using System.IO;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Class to generate json interface templates.
  /// </summary>
  public class JCGenerator {

    public void GenerateTemplate(string path, IOptimizer optimizer) =>
      GenerateTemplate(path, optimizer.Name, optimizer);

    public void GenerateTemplate(string path, string templateName, IOptimizer optimizer) =>
      GenerateTemplate(path, templateName, optimizer, JsonItemConverter.Extract(optimizer));

    public void GenerateTemplate(string path, string templateName, IOptimizer optimizer, IJsonItem rootItem) {
      // init
      JObject template = JObject.Parse(Constants.Template);
      JArray parameterItems = new JArray();
      JArray resultItems = new JArray();
      IList<IJsonItem> jsonItems = new List<IJsonItem>();
      string fullPath = Path.GetDirectoryName(Path.GetFullPath(path));
      
      // recursively filter items with values/ranges/actualNames
      PopulateJsonItems(rootItem, jsonItems);

      // serialize hl file
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      string hlFilePath = fullPath + @"\" + templateName + ".hl";
      serializer.Serialize(optimizer, hlFilePath);

      // filter result items
      foreach (var item in jsonItems) {
        if (item is ResultItem)
          resultItems.Add(Serialize(item));
        else
          parameterItems.Add(Serialize(item));
      }
      
      // set template data
      template[Constants.Metadata][Constants.TemplateName] = templateName;
      template[Constants.Metadata][Constants.HLFileLocation] = hlFilePath;
      template[Constants.Parameters] = parameterItems;
      template[Constants.ActivatedResults] = resultItems;

      // serialize template and write file
      File.WriteAllText(fullPath + @"\" + templateName + ".json", SingleLineArrayJsonWriter.Serialize(template));
    }

    #region Helper    
    // serializes ParameterizedItems and saves them in list "JsonItems".
    private void PopulateJsonItems(IJsonItem item, IList<IJsonItem> jsonItems) {
      IEnumerable<IJsonItem> tmpParameter = item.Children;
      
      if (item.Value != null || item.Range != null || item is ResultItem || item.ActualName != null) {
        jsonItems.Add(item);
      }

      if (tmpParameter != null) {
        foreach (var p in tmpParameter) {
          PopulateJsonItems(p, jsonItems);
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

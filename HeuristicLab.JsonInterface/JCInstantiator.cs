using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.SequentialEngine;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Static class to instantiate an IAlgorithm object with a json interface template and config.
  /// </summary>
  public static class JCInstantiator {
    private struct InstData {
      public JToken Template { get; set; }
      public JArray Config { get; set; }
      public IDictionary<string, JsonItem> Objects { get; set; }
      public IDictionary<string, JsonItem> ResolvedItems { get; set; }
    }

    /// <summary>
    /// Instantiate an IAlgorithm object with a template and config.
    /// </summary>
    /// <param name="templateFile">Template file (json), generated with JCGenerator.</param>
    /// <param name="configFile">Config file (json) for the template.</param>
    /// <returns>confugrated IAlgorithm object</returns>
    public static IAlgorithm Instantiate(string templateFile, string configFile = "") {
      InstData instData = new InstData() {
        Objects = new Dictionary<string, JsonItem>(),
        ResolvedItems = new Dictionary<string, JsonItem>()
      };

      // parse template and config files
      instData.Template = JToken.Parse(File.ReadAllText(templateFile));
      if(!string.IsNullOrEmpty(configFile))
        instData.Config = JArray.Parse(File.ReadAllText(configFile));

      // extract metadata information
      string algorithmName = instData.Template[Constants.Metadata][Constants.Algorithm].ToString();
      string problemName = instData.Template[Constants.Metadata][Constants.Problem].ToString();
      string hLFileLocation = instData.Template[Constants.Metadata][Constants.HLFileLocation].ToString();

      // deserialize hl file
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      IAlgorithm algorithm = (IAlgorithm)serializer.Deserialize(hLFileLocation);

      // collect all parameterizedItems from template
      CollectParameterizedItems(instData);

      // rebuild tree with paths
      RebuildTree(instData);

      // if config != null -> merge Template and Config 
      if (instData.Config != null)
        MergeTemplateWithConfig(instData);

      // get algorthm data and object
      JsonItem algorithmData = GetData(algorithmName, instData);

      // get problem data and object
      JsonItem problemData = GetData(problemName, instData);

      // inject configuration
      JsonItemConverter.Inject(algorithm, algorithmData);
      if(algorithm.Problem != null)
        JsonItemConverter.Inject(algorithm.Problem, problemData);

      return algorithm;
    }

    #region Helper
    private static void CollectParameterizedItems(InstData instData) {
      foreach (JObject item in instData.Template[Constants.Parameters]) {
        JsonItem data = JsonItem.BuildJsonItem(item);
        instData.Objects.Add(data.Path, data);
      }
    }
    
    private static JsonItem RebuildTreeHelper(IList<JsonItem> col, string name) {
      JsonItem target = null;
      foreach (var val in col) {
        if (val.Name == name)
          target = val;
      }
      if (target == null) {
        target = new JsonItem() {
          Name = name,
          Path = name,
          Children = new List<JsonItem>()
        };
        col.Add(target);
      }
      return target;
    }

    // rebuilds item tree with splitting paths of each jsonitem
    private static void RebuildTree(InstData instData) {
      List<JsonItem> values = new List<JsonItem>();
      foreach (var x in instData.Objects) {
        string[] pathParts = x.Key.Split('.');
        JsonItem target = RebuildTreeHelper(values, pathParts[0]);

        for (int i = 1; i < pathParts.Length; ++i) {
          target = RebuildTreeHelper(target.Children, pathParts[i]);
        }

        JsonItem.Merge(target, x.Value);
      }
      foreach(var val in values) {
        instData.ResolvedItems.Add(val.Name, val);
      }
    }
    
    private static void MergeTemplateWithConfig(InstData instData) {
      foreach (JObject obj in instData.Config) {
        // build item from config object
        JsonItem item = JsonItem.BuildJsonItem(obj);
        // override default value
        if (instData.Objects.TryGetValue(item.Path, out JsonItem param)) {
          param.Value = item.Value;
          // override ActualName (for LookupParameters)
          if (param.ActualName != null)
            param.ActualName = item.ActualName;
        } else throw new InvalidDataException($"No parameter with path='{item.Path}' defined!");
      }
    }

    private static JsonItem GetData(string key, InstData instData)
    {
      if (instData.ResolvedItems.TryGetValue(key, out JsonItem value))
        return value;
      else
        throw new InvalidDataException($"Type of item '{key}' is not defined!");
    }
    #endregion
  }
}

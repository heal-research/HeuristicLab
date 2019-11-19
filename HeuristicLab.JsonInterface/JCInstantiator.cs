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
      public Dictionary<string, string> TypeList { get; set; }
      public IDictionary<string, JsonItem> ParameterizedItems { get; set; }
      public IDictionary<string, JsonItem> ConfigurableItems { get; set; }
    }

    /// <summary>
    /// Instantiate an IAlgorithm object with a template and config.
    /// </summary>
    /// <param name="templateFile">Template file (json), generated with JCGenerator.</param>
    /// <param name="configFile">Config file (json) for the template.</param>
    /// <returns>confugrated IAlgorithm object</returns>
    public static IAlgorithm Instantiate(string templateFile, string configFile = "") {
      InstData instData = new InstData() {
        TypeList = new Dictionary<string, string>(),
        ParameterizedItems = new Dictionary<string, JsonItem>(),
        ConfigurableItems = new Dictionary<string, JsonItem>()
      };

      //1. Parse Template and Config files
      instData.Template = JToken.Parse(File.ReadAllText(templateFile));
      if(!string.IsNullOrEmpty(configFile))
        instData.Config = JArray.Parse(File.ReadAllText(configFile));
      instData.TypeList = instData.Template[Constants.Types].ToObject<Dictionary<string, string>>();
      string algorithmName = instData.Template[Constants.Metadata][Constants.Algorithm].ToString();
      string problemName = instData.Template[Constants.Metadata][Constants.Problem].ToString();

      //2. Collect all parameterizedItems from template
      CollectParameterizedItems(instData);

      //3. select all ConfigurableItems
      SelectConfigurableItems(instData);

      //4. if config != null -> merge Template and Config 
      if (instData.Config != null)
        MergeTemplateWithConfig(instData);

      //5. resolve the references between parameterizedItems
      ResolveReferences(instData);

      //6. get algorthm data and object
      JsonItem algorithmData = GetData(algorithmName, instData);
      IAlgorithm algorithm = CreateObject<IAlgorithm>(algorithmData, instData);

      //7. get problem data and object
      JsonItem problemData = GetData(problemName, instData);
      IProblem problem = CreateObject<IProblem>(problemData, instData);
      algorithm.Problem = problem;

      //8. inject configuration
      JsonItemConverter.Inject(algorithm, algorithmData);
      JsonItemConverter.Inject(problem, problemData);

      // TODO: let the engine be configurable
      if (algorithm is EngineAlgorithm)
        algorithm.Cast<EngineAlgorithm>().Engine = new SequentialEngine.SequentialEngine();

      return algorithm;
    }

    #region Helper
    private static void CollectParameterizedItems(InstData instData) {
      foreach (JObject item in instData.Template[Constants.Objects]) {
        JsonItem data = BuildJsonItem(item, instData);
        instData.ParameterizedItems.Add(data.Path, data);
      }
    }

    private static void SelectConfigurableItems(InstData instData) {
      foreach (var item in instData.ParameterizedItems.Values) {
        if (item.Parameters != null)
          AddConfigurableItems(item.Parameters, instData);

        if (item.Operators != null)
          AddConfigurableItems(item.Operators, instData);
      }
    }

    private static void AddConfigurableItems(IEnumerable<JsonItem> items, InstData instData) {
      foreach (var item in items)
        if (item.IsConfigurable)
          instData.ConfigurableItems.Add(item.Path, item);
    }

    private static void ResolveReferences(InstData instData) {
      foreach(var x in instData.ParameterizedItems.Values)
        foreach (var p in x.Parameters)
          if (p.Value is string) {
            string key = p.Path;
            if (p.Range != null)
              key = $"{p.Path}.{p.Value.Cast<string>()}";

            if (instData.ParameterizedItems.TryGetValue(key, out JsonItem value))
              p.Reference = value;
          }
    }

    private static void MergeTemplateWithConfig(InstData instData) {
      foreach (JObject obj in instData.Config) {
        // build item from config object
        JsonItem item = BuildJsonItem(obj, instData);
        // override default value
        if (instData.ConfigurableItems.TryGetValue(item.Path, out JsonItem param)) {
          param.Value = item.Value;
          // override ActualName (for LookupParameters)
          if (param.ActualName != null)
            param.ActualName = item.ActualName;
        } else throw new InvalidDataException($"No {Constants.FreeParameters.Trim('s')} with path='{item.Path}' defined!");
      }
    }

    private static JsonItem GetData(string key, InstData instData)
    {
      if (instData.ParameterizedItems.TryGetValue(key, out JsonItem value))
        return value;
      else
        throw new InvalidDataException($"Type of item '{key}' is not defined!");
    }

    private static T CreateObject<T>(JsonItem data, InstData instData) {
      if (instData.TypeList.TryGetValue(data.Name, out string typeName)) {
        Type type = Type.GetType(typeName);
        return (T)Activator.CreateInstance(type);
      } else throw new TypeLoadException($"Cannot find AssemblyQualifiedName for {data.Name}.");
    }

    #region BuildJsonItemMethods
    private static JsonItem BuildJsonItem(JObject obj, InstData instData) =>
      new JsonItem() {
        Name = obj[nameof(JsonItem.Name)]?.ToString(),
        Path = obj[nameof(JsonItem.Path)]?.ToString(),
        Value = obj[nameof(JsonItem.Value)]?.ToObject<object>(),
        Range = obj[nameof(JsonItem.Range)]?.ToObject<object[]>(),
        Type = GetType(obj[nameof(JsonItem.Path)]?.ToObject<string>(), instData),
        ActualName = obj[nameof(JsonItem.ActualName)]?.ToString(),
        Parameters = PopulateParameters(obj, instData),
        Operators = PopulateOperators(obj, instData)
      };

    private static string GetType(string path, InstData instData) {
      if(!string.IsNullOrEmpty(path))
        if (instData.TypeList.TryGetValue(path, out string value))
          return value;
      return null;
    }

    private static IList<JsonItem> PopulateParameters(JObject obj, InstData instData) {
      IList<JsonItem> list = new List<JsonItem>();

      // add staticParameters
      if (obj[Constants.StaticParameters] != null)
        foreach (JObject param in obj[Constants.StaticParameters])
          list.Add(BuildJsonItem(param, instData));

      // merge staticParameter with freeParameter
      if (obj[Constants.FreeParameters] != null) {
        foreach (JObject param in obj[Constants.FreeParameters]) {
          JsonItem tmp = BuildJsonItem(param, instData);
          
          // search staticParameter from list
          JsonItem comp = null;
          foreach (var p in list)
            if (p.Name == tmp.Name) comp = p;
          if (comp == null)
            throw new InvalidDataException($"Invalid {Constants.FreeParameters.Trim('s')}: '{tmp.Name}'!");

          JsonItem.Merge(comp, tmp);
        }
      }
      return list;
    }

    private static IList<JsonItem> PopulateOperators(JObject obj, InstData instData) {
      IList<JsonItem> list = new List<JsonItem>();
      JToken operators = obj[nameof(JsonItem.Operators)];
      if (operators != null)
        foreach (JObject sp in operators)
          list.Add(BuildJsonItem(sp, instData));
      return list;
    }
    #endregion
    #endregion
  }
}

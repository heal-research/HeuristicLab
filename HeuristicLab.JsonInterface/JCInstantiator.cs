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
  public class JCInstantiator {

    private JToken Template { get; set; }
    private JArray Config { get; set; }

    private Dictionary<string, string> TypeList = new Dictionary<string, string>();
    private IDictionary<string, JsonItem> ParameterizedItems { get; set; } = new Dictionary<string, JsonItem>();
    private IDictionary<string, JsonItem> ConfigurableItems { get; set; } = new Dictionary<string, JsonItem>();
    
    public IAlgorithm Instantiate(string templateFile, string configFile = "") {

      //1. Parse Template and Config files
      Template = JToken.Parse(File.ReadAllText(templateFile));
      if(!string.IsNullOrEmpty(configFile))
        Config = JArray.Parse(File.ReadAllText(configFile));
      TypeList = Template[Constants.Types].ToObject<Dictionary<string, string>>();
      string algorithmName = Template[Constants.Metadata][Constants.Algorithm].ToString();
      string problemName = Template[Constants.Metadata][Constants.Problem].ToString();

      //2. Collect all parameterizedItems from template
      CollectParameterizedItems();

      //3. select all ConfigurableItems
      SelectConfigurableItems();

      //4. if config != null -> merge Template and Config 
      if (Config != null)
        MergeTemplateWithConfig();

      //5. resolve the references between parameterizedItems
      ResolveReferences();

      //6. get algorthm data and object
      JsonItem algorithmData = GetData(algorithmName);
      IAlgorithm algorithm = CreateObject<IAlgorithm>(algorithmData);

      //7. get problem data and object
      JsonItem problemData = GetData(problemName);
      IProblem problem = CreateObject<IProblem>(problemData);
      algorithm.Problem = problem;

      //8. inject configuration
      JsonItemConverter.Inject(algorithm, algorithmData);
      JsonItemConverter.Inject(problem, problemData);

      if (algorithm is EngineAlgorithm)
        algorithm.Cast<EngineAlgorithm>().Engine = new SequentialEngine.SequentialEngine();

      return algorithm;
    }

    private void CollectParameterizedItems() {
      foreach (JObject item in Template[Constants.Objects]) {
        JsonItem data = BuildJsonItem(item);
        ParameterizedItems.Add(data.Path, data);
      }
    }

    private void SelectConfigurableItems() {
      foreach (var item in ParameterizedItems.Values) {
        if (item.Parameters != null)
          AddConfigurableItems(item.Parameters);

        if (item.Operators != null)
          AddConfigurableItems(item.Operators);
      }
    }

    private void AddConfigurableItems(IEnumerable<JsonItem> items) {
      foreach (var item in items)
        if (item.IsConfigurable)
          ConfigurableItems.Add(item.Path, item);
    }

    private void ResolveReferences() {
      foreach(var x in ParameterizedItems.Values)
        foreach (var p in x.Parameters)
          if (p.Default is string) {
            string key = p.Path;
            if (p.Range != null)
              key = $"{p.Path}.{p.Default.Cast<string>()}";

            if (ParameterizedItems.TryGetValue(key, out JsonItem value))
              p.Reference = value;
          }
    }

    private void MergeTemplateWithConfig() {
      foreach (JObject obj in Config) {
        // build item from config object
        JsonItem item = BuildJsonItem(obj);
        // override default value
        if (ConfigurableItems.TryGetValue(item.Path, out JsonItem param)) {
          param.Default = item.Default;
          // override default value of reference (for ValueLookupParameters)
          if (param.Reference != null)
            param.Reference.Default = item.Reference?.Default;
        } else throw new InvalidDataException($"No {Constants.FreeParameters.Trim('s')} with path='{item.Path}' defined!");
      }
    }

    private JsonItem GetData(string key)
    {
      if (ParameterizedItems.TryGetValue(key, out JsonItem value))
        return value;
      else
        throw new InvalidDataException($"Type of item '{key}' is not defined!");
    }

    private T CreateObject<T>(JsonItem data) {
      if (TypeList.TryGetValue(data.Name, out string typeName)) {
        Type type = Type.GetType(typeName);
        return (T)Activator.CreateInstance(type);
      } else throw new TypeLoadException($"Cannot find AssemblyQualifiedName for {data.Name}.");
    }

    #region BuildJsonItemMethods
    private JsonItem BuildJsonItem(JObject obj) =>
      new JsonItem() {
        Name = obj[nameof(JsonItem.Name)]?.ToString(),
        Path = obj[nameof(JsonItem.Path)]?.ToString(),
        Default = obj[nameof(JsonItem.Default)]?.ToObject<object>(),
        Range = obj[nameof(JsonItem.Range)]?.ToObject<object[]>(),
        Type = GetType(obj[nameof(JsonItem.Path)]?.ToObject<string>()),
        Reference = obj[nameof(JsonItem.Type)] == null ? 
                    null : 
                    BuildJsonItem(obj[nameof(JsonItem.Type)].ToObject<JObject>()),
        Parameters = PopulateParameters(obj),
        Operators = PopulateOperators(obj)
      };

    private string GetType(string path) {
      if(!string.IsNullOrEmpty(path))
        if (TypeList.TryGetValue(path, out string value))
          return value;
      return null;
    }

    private IList<JsonItem> PopulateParameters(JObject obj) {
      IList<JsonItem> list = new List<JsonItem>();

      // add staticParameters
      if (obj[Constants.StaticParameters] != null)
        foreach (JObject param in obj[Constants.StaticParameters])
          list.Add(BuildJsonItem(param));

      // merge staticParameter with freeParameter
      if (obj[Constants.FreeParameters] != null) {
        foreach (JObject param in obj[Constants.FreeParameters]) {
          JsonItem tmp = BuildJsonItem(param);
          
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

    private IList<JsonItem> PopulateOperators(JObject obj) {
      IList<JsonItem> list = new List<JsonItem>();
      JToken operators = obj[nameof(JsonItem.Operators)];
      if (operators != null)
        foreach (JObject sp in operators)
          list.Add(BuildJsonItem(sp));
      return list;
    }
    #endregion
  }
}

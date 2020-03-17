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
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public struct InstantiatorResult {
    public IOptimizer Optimizer { get; set; }
    public IEnumerable<IResultJsonItem> ConfiguredResultItems { get; set; }
  }


  /// <summary>
  /// Class to instantiate an IAlgorithm object with a json interface template and config.
  /// </summary>
  public class JsonTemplateInstantiator {

    #region Private Properties
    private JToken Template { get; set; }
    private JArray Config { get; set; }
    private IDictionary<string, IJsonItem> Objects { get; set; } = new Dictionary<string, IJsonItem>();
    #endregion

    /// <summary>
    /// Instantiate an IAlgorithm object with a template and config.
    /// </summary>
    /// <param name="templateFile">Template file (json), generated with JCGenerator.</param>
    /// <param name="configFile">Config file (json) for the template.</param>
    /// <returns>confugrated IOptimizer object</returns>
    public static InstantiatorResult Instantiate(string templateFile, string configFile = null) {
      JsonTemplateInstantiator instantiator = new JsonTemplateInstantiator();
      return instantiator.ExecuteInstantiaton(templateFile, configFile);
    }

    #region Helper
    private InstantiatorResult ExecuteInstantiaton(string templateFile, string configFile = null) {
      InstantiatorResult result = new InstantiatorResult();

      #region Parse Files
      Template = JToken.Parse(File.ReadAllText(templateFile));
      if(!string.IsNullOrEmpty(configFile))
        Config = JArray.Parse(File.ReadAllText(configFile));
      #endregion

      // extract metadata information
      string hLFileLocation = Path.GetFullPath(Template[Constants.Metadata][Constants.HLFileLocation].ToString());

      #region Deserialize HL File
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      IOptimizer optimizer = (IOptimizer)serializer.Deserialize(hLFileLocation);
      result.Optimizer = optimizer;
      #endregion

      // collect all parameterizedItems from template
      CollectParameterizedItems(optimizer);
      
      if (Config != null)
        MergeTemplateWithConfig();

      // get algorithm root item
      IJsonItem rootItem = Objects.First().Value;
      
      // inject configuration
      JsonItemConverter.Inject(optimizer, rootItem);

      result.ConfiguredResultItems = CollectResults();

      return result;
    }

    
    private IEnumerable<IResultJsonItem> CollectResults() {
      IList<IResultJsonItem> res = new List<IResultJsonItem>();
      foreach(JObject obj in Template[Constants.Results]) {
        string name = obj.Property("Name").Value.ToString();
        res.Add(new ResultJsonItem() { Name = name });
      }
      return res;
    }

    private void CollectParameterizedItems(IOptimizer optimizer) {
      IJsonItem root = JsonItemConverter.Extract(optimizer);
      Objects.Add(root.Path, root);

      foreach (JObject obj in Template[Constants.Parameters]) {
        string path = obj.Property("Path").Value.ToString();
        foreach(var tmp in root) {
          if(tmp.Path == path) {
            tmp.SetJObject(obj);
            Objects.Add(tmp.Path, tmp);
          }
        }
      }
    }
    
    private void MergeTemplateWithConfig() {
      foreach (JObject obj in Config) {
        // build item from config object
        string path = obj.Property("Path").Value.ToString();
        // override default value
        if (Objects.TryGetValue(path, out IJsonItem param)) {
          // remove fixed template parameter => dont allow to copy them from concrete config
          obj.Property(nameof(IIntervalRestrictedJsonItem<int>.Minimum))?.Remove();
          obj.Property(nameof(IIntervalRestrictedJsonItem<int>.Maximum))?.Remove();
          obj.Property(nameof(IConcreteRestrictedJsonItem<string>.ConcreteRestrictedItems))?.Remove();
          // merge
          param.SetJObject(obj);
        } else throw new InvalidDataException($"No parameter with path='{path}' defined!");
      }
    }
    #endregion
  }
}

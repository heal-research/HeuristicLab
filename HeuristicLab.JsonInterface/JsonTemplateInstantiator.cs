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
  /// <summary>
  /// Static class to instantiate an IAlgorithm object with a json interface template and config.
  /// </summary>
  public static class JsonTemplateInstantiator {
    private struct InstData {
      public JToken Template { get; set; }
      public JArray Config { get; set; }
      public IDictionary<string, IJsonItem> Objects { get; set; }
      public IOptimizer Optimizer { get; set; }
    }

    /// <summary>
    /// Instantiate an IAlgorithm object with a template and config.
    /// </summary>
    /// <param name="templateFile">Template file (json), generated with JCGenerator.</param>
    /// <param name="configFile">Config file (json) for the template.</param>
    /// <returns>confugrated IOptimizer object</returns>
    public static IOptimizer Instantiate(string templateFile, string configFile = "") {
      InstData instData = new InstData() {
        Objects = new Dictionary<string, IJsonItem>()
      };

      // parse template and config files
      instData.Template = JToken.Parse(File.ReadAllText(templateFile));
      if(!string.IsNullOrEmpty(configFile))
        instData.Config = JArray.Parse(File.ReadAllText(configFile));

      // extract metadata information
      string optimizerName = instData.Template[Constants.Metadata][Constants.TemplateName].ToString();
      string hLFileLocation = Path.GetFullPath(instData.Template[Constants.Metadata][Constants.HLFileLocation].ToString());

      // deserialize hl file
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      IOptimizer optimizer = (IOptimizer)serializer.Deserialize(hLFileLocation);
      instData.Optimizer = optimizer;

      // collect all parameterizedItems from template
      CollectParameterizedItems(instData);
      
      // if config != null -> merge Template and Config 
      if (instData.Config != null)
        MergeTemplateWithConfig(instData);

      // get algorthm data and object
      IJsonItem optimizerData = instData.Objects.First().Value;
      
      // inject configuration
      JsonItemConverter.Inject(optimizer, optimizerData);

      return optimizer;
    }

    #region Helper

    private static object GetValueFromJObject(JObject obj) =>
      obj[nameof(IJsonItem.Value)]?.ToObject<object>();

    private static void CollectParameterizedItems(InstData instData) {
      IJsonItem root = JsonItemConverter.Extract(instData.Optimizer);
      instData.Objects.Add(root.Path, root);

      foreach (JObject obj in instData.Template[Constants.Parameters]) {
        string[] pathParts = obj.Property("Path").Value.ToString().Split('.');
        IJsonItem tmp = root;
        IJsonItem old = null;
        for(int i = 1; i < pathParts.Length; ++i) {
          foreach(var c in tmp.Children) {
            if (c.Name == pathParts[i])
              tmp = c;
          }
          if (old == tmp)
            throw new Exception($"Invalid path '{string.Join(".", pathParts)}'");
          else old = tmp;
        }
        tmp.Value = GetValueFromJObject(obj);
        tmp.Range = obj[nameof(IJsonItem.Range)]?.ToObject<object[]>();
        tmp.ActualName = obj[nameof(IJsonItem.ActualName)]?.ToString();
        instData.Objects.Add(tmp.Path, tmp);
      }
    }
    
    private static void MergeTemplateWithConfig(InstData instData) {
      foreach (JObject obj in instData.Config) {
        // build item from config object
        string path = obj.Property("Path").Value.ToString();
        // override default value
        if (instData.Objects.TryGetValue(path, out IJsonItem param)) {
          param.Value = GetValueFromJObject(obj);
          // override ActualName (for LookupParameters)
          if (param.ActualName != null)
            param.ActualName = obj[nameof(IJsonItem.ActualName)]?.ToString();
        } else throw new InvalidDataException($"No parameter with path='{path}' defined!");
      }
    }
    #endregion
  }
}

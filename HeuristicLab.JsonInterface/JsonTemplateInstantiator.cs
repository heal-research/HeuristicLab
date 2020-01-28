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
      string optimizerName = instData.Template[Constants.Metadata][Constants.Optimizer].ToString();
      string hLFileLocation = instData.Template[Constants.Metadata][Constants.HLFileLocation].ToString();

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
      IJsonItem optimizerData = instData.Objects[optimizerName];

      // inject configuration
      JsonItemConverter.Inject(optimizer, optimizerData);

      return optimizer;
    }

    #region Helper

    private static object GetValueFromJObject(JObject obj) {
      object val = obj[nameof(IJsonItem.Value)]?.ToObject<object>();
      if (val is JContainer jContainer) // for resolving array values
        val = jContainer.ToObject<object[]>();

      return val;
    }

    private static void CollectParameterizedItems(InstData instData) {
      //JCGenerator generator = new JCGenerator();
      //IEnumerable<IJsonItem> items = generator.FetchJsonItems(instData.Optimizer);
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


      /*
      foreach (JObject item in instData.Template[Constants.Parameters]) {
        string[] pathParts = item.Property("Path").Value.ToString().Split('.');
        
        // rebuilds object tree
        IJsonItem parent = null;
        StringBuilder partialPath = new StringBuilder();
        for(int i = 0; i < pathParts.Length-1; ++i) {
          partialPath.Append(pathParts[i]);
          IJsonItem tmp = null;
          if (instData.Objects.TryGetValue(partialPath.ToString(), out IJsonItem value)) {
            tmp = value;
          } else {
            tmp = new JsonItem() { Name = pathParts[i] };
            if (parent != null) parent.AddChilds(tmp);
            instData.Objects.Add(partialPath.ToString(), tmp);
          }
          partialPath.Append(".");
          parent = tmp;
        }

        IJsonItem data = JsonItem.BuildJsonItem(item);
        parent.AddChilds(data);
        instData.Objects.Add(data.Path, data);
      }*/
    }
    
    private static void MergeTemplateWithConfig(InstData instData) {
      foreach (JObject obj in instData.Config) {
        // build item from config object
        //IJsonItem item = JsonItem.BuildJsonItem(obj);
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

    private static IJsonItem GetData(string key, InstData instData)
    {
      if (instData.Objects.TryGetValue(key, out IJsonItem value))
        return value;
      else
        throw new InvalidDataException($"Type of item '{key}' is not defined!");
    }
    #endregion
  }
}

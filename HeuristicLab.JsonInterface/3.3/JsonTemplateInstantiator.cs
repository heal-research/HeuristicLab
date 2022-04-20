using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {

  /// <summary>
  /// Class to instantiate an IAlgorithm object with a json interface template and config.
  /// </summary>
  public class JsonTemplateInstantiator {

    #region Constants
    private const string RelativePathCurrentDirectoryRegex = @"^(\.)";
    #endregion

    #region Private Properties
    private JToken Template { get; set; }
    private JArray Config { get; set; }
    private IDictionary<string, JsonItem> Objects { get; } = new Dictionary<string, JsonItem>();
    private JsonItemConverter Converter { get; } = new JsonItemConverter();
    #endregion

    /// <summary>
    /// Instantiate an IAlgorithm object with a template and config.
    /// </summary>
    /// <param name="templateFile">Template file (json), generated with JCGenerator.</param>
    /// <param name="configFile">Config file (json) for the template.</param>
    /// <returns>confugrated IOptimizer object</returns>
    public static IJsonConvertable Instantiate(string templateFile, string configFile = null) {
      JsonTemplateInstantiator instantiator = new JsonTemplateInstantiator();
      return instantiator.ExecuteInstantiaton(templateFile, configFile);
    }

    #region Helper
    private IJsonConvertable ExecuteInstantiaton(string templateFile, string configFile = null) {

      #region Parse Files
      string templateFileFullPath = Path.GetFullPath(templateFile);
      Template = JToken.Parse(File.ReadAllText(templateFile));
      if(!string.IsNullOrEmpty(configFile))
        Config = JArray.Parse(File.ReadAllText(Path.GetFullPath(configFile)));
      #endregion

      #region Deserialize HL File
      // extract metadata information
      string relativePath = Template[Constants.Metadata][Constants.HLFileLocation].ToString().Trim(); // get relative path
      // convert to absolute path
      if (Regex.IsMatch(relativePath, RelativePathCurrentDirectoryRegex))
        relativePath = relativePath.Remove(0, 2); // remove first 2 chars -> indicates the current directory

      string hLFileLocation = Path.Combine(Path.GetDirectoryName(templateFileFullPath), relativePath);
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      IJsonConvertable convertable = (IJsonConvertable)serializer.Deserialize(hLFileLocation);
      #endregion

      // get algorithm root item
      var rootItem = Converter.ConvertToJson(convertable);

      // collect all parameterizedItems from template
      CollectParameterizedItems(rootItem);
      
      if (Config != null)
        MergeTemplateWithConfig();

      // inject configuration
      Converter.ConvertFromJson(convertable, rootItem);

      return convertable;
    }

    private void CollectParameterizedItems(JsonItem root) {
      foreach (JObject obj in Template[Constants.Parameters]) {
        var path = obj[nameof(JsonItem.Path)].ToString();
        foreach (var item in root.Iterate()) {
          if (item.Path == path) {
            item.FromJObject(obj);
            Objects.Add(item.Path, item);
          }
        }
      }
    }

    private void MergeTemplateWithConfig() {
      foreach (JObject obj in Config) {
        // build item from config object
        var prop = obj.Property("Path");
        if (prop == null) 
          throw new ArgumentException("Path property is missing.");

        string path = prop.Value.ToString();
        // override default value
        if (Objects.TryGetValue(path, out JsonItem param)) {
          // merge
          param.FromJObject(obj);
        } else throw new InvalidDataException($"No parameter with path='{path}' defined!");
      }
    }
    #endregion
  }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public readonly struct InstantiatorResult {

    public InstantiatorResult(IJsonConvertable convertable/*, IEnumerable<IRunCollectionModifier> runCollectionModifiers*/) {
      Convertable = convertable;
      //RunCollectionModifiers = runCollectionModifiers;
    }

    public IJsonConvertable Convertable { get; }
    //public IEnumerable<IRunCollectionModifier> RunCollectionModifiers { get; }
  }


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
    public static InstantiatorResult Instantiate(string templateFile, string configFile = null) {
      JsonTemplateInstantiator instantiator = new JsonTemplateInstantiator();
      return instantiator.ExecuteInstantiaton(templateFile, configFile);
    }

    #region Helper
    private InstantiatorResult ExecuteInstantiaton(string templateFile, string configFile = null) {

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
      //convertable.Prepare(true);
      #endregion

      // collect all parameterizedItems from template
      CollectParameterizedItems(convertable);
      
      if (Config != null)
        MergeTemplateWithConfig();

      // get algorithm root item
      JsonItem rootItem = Objects.First().Value;

      // validation
      ValidationResult validationResult = rootItem.GetValidator().Validate();
      if (!validationResult.Success)
        throw validationResult.GenerateException();

      // inject configuration
      Converter.ConvertFromJson(convertable, rootItem);

      return new InstantiatorResult(convertable/*, CollectRunCollectionModifiers()*/);
    }

    /// <summary>
    /// Instantiates all defined (in template) ResultCollectionProcessors and injects the configured parameters.
    /// </summary>
    //private IEnumerable<IRunCollectionModifier> CollectRunCollectionModifiers() {
    //  IList<IRunCollectionModifier> runCollectionModifiers = new List<IRunCollectionModifier>();

    //  if (Template is JObject o && !o.ContainsKey(Constants.RunCollectionModifiers))
    //    return Enumerable.Empty<IRunCollectionModifier>();

    //  foreach (JObject obj in Template[Constants.RunCollectionModifiers]) {
    //    var guid = obj["GUID"].ToString();
    //    var parameters = obj[Constants.Parameters];
    //    var type = Mapper.StaticCache.GetType(new Guid(guid));
    //    var rcModifier = (IRunCollectionModifier)Activator.CreateInstance(type);
    //    var rcModifierItem = Converter.ConvertToJson(rcModifier);

    //    SetJObjects(rcModifierItem, parameters);

    //    Converter.ConvertFromJson(rcModifier, rcModifierItem);
    //    runCollectionModifiers.Add(rcModifier);
    //  }
    //  return runCollectionModifiers;
    //}

    private void CollectParameterizedItems(IJsonConvertable convertable) {
      var root = Converter.ConvertToJson(convertable);
      Objects.Add(root.Path, root);

      foreach (var kvp in SetJObjects(root, Template[Constants.Parameters]))
        Objects.Add(kvp);
    }

    private IDictionary<string, JsonItem> SetJObjects(JsonItem root, JToken parameters) {
      var dict = new Dictionary<string, JsonItem>();
      foreach (JObject obj in parameters) {
        var path = obj[nameof(JsonItem.Path)].ToString();
        foreach (var item in root.Iterate()) {
          if (item.Path == path) {
            item.FromJObject(obj);
            item.Active = true;
            dict.Add(item.Path, item);
          }
        }
      }
      return dict;
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
          // remove fixed template parameter from config => dont allow to copy them from concrete config
          // TODO: shift this into JsonItems?
          obj.Property(nameof(IIntervalRestrictedJsonItem<int>.Minimum))?.Remove();
          obj.Property(nameof(IIntervalRestrictedJsonItem<int>.Maximum))?.Remove();
          obj.Property(nameof(IConcreteRestrictedJsonItem<string>.ConcreteRestrictedItems))?.Remove();
          obj.Property(nameof(IMatrixJsonItem.ColumnsResizable))?.Remove();
          obj.Property(nameof(IMatrixJsonItem.RowsResizable))?.Remove();
          obj.Property(nameof(IArrayJsonItem.Resizable))?.Remove();
          // merge
          param.FromJObject(obj);
        } else throw new InvalidDataException($"No parameter with path='{path}' defined!");
      }
    }
    #endregion
  }
}

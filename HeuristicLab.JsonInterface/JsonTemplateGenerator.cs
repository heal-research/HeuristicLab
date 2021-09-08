using System.Collections.Generic;
using HeuristicLab.Optimization;
using Newtonsoft.Json.Linq;
using HEAL.Attic;
using System.IO;
using HeuristicLab.Core;
using System.Linq;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Class to generate json interface templates.
  /// </summary>
  public class JsonTemplateGenerator {

    /// <summary>
    /// static Function to generate a template.
    /// </summary>
    /// <param name="templatePath">the path for the template files</param>
    /// <param name="optimizer">the optimizer object to serialize</param>
    /// <param name="rootItem">Root JsonItem for serialization, considers only active JsonItems for serialization</param>
    public static void GenerateTemplate(string templatePath, IOptimizer optimizer, IJsonItem rootItem, IEnumerable<IResultCollectionProcessor> resultCollectionProcessors) {
      // clear all runs
      if (optimizer.ExecutionState == ExecutionState.Paused)
        optimizer.Stop();
      optimizer.Runs.Clear();

      // validation
      ValidationResult validationResult = rootItem.GetValidator().Validate();
      if (!validationResult.Success)
        throw validationResult.GenerateException();

      #region Init
      JObject template = JObject.Parse(Constants.Template);
      JArray parameterItems = new JArray();
      JArray resultItems = new JArray();
      JArray resultCollectionProcessorItems = new JArray();
      string templateName = Path.GetFileName(templatePath);
      string templateDirectory = Path.GetDirectoryName(templatePath);
      #endregion

      // filter items with values/ranges/actualNames
      var jsonItems = rootItem.Where(x => x.Active && !(x is EmptyJsonItem) && !(x is UnsupportedJsonItem));

      #region Serialize HL File
      ProtoBufSerializer serializer = new ProtoBufSerializer();
      // get absolute path for serialization
      string hlFilePath = Path.Combine(templateDirectory, $"{templateName}.hl");
      serializer.Serialize(optimizer, hlFilePath);
      // overwrite string for relative path
      hlFilePath = Path.Combine($".", $"{templateName}.hl");
      #endregion

      #region Filter Items
      foreach (var item in jsonItems) {
        if (item is IResultJsonItem)
          resultItems.Add(item.GenerateJObject());
        else
          parameterItems.Add(item.GenerateJObject());
      }
      #endregion

      #region ResultCollectionProcessor Serialization
      foreach (var proc in resultCollectionProcessors) {
        JArray rcpParameterItems = new JArray();
        var guid = StorableTypeAttribute.GetStorableTypeAttribute(proc.GetType()).Guid.ToString();
        var item = JsonItemConverter.Extract(proc);

        var rcpItems = item
          .Where(x => !(x is EmptyJsonItem) && !(x is UnsupportedJsonItem))
          .Select(x => x.GenerateJObject());

        foreach (var i in rcpItems)
          rcpParameterItems.Add(i);

        JObject processorObj = new JObject();
        processorObj.Add(nameof(IJsonItem.Name), item.Name);
        processorObj.Add("GUID", guid);
        processorObj.Add(Constants.Parameters, rcpParameterItems);
        resultCollectionProcessorItems.Add(processorObj);
      }
      #endregion

      #region Set Template Data
      template[Constants.Metadata][Constants.TemplateName] = templateName;
      template[Constants.Metadata][Constants.HLFileLocation] = hlFilePath;
      template[Constants.Parameters] = parameterItems;
      template[Constants.Results] = resultItems;
      template[Constants.ResultCollectionProcessorItems] = resultCollectionProcessorItems;
      #endregion

      #region Serialize and write to file
      File.WriteAllText(Path.Combine(templateDirectory, $"{templateName}.json"), SingleLineArrayJsonWriter.Serialize(template));
      #endregion
    }
  }
}

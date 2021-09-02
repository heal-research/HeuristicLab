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
    public static void GenerateTemplate(string templatePath, IOptimizer optimizer, IJsonItem rootItem, IEnumerable<IResultCollectionPostProcessor> postProcessors) {
      // clear all runs
      optimizer.Runs.Clear();

      // validation
      ValidationResult validationResult = rootItem.GetValidator().Validate();
      if (!validationResult.Success)
        throw validationResult.GenerateException();

      #region Init
      JObject template = JObject.Parse(Constants.Template);
      JArray parameterItems = new JArray();
      JArray resultItems = new JArray();
      JArray postProcessorItems = new JArray();
      // postProcessors.Select(x => new JObject().Add("Name", JToken.Parse(x.GetType().Name))
      foreach (var proc in postProcessors) {
        var tmp = new JObject();
        tmp.Add("Name", proc.GetType().Name);
        postProcessorItems.Add(tmp);
      }

      IList<IJsonItem> jsonItems = new List<IJsonItem>();
     
      string templateName = Path.GetFileName(templatePath);
      string templateDirectory = Path.GetDirectoryName(templatePath);

      #endregion

      if(optimizer.ExecutionState == ExecutionState.Paused)
        optimizer.Stop();

      // recursively filter items with values/ranges/actualNames
      PopulateJsonItems(rootItem, jsonItems);

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

      #region Set Template Data
      template[Constants.Metadata][Constants.TemplateName] = templateName;
      template[Constants.Metadata][Constants.HLFileLocation] = hlFilePath;
      template[Constants.Parameters] = parameterItems;
      template[Constants.Results] = resultItems;
      template["PostProcessors"] = postProcessorItems;
      #endregion

      #region Serialize and write to file
      File.WriteAllText(Path.Combine(templateDirectory, $"{templateName}.json"), SingleLineArrayJsonWriter.Serialize(template));
      #endregion
    }

    #region Helper    
    private static void PopulateJsonItems(IJsonItem item, IList<IJsonItem> jsonItems) {
      foreach(var x in item) { // TODO: dieses konstrukt notwendig?
        if (x.Active && !(x is EmptyJsonItem) && !(x is UnsupportedJsonItem)) {
          jsonItems.Add(x);
        }
      }
    }
    #endregion
  }
}

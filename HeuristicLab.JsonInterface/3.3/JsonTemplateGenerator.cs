using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;
using System.IO;
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
    /// <param name="convertable">the object to serialize</param>
    /// <param name="rootItem">Root JsonItem for serialization, considers only active JsonItems for serialization</param>
    public static void GenerateTemplate(string templatePath, IJsonConvertable convertable/*, JsonItem rootItem*//*, IEnumerable<IRunCollectionModifier> runCollectionModifiers*/) {
      // clear all runs
      /*
      if (optimizer.ExecutionState == ExecutionState.Paused)
        optimizer.Stop();
      optimizer.Runs.Clear();
      */


      var converter = new JsonItemConverter();
      var rootItem = converter.ConvertToJson(convertable);

      // validation
      ValidationResult validationResult = rootItem.GetValidator().Validate();
      if (!validationResult.Success)
        throw validationResult.GenerateException();

      #region Init
      JObject template = JObject.Parse(Constants.Template);
      JArray parameterItems = new JArray();
      JArray resultItems = new JArray();
      JArray runCollectionModifierItems = new JArray();
      string templateName = Path.GetFileName(templatePath);
      string templateDirectory = Path.GetDirectoryName(templatePath);
      #endregion

      // filter items with values/ranges/actualNames
      var jsonItems = rootItem.Iterate();//.Where(x => /*x.Active && !(x is EmptyJsonItem) &&*/ !(x is UnsupportedJsonItem));

      #region Serialize HL File
      //ProtoBufSerializer serializer = new ProtoBufSerializer();
      //// get absolute path for serialization
      //string hlFilePath = Path.Combine(templateDirectory, $"{templateName}.hl");
      //serializer.Serialize(convertable, hlFilePath);
      //// overwrite string for relative path
      //hlFilePath = Path.Combine($".", $"{templateName}.hl");
      #endregion

      #region Filter Items
      foreach (var item in jsonItems) {
        parameterItems.Add(item.ToJObject());
      }
      #endregion

      #region RunCollectionModifiers Serialization
      // separate HL Class -> e.g. filteredOptimizer? (like Batch Run)
      //var converter = new JsonItemConverter();
      //foreach (var rcModifier in runCollectionModifiers) {
      //  JArray rcModifierParameterItems = new JArray();
      //  var guid = StorableTypeAttribute.GetStorableTypeAttribute(rcModifier.GetType()).Guid.ToString();
      //  var item = converter.ConvertToJson(rcModifier);

      //  var rcModifierItems = item
      //    .Where(x => !(x is EmptyJsonItem) && !(x is UnsupportedJsonItem))
      //    .Select(x => x.ToJObject());

      //  foreach (var i in rcModifierItems)
      //    rcModifierParameterItems.Add(i);

      //  JObject rcModifierObj = new JObject();
      //  rcModifierObj.Add(nameof(IJsonItem.Name), item.Name);
      //  rcModifierObj.Add("GUID", guid);
      //  rcModifierObj.Add(Constants.Parameters, rcModifierParameterItems);
      //  runCollectionModifierItems.Add(rcModifierObj);
      //}
      #endregion

      #region Set Template Data
      template[Constants.Metadata][Constants.TemplateName] = templateName;
      //template[Constants.Metadata][Constants.HLFileLocation] = hlFilePath;
      //template[Constants.Metadata][Constants.OptimizerDescription] = convertable.Description;
      template[Constants.Parameters] = parameterItems;
      template[Constants.RunCollectionModifiers] = runCollectionModifierItems;
      #endregion

      #region Serialize and write to file
      File.WriteAllText(Path.Combine(templateDirectory, $"{templateName}.json"), SingleLineArrayJsonWriter.Serialize(template));
      #endregion
    }
  }
}

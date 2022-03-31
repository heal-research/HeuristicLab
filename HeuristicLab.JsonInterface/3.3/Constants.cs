namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Constants for reading/writing templates.
  /// </summary>
  internal class Constants {

    internal const string Metadata = "Metadata";
    internal const string TemplateName = "TemplateName";
    internal const string HLFileLocation = "HLFileLocation";
    internal const string TemplateDescription = "TemplateDescription";
    internal const string Parameters = "Parameters";

    internal const string Template = @"{
      '" + Metadata + @"': {
        '" + TemplateName + @"':'',
        '" + HLFileLocation + @"':'',
        '" + TemplateDescription + @"':''
      },
      '" + Parameters + @"': []
    }";
  }
}

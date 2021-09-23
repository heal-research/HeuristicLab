namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Constants for reading/writing templates.
  /// </summary>
  internal class Constants {

    internal const string Metadata = "Metadata";
    internal const string TemplateName = "TemplateName";
    internal const string HLFileLocation = "HLFileLocation";
    internal const string OptimizerDescription = "OptimizerDescription";
    internal const string Parameters = "Parameters";
    internal const string RunCollectionModifiers = "RunCollectionModifiers";

    internal const string Template = @"{
      '" + Metadata + @"': {
        '" + TemplateName + @"':'',
        '" + HLFileLocation + @"':'',
        '" + OptimizerDescription + @"':''
      },
      '" + Parameters + @"': [],
      '" + RunCollectionModifiers + @"': []
    }";
  }
}

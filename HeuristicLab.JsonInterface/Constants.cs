using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Constants for reading/writing templates.
  /// </summary>
  internal class Constants {

    internal const string Metadata = "Metadata";
    internal const string TemplateName = "TemplateName";
    internal const string HLFileLocation = "HLFileLocation";
    internal const string Parameters = "Parameters";
    internal const string Results = "Results";

    internal const string Template = @"{
      '" + Metadata + @"': {
        '" + TemplateName + @"':'',
        '" + HLFileLocation + @"':''
      },
      '" + Parameters + @"': [],
      '" + Results + @"': []
    }";
  }
}

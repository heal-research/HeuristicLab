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
    internal const string Optimizer = "Optimizer";
    internal const string Problem = "Problem";
    internal const string HLFileLocation = "HLFileLocation";
    internal const string Parameters = "Parameters";
    internal const string ActivatedResults = "ActivatedResults";

    internal const string Template = @"{
      '" + Metadata + @"': {
        '" + Optimizer + @"':'',
        '" + Problem + @"':'',
        '" + HLFileLocation + @"':''
      },
      '" + Parameters + @"': [],
      '" + ActivatedResults + @"': []
    }";
  }
}

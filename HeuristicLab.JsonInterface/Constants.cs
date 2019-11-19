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
    internal const string Algorithm = "Algorithm";
    internal const string Problem = "Problem";
    internal const string Objects = "Objects";
    internal const string Types = "Types";
    internal const string StaticParameters = "StaticParameters";
    internal const string FreeParameters = "FreeParameters";

    internal const string Template = @"{
      'Metadata': {
        'Algorithm':'',
        'Problem':''
      },
      'Objects': [],
      'Types': {}
    }";
  }
}

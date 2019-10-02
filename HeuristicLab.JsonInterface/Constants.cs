using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  internal class Constants {

    public const string Metadata = "Metadata";
    public const string Algorithm = "Algorithm";
    public const string Problem = "Problem";
    public const string Objects = "Objects";
    public const string Types = "Types";
    public const string StaticParameters = "StaticParameters";
    public const string FreeParameters = "FreeParameters";

    public const string Template = @"{
      'Metadata': {
        'Algorithm':'',
        'Problem':''
      },
      'Objects': [],
      'Types': {}
    }";
  }
}

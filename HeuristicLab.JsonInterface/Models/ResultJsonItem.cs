using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class ResultJsonItem : JsonItem, IResultJsonItem {
    protected override ValidationResult Validate() => ValidationResult.Successful();
  }
}

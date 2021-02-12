using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class StringResultFormatter : ResultFormatter {
    public override int Priority => 1;

    public override bool CanFormatType(Type t) => true;

    public override string Format(object o) => o.ToString();
  }
}

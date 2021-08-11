using System;

namespace HeuristicLab.JsonInterface {
  public class StringResultFormatter : ResultFormatter {
    public override int Priority => 1;

    public override bool CanFormatType(Type t) => true;

    public override string Format(object o) => o.ToString();
  }
}

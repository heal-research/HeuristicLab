using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.DebugString.Formatters {

  public abstract class DebugStringFormatterBase<T> : FormatterBase<T, DebugString> {
    public override DebugString Format(T o) { return new DebugString(o.ToString()); }
    public override T Parse(DebugString s) {
      throw new NotImplementedException();
    }
  }
}

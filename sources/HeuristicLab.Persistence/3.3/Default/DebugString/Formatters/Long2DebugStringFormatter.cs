using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.DebugString.Formatters {

  [EmptyStorableClass]
  public class Long2DebugStringFormatter : ValueType2DebugStringFormatterBase<long> { }

}

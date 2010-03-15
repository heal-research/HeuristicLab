using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.DebugString {

  /// <summary>
  /// Simple write-only format for debugging purposes.
  /// </summary>
  [StorableClass]
  public class DebugStringFormat : FormatBase<DebugString> {
    /// <summary>
    /// Gets the format's name.
    /// </summary>
    /// <value>The format's name.</value>
    public override string Name { get { return "DebugString"; } }
  }

}

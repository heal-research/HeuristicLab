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
  [StorableClass(StorableClassType.MarkedOnly)]  
  public class DebugString : ISerialData {

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>The data.</value>
    [Storable]
    public string Data { get; set; }

    private DebugString() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugString"/> class.
    /// </summary>
    /// <param name="s">The string.</param>
    public DebugString(string s) {
      Data = s;
    }

  }

}

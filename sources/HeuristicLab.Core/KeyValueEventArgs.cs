using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicLab.Core {
  /// <summary>
  /// Event arguments to be able to specify the affected key-value pair.
  /// </summary>
  public class KeyValueEventArgs : EventArgs {

    private IItem key;
    /// <summary>
    /// Gets the affected key.
    /// </summary>
    public IItem Key {
      get { return key; }
    }

    private IItem value;
    /// <summary>
    /// Gets the affected value.
    /// </summary>
    public IItem Value {
      get { return value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="KeyValueEventArgs"/> with the given <paramref name="key"/>
    /// and <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The affected key.</param>
    /// <param name="value">The affected value.</param>
    public KeyValueEventArgs(IItem key, IItem value) {
      this.key = key;
      this.value = value; 
    }
  }
}

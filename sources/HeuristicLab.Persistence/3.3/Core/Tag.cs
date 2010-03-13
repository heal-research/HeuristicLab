using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Vehicle used inside the serialization/deserizalisation process
  /// between composite serializers and the core.
  /// </summary>  
  public class Tag {

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public object Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public Tag(string name, object value) {
      Name = name;
      Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class
    /// whithout a name.
    /// </summary>
    /// <param name="value">The value.</param>
    public Tag(object value) {
      Name = null;
      Value = value;
    }
  }

}
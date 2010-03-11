using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Vehicle used inside the serialization/deserizalisation process
  /// between composite serializers and the core.
  /// </summary>  
  public class Tag {
    public string Name { get; private set; }
    public object Value { get; set; }

    public Tag(string name, object value) {
      Name = name;
      Value = value;
    }

    public Tag(object value) {
      Name = null;
      Value = value;
    }
  }

}
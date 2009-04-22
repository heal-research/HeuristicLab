using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

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
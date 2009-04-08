using System;

namespace HeuristicLab.Persistence.Interfaces {

  public abstract class FormatBase : IFormat {
    public abstract string Name { get;  }
    public override bool Equals(object obj) {
      if (obj as IFormat == null)
        return false;
      return this.Equals((FormatBase)obj);      
    }
    public bool Equals(FormatBase f) {
      return Name.Equals(f.Name);
    }
    public override int GetHashCode() {
      return Name.GetHashCode();
    }
  }
}
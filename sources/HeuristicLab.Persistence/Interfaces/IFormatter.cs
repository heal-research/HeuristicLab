using System;

namespace HeuristicLab.Persistence.Interfaces {

  public interface IFormat {
    string Name { get; }
  }

  public abstract class Format : IFormat {
    public abstract string Name { get;  }
    public override bool Equals(object obj) {
      return Name == ((Format) obj).Name;
    }
    public override int GetHashCode() {
      return Name.GetHashCode();
    }
  }

  public interface IFormatter {
    Type Type { get; }
    IFormat Format { get; }
    object DoFormat(object o);
    object Parse(object o);
  }

}
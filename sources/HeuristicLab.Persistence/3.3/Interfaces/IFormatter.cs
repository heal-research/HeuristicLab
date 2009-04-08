using System;

namespace HeuristicLab.Persistence.Interfaces {

  public interface IFormatter {
    Type Type { get; }
    IFormat Format { get; }
    object DoFormat(object o);
    object Parse(object o);
  }

}
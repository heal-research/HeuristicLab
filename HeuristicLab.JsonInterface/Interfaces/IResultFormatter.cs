using System;

namespace HeuristicLab.JsonInterface {
  public interface IResultFormatter {
    /// <summary>
    /// A given priority, higher numbers are prior.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Checks if the given type can be formatted
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    bool CanFormatType(Type t);

    /// <summary>
    /// The format method which formats and object into a string (which can be saved in a JSON file).
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    string Format(object o);
  }
}

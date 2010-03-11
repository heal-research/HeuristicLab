using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Marks the end of a composite element.
  /// </summary>
  public class EndToken : CompositeTokenBase {
    public EndToken(string name, int typeId, int? id) : base(name, typeId, id) { }
  }

}
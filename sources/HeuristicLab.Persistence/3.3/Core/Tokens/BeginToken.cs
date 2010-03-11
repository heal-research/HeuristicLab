using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {


  /// <summary>
  /// Marks the beginning of a composite element.
  /// </summary>
  public class BeginToken : CompositeTokenBase {
    public BeginToken(string name, int typeId, int? id) : base(name, typeId, id) { }
  }

}
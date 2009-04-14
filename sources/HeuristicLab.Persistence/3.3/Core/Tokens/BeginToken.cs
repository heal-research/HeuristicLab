using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  public class BeginToken : CompositeTokenBase {
    public BeginToken(string name, int? typeId, int? id) : base(name, typeId, id) { }
  }

}
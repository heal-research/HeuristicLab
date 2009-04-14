using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  public class EndToken : CompositeTokenBase {
    public EndToken(string name, int? typeId, int? id) : base(name, typeId, id) { }
  }

}
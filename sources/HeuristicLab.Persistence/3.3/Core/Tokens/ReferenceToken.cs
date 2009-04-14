using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {
  
  public class ReferenceToken : SerializationTokenBase {    
    public readonly int Id;
    public ReferenceToken(string name, int id)
      : base(name) {
      Id = id;
    }
  }

}
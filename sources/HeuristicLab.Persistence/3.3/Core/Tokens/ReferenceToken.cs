using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {


  /// <summary>
  /// References a previously used token (composite or primitive).
  /// </summary>
  public class ReferenceToken : SerializationTokenBase {
    public readonly int Id;
    public ReferenceToken(string name, int id)
      : base(name) {
      Id = id;
    }
  }

}
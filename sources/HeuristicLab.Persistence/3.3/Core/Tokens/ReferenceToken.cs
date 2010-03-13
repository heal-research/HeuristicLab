using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {


  /// <summary>
  /// References a previously used token (composite or primitive).
  /// </summary>
  public class ReferenceToken : SerializationTokenBase {
    /// <summary>
    /// The refereced object's id.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceToken"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="id">The referenced object's id.</param>
    public ReferenceToken(string name, int id)
      : base(name) {
      Id = id;
    }
  }

}
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Describes a reference to <c>null</c>.
  /// </summary>
  public class NullReferenceToken : SerializationTokenBase {

    /// <summary>
    /// Initializes a new instance of the <see cref="NullReferenceToken"/> class.
    /// </summary>
    /// <param name="name">The token name.</param>
    public NullReferenceToken(string name) : base(name) { }
  }

}
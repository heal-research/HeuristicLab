using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Marks the end of a composite element.
  /// </summary>
  public class EndToken : CompositeTokenBase {

    /// <summary>
    /// Initializes a new instance of the <see cref="EndToken"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="typeId">The type id.</param>
    /// <param name="id">The object id.</param>
    public EndToken(string name, int typeId, int? id) : base(name, typeId, id) { }
  }

}
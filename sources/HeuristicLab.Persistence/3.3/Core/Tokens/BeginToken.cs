using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {


  /// <summary>
  /// Marks the beginning of a composite element.
  /// </summary>
  public class BeginToken : CompositeTokenBase {

    /// <summary>
    /// Initializes a new instance of the <see cref="BeginToken"/> class.
    /// </summary>
    /// <param name="name">The token name.</param>
    /// <param name="typeId">The type id.</param>
    /// <param name="id">The object id.</param>
    public BeginToken(string name, int typeId, int? id) : base(name, typeId, id) { }
  }

}

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Common base class of <c>BeginToken</c> and <c>EndToken</c>
  /// that surround a composite element.
  /// </summary>
  public abstract class CompositeTokenBase : SerializationTokenBase {

    /// <summary>
    /// The type's id.
    /// </summary>
    public readonly int TypeId;


    /// <summary>
    /// The object's id for references in case it is a reference type.
    /// </summary>
    public readonly int? Id;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeTokenBase"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="typeId">The type id.</param>
    /// <param name="id">The object id.</param>
    public CompositeTokenBase(string name, int typeId, int? id)
      : base(name) {
      TypeId = typeId;
      Id = id;
    }
  }

}

namespace HeuristicLab.Persistence.Core.Tokens {

  /// <summary>
  /// Common base class of <code>BeginToken</code> and <code>EndToken</code>
  /// that surround a composite element.
  /// </summary>
  public abstract class CompositeTokenBase : SerializationTokenBase {
    public readonly int TypeId;
    public readonly int? Id;
    public CompositeTokenBase(string name, int typeId, int? id)
      : base(name) {
      TypeId = typeId;
      Id = id;
    }
  }

}
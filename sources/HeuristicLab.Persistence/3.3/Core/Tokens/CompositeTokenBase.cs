
namespace HeuristicLab.Persistence.Core.Tokens {

  public abstract class CompositeTokenBase : SerializationTokenBase {
    public readonly int? TypeId;
    public readonly int? Id;
    public CompositeTokenBase(string name, int? typeId, int? id)
      : base(name) {
      TypeId = typeId;
      Id = id;
    }
  }

}
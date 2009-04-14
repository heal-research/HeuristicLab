using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  public abstract class SerializationTokenBase : ISerializationToken {
    public readonly string Name;
    public SerializationTokenBase(string name) {
      Name = name;
    }
  }

}
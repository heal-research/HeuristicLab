
namespace HeuristicLab.Persistence.Interfaces.Tokens {

  public abstract class SerializationTokenBase : ISerializationToken {
    public readonly string Name;
    public SerializationTokenBase(string name) {
      Name = name;
    }
  }

}
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {
  
  public class PrimitiveToken : SerializationTokenBase {    
    public readonly int TypeId;
    public readonly int? Id;
    public readonly ISerialData SerialData;
    public PrimitiveToken(string name, int typeId, int? id, ISerialData serialData)
      : base(name) {
      TypeId = typeId;
      Id = id;
      SerialData = serialData;
    }
  }

}
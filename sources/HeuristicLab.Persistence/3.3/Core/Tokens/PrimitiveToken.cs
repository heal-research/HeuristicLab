using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {
  
  public class PrimitiveToken : SerializationTokenBase {    
    public readonly int TypeId;
    public readonly int? Id;
    public readonly object SerialData;
    public PrimitiveToken(string name, int typeId, int? id, object serialData)
      : base(name) {
      TypeId = typeId;
      Id = id;
      SerialData = serialData;
    }
  }

}
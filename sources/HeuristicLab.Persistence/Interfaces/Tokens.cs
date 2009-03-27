
namespace HeuristicLab.Persistence.Interfaces {

  public interface ISerializationToken {}

  public class BeginToken : ISerializationToken {    
    public readonly string Name;
    public readonly int? TypeId;
    public readonly int? Id;
    public BeginToken(string name, int? typeId, int? id) {
      Name = name;
      TypeId = typeId;
      Id = id;
    }
  }

  public class EndToken : ISerializationToken {
    public readonly string Name;
    public readonly int? TypeId;
    public readonly int? Id;
    public EndToken(string name, int? typeId, int? id) {
      Name = name;
      TypeId = typeId;
      Id = id;
    }
  }

  public class PrimitiveToken : ISerializationToken {
    public readonly string Name;
    public readonly int? TypeId;
    public readonly int? Id;
    public readonly object SerialData;
    public PrimitiveToken(string name, int? typeId, object serialData, int? id) {
      Name = name;
      TypeId = typeId;
      SerialData = serialData;      
      Id = id;
    }
  }

  public class ReferenceToken : ISerializationToken {
    public readonly string Name;
    public readonly int Id;
    public ReferenceToken(string name, int id) {
      Name = name;
      Id = id;
    }
  }

  public class NullReferenceToken : ISerializationToken {
    public readonly string Name;
    public NullReferenceToken(string name) {
      Name = name;
    }
  }

}
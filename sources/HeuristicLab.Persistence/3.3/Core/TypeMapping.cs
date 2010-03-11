using System.Collections.Generic;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Association of id, type name and serializer
  /// </summary>
  public class TypeMapping {

    public readonly int Id;
    public readonly string TypeName;
    public readonly string Serializer;

    public TypeMapping(int id, string typeName, string serializer) {
      Id = id;
      TypeName = typeName;
      Serializer = serializer;
    }

    public Dictionary<string, object> GetDict() {
      return new Dictionary<string, object> {
        {"id", Id},
        {"typeName", TypeName},
        {"serializer", Serializer}};
    }

  }

}
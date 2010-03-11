using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core.Tokens {

  public class TypeToken : ISerializationToken {
    public readonly int Id;
    public readonly string TypeName;
    public readonly string Serializer;

    public TypeToken(int id, string typeName, string serializer) {
      Id = id;
      TypeName = typeName;
      Serializer = serializer;
    }
  }
}

using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class Char2XmlSerializer : PrimitiveSerializerBase<char, XmlString> {  
    
    public override XmlString Format(char c) {
      return new XmlString(new string(c, 1));
    }

    public override char Parse(XmlString x) {
      if (x.Data.Length != 1)
        throw new PersistenceException("Invalid character format, XML string length != 1");
      return x.Data[0];
    }
  }
}
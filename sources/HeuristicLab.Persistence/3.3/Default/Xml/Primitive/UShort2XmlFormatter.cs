using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class UShort2XmlFormatter : SimpleNumber2XmlFormatterBase<ushort> { }

}
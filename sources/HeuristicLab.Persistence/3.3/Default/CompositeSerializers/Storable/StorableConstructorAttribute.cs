using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Indicate that this constructor should be used instead of the default constructor
  /// when the <c>StorableSerializer</c> instantiates this class during
  /// deserialization.
  /// 
  /// The constructor must take exactly one <c>bool</c> argument that will be
  /// set to <c>true</c> during deserialization.
  /// </summary>
  [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
  public sealed class StorableConstructorAttribute : Attribute {  }
}

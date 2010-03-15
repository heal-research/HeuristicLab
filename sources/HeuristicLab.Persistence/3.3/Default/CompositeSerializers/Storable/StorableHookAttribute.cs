using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Indicates the time at which the hook should be invoked.
  /// </summary>
  public enum HookType {

    /// <summary>
    /// States that this hook should be called before the storable
    /// serializer starts decomposing the object.
    /// </summary>
    BeforeSerialization,
    
    /// <summary>
    /// States that this hook should be called after the storable
    /// serializer hast complete re-assembled the object.
    /// </summary>
    AfterDeserialization };


  /// <summary>
  /// Mark methods that should be called at certain times during
  /// serialization/deserialization by the <c>StorableSerializer</c>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
  public sealed class StorableHookAttribute : Attribute {    

    private readonly HookType hookType;
    /// <summary>
    /// Gets the type of the hook.
    /// </summary>
    /// <value>The type of the hook.</value>
    public HookType HookType {
      get { return hookType; }
    }

    /// <summary>
    /// Mark method as <c>StorableSerializer</c> hook to be run
    /// at the <c>HookType</c> time.
    /// </summary>
    /// <param name="hookType">Type of the hook.</param>
    public StorableHookAttribute(HookType hookType) {
      this.hookType = hookType;
    }    
  }
}
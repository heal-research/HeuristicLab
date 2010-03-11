using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Persistence.Core;
using System.Windows.Forms;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Reflection;

namespace HeuristicLab.Persistence.GUI {
  public class PersistenceAnalysis {

    public static bool IsSerializable(Type type, Configuration config) {
      if (config.PrimitiveSerializers.Any(ps => ps.SourceType == type))
        return true;
      foreach (var cs in config.CompositeSerializers) {
        if (cs.CanSerialize(type))
          return true;
      }
      return false;
    }

    private static bool DerivesFrom(Type baseType, Type type) {
      if (type == baseType)
        return true;
      if (type.BaseType == null)
        return false;
      return DerivesFrom(baseType, type.BaseType);
    }

    public static IEnumerable<Type> NonSerializableTypes(Configuration config) {
      var types = new List<Type>();
      var storableInconsistentcy = new List<Type>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        if (assembly.FullName.StartsWith("System.") ||
          assembly.FullName.StartsWith("HeuristicLab.PluginInfrastructure") ||
          assembly.FullName.StartsWith("log4net") ||
          assembly.FullName.StartsWith("WindowsBase") ||
          assembly.FullName.StartsWith("WeifenLuo") ||
          assembly.FullName.StartsWith("ICSharpCode") ||
          assembly.FullName.StartsWith("Mono") ||
          assembly.FullName.StartsWith("Netron"))
          continue;
        foreach (var type in assembly.GetTypes()) {          
          if (type.IsInterface || type.IsAbstract ||
              type.FullName.StartsWith("System.") ||
              type.FullName.StartsWith("Microsoft.") ||              
              type.FullName.Contains("<") ||
              type.FullName.Contains(">") ||
              DerivesFrom(typeof(Exception), type) ||
              DerivesFrom(typeof(Control), type) ||
              DerivesFrom(typeof(System.EventArgs), type) ||
              DerivesFrom(typeof(System.Attribute), type) ||
              type.GetInterface("HeuristicLab.MainForm.IUserInterfaceItem") != null
            )            
            continue;
          try {
            if (!IsSerializable(type, config))
              types.Add(type);
            if (!IsCorrectlyStorable(type))
              storableInconsistentcy.Add(type);
          } catch {
            types.Add(type);
          }
        }
      }
      return types;
    }

    private static bool IsCorrectlyStorable(Type type) {
      if (StorableAttribute.GetStorableMembers(type).Count() > 0) {
        if (!StorableClassAttribute.IsStorableType(type, true))
          return false;
        if (type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null) == null &&
          StorableConstructorAttribute.GetStorableConstructor(type) == null)
          return false;
      }
      return true;
    }    
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Reflection;
using HeuristicLab.Persistence.Auxiliary;
using System.Text;
using System.Reflection.Emit;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  internal static class StorableReflection {

    private const BindingFlags DECLARED_INSTANCE_MEMBERS =
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.DeclaredOnly;

    private delegate void HookWrapper<T>(T o);
    public delegate void Hook(object o);

    public static IEnumerable<StorableMemberInfo> GenerateStorableMembers(Type type) {
      var storableMembers = new List<StorableMemberInfo>();
      if (type.BaseType != null)
        storableMembers.AddRange(GenerateStorableMembers(type.BaseType));

      var storableClassAttribute = GetStorableClassAttribute(type);
      if (storableClassAttribute != null) {
        switch (storableClassAttribute.Type) {
          case StorableClassType.MarkedOnly:
            AddMarkedMembers(type, storableMembers); break;
          case StorableClassType.AllFields:
            AddAll(type, MemberTypes.Field, storableMembers); break;
          case StorableClassType.AllProperties:
            AddAll(type, MemberTypes.Property, storableMembers); break;
          case StorableClassType.AllFieldsAndAllProperties:
            AddAll(type, MemberTypes.Field | MemberTypes.Property, storableMembers); break;
          default:
            throw new PersistenceException("unsupported [StorableClassType]: " + storableClassAttribute.Type);
        }
      }

      return DisentangleNameMapping(storableMembers);
    }

    public static bool IsEmptyOrStorableType(Type type, bool recursive) {      
      if (!HasStorableClassAttribute(type) && !IsEmptyType(type, false)) return false;
      return !recursive || type.BaseType == null || IsEmptyOrStorableType(type.BaseType, true);
    }

    private static object[] emptyArgs = new object[0];

    public static IEnumerable<Hook> CollectHooks(HookType hookType, Type type) {
      if (type.BaseType != null)
        foreach (var mi in CollectHooks(hookType, type.BaseType))
          yield return mi;
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        foreach (StorableHookAttribute hook in memberInfo.GetCustomAttributes(typeof(StorableHookAttribute), false)) {
          if (hook != null && hook.HookType == hookType) {
            MethodInfo methodInfo = memberInfo as MethodInfo;
            if (memberInfo.MemberType != MemberTypes.Method || memberInfo == null)
              throw new ArgumentException("Storable hooks must be methods");
            if (System.IntPtr.Size == 4) {
              DynamicMethod dm = new DynamicMethod("", null, new[] { typeof(object) }, type);
              ILGenerator ilgen = dm.GetILGenerator();
              ilgen.Emit(OpCodes.Ldarg_1);
              ilgen.Emit(OpCodes.Call, methodInfo);
              ilgen.Emit(OpCodes.Ret);
              yield return (Hook)dm.CreateDelegate(typeof(Hook));
            } else {
              yield return new Hook(o => methodInfo.Invoke(o, emptyArgs));
            }
          }
        }
      }
    }

    #region [Storable] helpers

    private static void AddMarkedMembers(Type type, List<StorableMemberInfo> storableMembers) {
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        foreach (StorableAttribute attribute in memberInfo.GetCustomAttributes(typeof(StorableAttribute), false)) {
          storableMembers.Add(new StorableMemberInfo(attribute, memberInfo));
        }
      }
    }

    private static void AddAll(Type type, MemberTypes memberTypes, List<StorableMemberInfo> storableMembers) {
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        if ((memberInfo.MemberType & memberTypes) == memberInfo.MemberType &&
            !memberInfo.Name.StartsWith("<") &&
            !memberInfo.Name.EndsWith("k__BackingField"))
          storableMembers.Add(new StorableMemberInfo(memberInfo));
      }
    }


    private static IEnumerable<StorableMemberInfo> DisentangleNameMapping(
        IEnumerable<StorableMemberInfo> storableMemberInfos) {
      var nameGrouping = new Dictionary<string, List<StorableMemberInfo>>();
      foreach (StorableMemberInfo storable in storableMemberInfos) {
        if (!nameGrouping.ContainsKey(storable.MemberInfo.Name))
          nameGrouping[storable.MemberInfo.Name] = new List<StorableMemberInfo>();
        nameGrouping[storable.MemberInfo.Name].Add(storable);
      }
      var memberInfos = new List<StorableMemberInfo>();
      foreach (var storableMemberInfoGroup in nameGrouping.Values) {
        if (storableMemberInfoGroup.Count == 1) {
          storableMemberInfoGroup[0].SetDisentangledName(storableMemberInfoGroup[0].MemberInfo.Name);
          memberInfos.Add(storableMemberInfoGroup[0]);
        } else if (storableMemberInfoGroup[0].MemberInfo.MemberType == MemberTypes.Field) {
          foreach (var storableMemberInfo in storableMemberInfoGroup) {
            storableMemberInfo.SetDisentangledName(storableMemberInfo.FullyQualifiedMemberName);
            memberInfos.Add(storableMemberInfo);
          }
        } else {
          memberInfos.AddRange(MergePropertyAccessors(storableMemberInfoGroup));
        }
      }
      return memberInfos;
    }

    private static IEnumerable<StorableMemberInfo> MergePropertyAccessors(List<StorableMemberInfo> members) {
      var uniqueAccessors = new Dictionary<Type, StorableMemberInfo>();
      foreach (var member in members)
        uniqueAccessors[member.GetPropertyDeclaringBaseType()] = member;
      if (uniqueAccessors.Count == 1) {
        var storableMemberInfo = uniqueAccessors.Values.First();
        storableMemberInfo.SetDisentangledName(storableMemberInfo.MemberInfo.Name);
        yield return storableMemberInfo;
      } else {
        foreach (var attribute in uniqueAccessors.Values) {
          attribute.SetDisentangledName(attribute.FullyQualifiedMemberName);
          yield return attribute;
        }
      }
    }

    #endregion

    #region [StorableClass] helpers

    private static StorableClassAttribute GetStorableClassAttribute(Type type) {
      return (StorableClassAttribute)type
        .GetCustomAttributes(typeof(StorableClassAttribute), false)
        .SingleOrDefault();
    }

    private static bool HasStorableClassAttribute(Type type) {
      return type.GetCustomAttributes(typeof(StorableClassAttribute), false).Length > 0;
    }

    #endregion

    #region other helpers

    private static bool IsEmptyType(Type type, bool recursive) {
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        if (IsModifiableMember(memberInfo)) return false;
      }
      return !recursive || type.BaseType == null || IsEmptyType(type.BaseType, true);
    }

    private static bool IsModifiableMember(MemberInfo memberInfo) {
      return memberInfo.MemberType == MemberTypes.Field && IsModifiableField((FieldInfo)memberInfo) ||
             memberInfo.MemberType == MemberTypes.Property && IsModifiableProperty((PropertyInfo)memberInfo);
    }

    private static bool IsModifiableField(FieldInfo fi) {
      return !fi.IsLiteral && !fi.IsInitOnly;
    }

    private static bool IsModifiableProperty(PropertyInfo pi) {
      return pi.CanWrite;
    }

    #endregion

  }
}
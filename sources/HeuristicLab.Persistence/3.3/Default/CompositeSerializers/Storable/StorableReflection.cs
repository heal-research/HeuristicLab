using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Reflection;
using HeuristicLab.Persistence.Auxiliary;
using System.Text;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  internal static class StorableReflection {

    private const BindingFlags DECLARED_INSTANCE_MEMBERS =
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.DeclaredOnly;


    public static IEnumerable<StorableMemberInfo> GenerateStorableMembers(Type type, bool inherited) {
      var storableMembers = new List<StorableMemberInfo>();
      if (inherited && type.BaseType != null)
        storableMembers.AddRange(GenerateStorableMembers(type.BaseType, true));

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

    private static StorableClassAttribute GetStorableClassAttribute(Type type) {
      return (StorableClassAttribute)type
        .GetCustomAttributes(typeof(StorableClassAttribute), false)
        .SingleOrDefault();
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

    public static bool IsEmptyOrStorableType(Type type, bool recusrive) {
      if (IsEmptyType(type, recusrive)) return true;
      if (!HastStorableClassAttribute(type)) return false;
      return !recusrive || type.BaseType == null || IsEmptyOrStorableType(type.BaseType, true);
    }

    private static bool HastStorableClassAttribute(Type type) {
      return type.GetCustomAttributes(typeof(StorableClassAttribute), false).Length > 0;
    }

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
  }
}
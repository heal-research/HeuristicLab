#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HEAL.Fossil;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  internal static class StorableReflection {

    private const BindingFlags DECLARED_INSTANCE_MEMBERS =
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.DeclaredOnly;

    public delegate void Hook(object o);

    public static IEnumerable<StorableMemberInfo> GenerateStorableMembers(Type type) {
      var storableMembers = new List<StorableMemberInfo>();
      if (type.BaseType != null)
        storableMembers.AddRange(GenerateStorableMembers(type.BaseType));

      var storableTypeAttribute = GetStorableTypeAttribute(type);
      if (storableTypeAttribute != null) {
        switch (storableTypeAttribute.MemberSelection) {
          case StorableMemberSelection.MarkedOnly:
            AddMarkedMembers(type, storableMembers); break;
          case StorableMemberSelection.AllFields:
            AddAll(type, MemberTypes.Field, storableMembers); break;
          case StorableMemberSelection.AllProperties:
            AddAll(type, MemberTypes.Property, storableMembers); break;
          case StorableMemberSelection.AllFieldsAndAllProperties:
            AddAll(type, MemberTypes.Field | MemberTypes.Property, storableMembers); break;
          default:
            throw new PersistenceException("unsupported [StorableMemberSelection]: " + storableTypeAttribute.MemberSelection);
        }
      }
      return DisentangleNameMapping(storableMembers);
    }

    public static bool IsEmptyOrStorableType(Type type, bool recursive) {
      if (!HasStorableTypeAttribute(type) && !IsEmptyType(type, false)) return false;
      return !recursive || type.BaseType == null || IsEmptyOrStorableType(type.BaseType, true);
    }

    private static object[] emptyArgs = new object[0];

    public static IEnumerable<Hook> CollectHooks(HookType hookType, Type type) {
      if (type.BaseType != null)
        foreach (var hook in CollectHooks(hookType, type.BaseType))
          yield return hook;
      if (HasStorableTypeAttribute(type)) {
        foreach (MethodInfo methodInfo in type.GetMethods(DECLARED_INSTANCE_MEMBERS)) {
          if (methodInfo.ReturnType == typeof(void) && methodInfo.GetParameters().Length == 0) {
            foreach (StorableHookAttribute hook in methodInfo.GetCustomAttributes(typeof(StorableHookAttribute), false)) {
              if (hook != null && hook.HookType == hookType) {
                yield return CreateHook(methodInfo);
              }
            }
          }
        }
      }
    }

    private static Hook CreateHook(MethodInfo methodInfo) {
      return new Hook((o) => methodInfo.Invoke(o, emptyArgs));
    }

    #region [Storable] helpers

    private static void AddMarkedMembers(Type type, List<StorableMemberInfo> storableMembers) {
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        if (memberInfo.MemberType == MemberTypes.Field ||
          memberInfo.MemberType == MemberTypes.Property) {
          foreach (StorableAttribute attribute in memberInfo.GetCustomAttributes(typeof(StorableAttribute), false)) {
            storableMembers.Add(new StorableMemberInfo(attribute, memberInfo));
          }
        }
      }
    }

    private static void AddAll(Type type, MemberTypes memberTypes, List<StorableMemberInfo> storableMembers) {
      foreach (MemberInfo memberInfo in type.GetMembers(DECLARED_INSTANCE_MEMBERS)) {
        if ((memberInfo.MemberType & memberTypes) == memberInfo.MemberType &&
            !memberInfo.Name.StartsWith("<") &&
            !memberInfo.Name.EndsWith("k__BackingField"))
          storableMembers.Add(new StorableMemberInfo(memberInfo, false));
      }
    }

    /// <summary>
    /// Ascertain distinct names for all fields and properties. This method takes care
    /// of disentangling equal names from different class hiarachy levels.
    /// 
    /// Field names are replaced with their fully qualified name which includes
    /// the class names where they were declared.
    /// 
    /// Property names are first reduced to unqiue accessors that are not overrides of
    /// each other and the replaced with their fully qualified name if more than one
    /// accessor remains.
    /// </summary>
    /// <param name="storableMemberInfos"></param>
    /// <returns></returns>
    private static IEnumerable<StorableMemberInfo> DisentangleNameMapping(IEnumerable<StorableMemberInfo> storableMemberInfos) {
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

    /// <summary>
    /// Merges property accessors that are overrides of each other but differentiates if a new
    /// property that shadows older implementations has been introduced with <code>new</code>.
    /// </summary>
    /// <param name="members">A list of <code>StorableMemberInfo</code>s for properties of the same type.</param>
    /// <returns>A fieltered <code>IEnumerable</code> of propery infos.</returns>
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

    private static StorableTypeAttribute GetStorableTypeAttribute(Type type) {
      lock (storableTypeCache) {
        if (storableTypeCache.ContainsKey(type))
          return storableTypeCache[type];
        StorableTypeAttribute attribute = type
          .GetCustomAttributes(typeof(StorableTypeAttribute), false)
          .SingleOrDefault() as StorableTypeAttribute;
        storableTypeCache.Add(type, attribute);
        return attribute;
      }
    }

    public static bool HasStorableTypeAttribute(Type type) {
      return GetStorableTypeAttribute(type) != null;
    }

    private static Dictionary<Type, StorableTypeAttribute> storableTypeCache =
      new Dictionary<Type, StorableTypeAttribute>();

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
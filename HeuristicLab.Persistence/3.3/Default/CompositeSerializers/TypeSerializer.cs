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
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HEAL.Attic;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableType("0CF9A082-629F-4554-8912-CC37228175E2")]
  internal sealed class TypeSerializer : ICompositeSerializer {

    [StorableConstructor]
    private TypeSerializer(StorableConstructorFlag _) { }
    public TypeSerializer() { }

    public int Priority {
      get { return 100; }
    }

    public bool CanSerialize(Type type) {
      #region Mono Compatibility
      return type == typeof(Type) ||
        type.VersionInvariantName() == "System.RuntimeType, mscorlib" ||
        type.VersionInvariantName() == "System.MonoType, mscorlib";
      #endregion
    }

    public string JustifyRejection(Type type) {
      #region Mono Compatibility
      return "not System.Type, System.RuntimeType, System.MonoType";
      #endregion
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      yield return new Tag("AssemblyQualifiedName", ((Type)o).AssemblyQualifiedName);
    }

    public IEnumerable<Tag> Decompose(object obj) {
      return new Tag[] { };
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      IEnumerator<Tag> it = metaInfo.GetEnumerator();
      try {
        it.MoveNext();
      }
      catch (InvalidOperationException e) {
        throw new PersistenceException("Insufficient meta information to instantiate Type object", e);
      }
      try {
        return TypeLoader.Load((string)it.Current.Value);
      }
      catch (InvalidCastException e) {
        throw new PersistenceException("Invalid meta information during reconstruction of Type object", e);
      }
      catch (TypeLoadException e) {
        throw new PersistenceException(String.Format(
          "Cannot load Type {0}, make sure all required assemblies are available.",
          (string)it.Current.Value), e);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> objects, Type type) {
      // Type ojects are populated during instance creation.
    }
  }
}

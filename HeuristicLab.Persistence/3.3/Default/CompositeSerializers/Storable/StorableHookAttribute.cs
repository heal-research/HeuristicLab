#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    AfterDeserialization
  };


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
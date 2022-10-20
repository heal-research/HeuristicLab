#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.PluginInfrastructure {
  [Serializable]
  public abstract class CommandLineArgument<T> : ICommandLineArgument<T> {
    object ICommandLineArgument.Value { get { return Value; } }
    public T Value { get; private set; }
    public bool Valid { get { return CheckValidity(); } }

    protected CommandLineArgument(T value) {
      Value = value;
    }

    public override bool Equals(object obj) {
      if (obj == null || this.GetType() != obj.GetType()) return false;
      var other = (ICommandLineArgument<T>)obj;
      return this.Value.Equals(other.Value);
    }

    public override int GetHashCode() {
      return GetType().GetHashCode() ^ Value.GetHashCode();
    }

    protected abstract bool CheckValidity();
  }
}

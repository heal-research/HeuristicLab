#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.PluginInfrastructure {
  public abstract class Argument : IArgument {
    public string Value { get; private set; }
    public abstract bool Valid { get; }

    protected Argument(string value) {
      this.Value = value;
    }

    public override bool Equals(object obj) {
      if (obj == null || this.GetType() != obj.GetType()) return false;
      return this.Value == ((Argument)obj).Value;
    }

    public override int GetHashCode() {
      return GetType().GetHashCode();
    }
  }

  public class StartArgument : Argument {
    public override bool Valid {
      get { return !string.IsNullOrEmpty(Value); }
    }

    public StartArgument(string value) : base(value) { }
  }
}

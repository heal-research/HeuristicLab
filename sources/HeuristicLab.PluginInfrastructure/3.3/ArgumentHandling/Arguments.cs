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
  public class Argument : IArgument {
    public const string START = "start";

    public string Token { get; private set; }
    public string Value { get; private set; }
    public bool Valid { get { return !string.IsNullOrEmpty(Value); } }

    public Argument(string token, string value) {
      Token = token;
      Value = value;
    }

    public override bool Equals(object obj) {
      if (obj == null || this.GetType() != obj.GetType()) return false;
      var other = (Argument)obj;
      return this.Token == other.Token && this.Value == other.Value;
    }

    public override int GetHashCode() {
      return GetType().GetHashCode();
    }
  }
}

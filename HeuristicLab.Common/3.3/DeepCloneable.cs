#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Common {
  /// <summary>
  /// Represents a base class for all deeply cloneable objects.
  /// </summary>
  public abstract class DeepCloneable : IDeepCloneable {
    protected DeepCloneable(DeepCloneable original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
    }
    protected DeepCloneable() { }

    /// <summary>
    /// Creates a deep clone of this instance.
    /// </summary>
    /// <remarks>
    /// This method is the entry point for creating a deep clone of a whole object graph.
    /// </remarks>
    /// <returns>A clone of this instance.</returns>
    public object Clone() {
      return Clone(new Cloner());
    }

    /// <summary>
    /// Creates a deep clone of this instance.
    /// </summary>
    /// <remarks>This method should not be called directly. It is used for creating clones of
    /// objects which are contained in the object that is currently cloned.</remarks>
    /// <param name="cloner">The cloner which is responsible for keeping track of all already
    /// cloned objects.</param>
    /// <returns>A clone of this instance.</returns>
    public abstract IDeepCloneable Clone(Cloner cloner);
  }
}

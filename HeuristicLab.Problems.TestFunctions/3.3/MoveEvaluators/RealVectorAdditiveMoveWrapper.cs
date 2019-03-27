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

using HeuristicLab.Common;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// This wrapper disguises as real vector for use in the evaluation functions.
  /// </summary>
  [NonDiscoverableType]
  internal sealed class RealVectorAdditiveMoveWrapper : RealVector {
    private int dimension;
    private double moveDistance;
    private RealVector vector;

    private RealVectorAdditiveMoveWrapper(RealVectorAdditiveMoveWrapper original, Cloner cloner)
      : base(original, cloner) {
      this.dimension = original.dimension;
      this.moveDistance = original.moveDistance;
      this.vector = cloner.Clone(vector);
    }
    public RealVectorAdditiveMoveWrapper(AdditiveMove move, RealVector vector) {
      dimension = move.Dimension;
      moveDistance = move.MoveDistance;
      this.vector = vector;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorAdditiveMoveWrapper(this, cloner);
    }

    public override double this[int index] {
      get {
        if (index != dimension)
          return vector[index];
        else return vector[index] + moveDistance;
      }
      set {
        throw new System.NotSupportedException("Error: Writing to the wrapper is not allowed.");
      }
    }

    public override int Length {
      get {
        return vector.Length;
      }
      #region Mono Compatibility
      // this setter should be protected, but the Mono compiler couldn't handle it
      set {
        throw new System.NotSupportedException("Error: Setting the lenght of the wrapper is not allowed.");
      }
      #endregion
    }
  }
}

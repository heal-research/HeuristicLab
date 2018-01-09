#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// An operator that relinks paths between test functions solutions.
  /// </summary>
  /// <remarks>
  /// It is based on an implementation described in Duarte, A., Martí, R., and Gortazar, F. (2011). Path Relinking for Large Scale Global Optimization. Soft Computing, Vol. 15.<br />
  /// The operator incrementally assimilates the initiating solution into the guiding solution by adapting the solution vector's elements.
  /// </remarks>
  [Item("SingleObjectiveTestFunctionPathRelinker", "An operator that relinks paths between test functions solutions. It is based on an implementation described in Duarte, A., Martí, R., and Gortazar, F. (2011). Path Relinking for Large Scale Global Optimization. Soft Computing, Vol. 15.")]
  [StorableClass]
  public sealed class SingleObjectiveTestFunctionPathRelinker : SingleObjectivePathRelinker {
    #region Parameter properties
    public IValueParameter<IntValue> RelinkingIntensityParameter {
      get { return (IValueParameter<IntValue>)Parameters["RelinkingIntensity"]; }
    }
    #endregion

    #region Properties
    private IntValue RelinkingIntensity {
      get { return RelinkingIntensityParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    private SingleObjectiveTestFunctionPathRelinker(bool deserializing) : base(deserializing) { }
    private SingleObjectiveTestFunctionPathRelinker(SingleObjectiveTestFunctionPathRelinker original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveTestFunctionPathRelinker()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueParameter<IntValue>("RelinkingIntensity", "Determines how strong path relinking should be applied.", new IntValue(10)));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveTestFunctionPathRelinker(this, cloner);
    }

    public static ItemArray<IItem> Apply(IItem initiator, IItem guide, IntValue k, PercentValue n) {
      if (!(initiator is RealVector) || !(guide is RealVector))
        throw new ArgumentException("Cannot relink path because one of the provided solutions or both have the wrong type.");
      if (n.Value <= 0.0)
        throw new ArgumentException("RelinkingAccuracy must be greater than 0.");

      RealVector v1 = initiator.Clone() as RealVector;
      RealVector v2 = guide as RealVector;

      if (v1.Length != v2.Length)
        throw new ArgumentException("The solutions are of different length.");

      IList<RealVector> solutions = new List<RealVector>();
      for (int i = 0; i < k.Value; i++) {
        RealVector solution = v1.Clone() as RealVector;
        for (int j = 0; j < solution.Length; j++)
          solution[j] = v1[j] + 1 / (k.Value - i) * (v2[j] - v1[j]);
        solutions.Add(solution);
      }

      IList<IItem> selection = new List<IItem>();
      if (solutions.Count > 0) {
        int noSol = (int)(solutions.Count * n.Value);
        if (noSol <= 0) noSol++;
        double stepSize = (double)solutions.Count / (double)noSol;
        for (int i = 0; i < noSol; i++)
          selection.Add(solutions.ElementAt((int)((i + 1) * stepSize - stepSize * 0.5)));
      }

      return new ItemArray<IItem>(selection);
    }

    protected override ItemArray<IItem> Relink(ItemArray<IItem> parents, PercentValue n) {
      if (parents.Length != 2)
        throw new ArgumentException("The number of parents is not equal to 2.");
      return Apply(parents[0], parents[1], RelinkingIntensity, n);
    }
  }
}

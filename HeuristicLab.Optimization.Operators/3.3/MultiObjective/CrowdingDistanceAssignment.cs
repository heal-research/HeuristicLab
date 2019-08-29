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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {
  [Item("CrowdingDistanceAssignment", "Calculates the crowding distances for each sub-scope as described in Deb et al. 2002. A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), pp. 182-197.")]
  [StorableType("F7DF8B74-F1E6-45D6-A1A8-5D381F20B382")]
  public class CrowdingDistanceAssignment : SingleSuccessorOperator, IMultiObjectiveOperator {
    public ScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (ScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> CrowdingDistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["CrowdingDistance"]; }
    }

    private void QualitiesParameter_DepthChanged(object sender, EventArgs e) {
      CrowdingDistanceParameter.Depth = QualitiesParameter.Depth;
    }

    [StorableConstructor]
    protected CrowdingDistanceAssignment(StorableConstructorFlag _) : base(_) { }
    protected CrowdingDistanceAssignment(CrowdingDistanceAssignment original, Cloner cloner) : base(original, cloner) { }
    public CrowdingDistanceAssignment() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The vector of quality values."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("CrowdingDistance", "Sets the crowding distance in each sub-scope."));
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      QualitiesParameter.DepthChanged += new EventHandler(QualitiesParameter_DepthChanged);
    }

    public static void Apply(DoubleArray[] qualities, DoubleValue[] distances) {
      var dist = CrowdingCalculator.CalculateCrowdingDistances(qualities.Select(x => x.ToArray()).ToArray());
      for (var i = 0; i < distances.Length; i++) distances[i].Value = dist[i];
    }

    public override IOperation Apply() {
      var dist = CrowdingCalculator.CalculateCrowdingDistances(QualitiesParameter.ActualValue.Select(x => x.ToArray()).ToArray());
      CrowdingDistanceParameter.ActualValue = new ItemArray<DoubleValue>(dist.Select(d => new DoubleValue(d)));
      return base.Apply();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdingDistanceAssignment(this, cloner);
    }
  }
}
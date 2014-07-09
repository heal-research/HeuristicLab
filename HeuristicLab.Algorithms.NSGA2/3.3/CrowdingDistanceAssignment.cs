#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Algorithms.NSGA2 {
  [Item("CrowdingDistanceAssignment", "Calculates the crowding distances for each sub-scope as described in Deb et al. 2002. A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), pp. 182-197.")]
  [StorableClass]
  public class CrowdingDistanceAssignment : SingleSuccessorOperator {

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
    protected CrowdingDistanceAssignment(bool deserializing) : base(deserializing) { }
    protected CrowdingDistanceAssignment(CrowdingDistanceAssignment original, Cloner cloner) : base(original, cloner) { }
    public CrowdingDistanceAssignment() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The vector of quality values."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("CrowdingDistance", "Sets the crowding distance in each sub-scope."));
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AttachEventHandlers() {
      QualitiesParameter.DepthChanged += new EventHandler(QualitiesParameter_DepthChanged);
    }

    public static void Apply(DoubleArray[] qualities, DoubleValue[] distances) {
      int populationSize = qualities.Length;
      int objectiveCount = qualities[0].Length;
      for (int m = 0; m < objectiveCount; m++) {
        Array.Sort<DoubleArray, DoubleValue>(qualities, distances, new QualitiesComparer(m));

        distances[0].Value = double.MaxValue;
        distances[populationSize - 1].Value = double.MaxValue;

        double minQuality = qualities[0][m];
        double maxQuality = qualities[populationSize - 1][m];
        for (int i = 1; i < populationSize - 1; i++) {
          distances[i].Value += (qualities[i + 1][m] - qualities[i - 1][m]) / (maxQuality - minQuality);
        }
      }
    }

    public override IOperation Apply() {
      DoubleArray[] qualities = QualitiesParameter.ActualValue.ToArray();
      int populationSize = qualities.Length;
      DoubleValue[] distances = new DoubleValue[populationSize];
      for (int i = 0; i < populationSize; i++)
        distances[i] = new DoubleValue(0);

      CrowdingDistanceParameter.ActualValue = new ItemArray<DoubleValue>(distances);
      
      Apply(qualities, distances);

      return base.Apply();
    }

    private void Initialize(ItemArray<DoubleValue> distances) {
      for (int i = 0; i < distances.Length; i++) {
        if (distances[i] == null) distances[i] = new DoubleValue(0);
        else distances[i].Value = 0;
      }
    }

    private class QualitiesComparer : IComparer<DoubleArray> {
      private int index;

      public QualitiesComparer(int index) {
        this.index = index;
      }

      #region IComparer<DoubleArray> Members

      public int Compare(DoubleArray x, DoubleArray y) {
        if (x[index] < y[index]) return -1;
        else if (x[index] > y[index]) return +1;
        else return 0;
      }

      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdingDistanceAssignment(this, cloner);
    }
  }
}

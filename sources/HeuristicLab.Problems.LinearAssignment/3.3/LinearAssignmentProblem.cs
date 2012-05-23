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

using System;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LinearAssignment {
  [Item("LinearAssignmentProblem", "In the linear assignment problem (LAP) an assignment of workers to jobs has to be found such that each worker is assigned to exactly one job, each job is assigned to exactly one worker and the sum of the resulting costs needs to be minimal.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class LinearAssignmentProblem : Problem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public IValueParameter<DoubleMatrix> CostsParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters["Costs"]; }
    }
    public IValueParameter<IntArray> SolutionParameter {
      get { return (IValueParameter<IntArray>)Parameters["Solution"]; }
    }
    public IValueParameter<DoubleValue> QualityParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Quality"]; }
    }
    #endregion

    #region Properties
    public DoubleMatrix Costs {
      get { return CostsParameter.Value; }
      set { CostsParameter.Value = value; }
    }
    public IntArray Solution {
      get { return SolutionParameter.Value; }
      set { SolutionParameter.Value = value; }
    }
    public DoubleValue Quality {
      get { return QualityParameter.Value; }
      set { QualityParameter.Value = value; }
    }

    #endregion

    [StorableConstructor]
    private LinearAssignmentProblem(bool deserializing) : base(deserializing) { }
    private LinearAssignmentProblem(LinearAssignmentProblem original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public LinearAssignmentProblem()
      : base() {
      Parameters.Add(new ValueParameter<DoubleMatrix>("Costs", "The cost matrix that describes the assignment of rows to columns.", new DoubleMatrix(3, 3)));
      Parameters.Add(new OptionalValueParameter<IntArray>("Solution", "An optimal solution.", null));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("Quality", "The solution quality.", null));

      ((ValueParameter<DoubleMatrix>)CostsParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearAssignmentProblem(this, cloner);
    }

    #region Events
    private void Costs_RowsChanged(object sender, EventArgs e) {
      if (Costs.Rows != Costs.Columns)
        ((IStringConvertibleMatrix)Costs).Columns = Costs.Rows;
    }
    private void Costs_ColumnsChanged(object sender, EventArgs e) {
      if (Costs.Rows != Costs.Columns)
        ((IStringConvertibleMatrix)Costs).Rows = Costs.Columns;
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      Costs.RowsChanged += new EventHandler(Costs_RowsChanged);
      Costs.ColumnsChanged += new EventHandler(Costs_ColumnsChanged);
    }
    #endregion
  }
}

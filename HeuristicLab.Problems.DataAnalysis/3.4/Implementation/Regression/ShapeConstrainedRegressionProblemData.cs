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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("8D44EABE-2D52-4501-B62D-5E28FB4CFEAE")]
  [Item("ShapeConstrainedProblemData", "Represents an item containing all data defining a regression problem with shape constraints.")]
  public class ShapeConstrainedRegressionProblemData : RegressionProblemData, IShapeConstrainedRegressionProblemData {
    protected const string ShapeConstraintsParameterName = "ShapeConstraints";

    #region default data
    private static double[,] sigmoid = new double[,] {
      {1.00, 0.09, 0.01390952},
      {1.10, 0.11, 0.048256016},
      {1.20, 0.14, 0.010182641},
      {1.30, 0.17, 0.270361269},
      {1.40, 0.20, 0.091503971},
      {1.50, 0.24, 0.338157191},
      {1.60, 0.28, 0.328508579},
      {1.70, 0.34, 0.21867684},
      {1.80, 0.40, 0.34515433},
      {1.90, 0.46, 0.562746903},
      {2.00, 0.54, 0.554800831},
      {2.10, 0.62, 0.623018787},
      {2.20, 0.71, 0.626224329},
      {2.30, 0.80, 0.909006688},
      {2.40, 0.90, 0.92514929},
      {2.50, 1.00, 1.097199936},
      {2.60, 1.10, 1.138309608},
      {2.70, 1.20, 1.087880692},
      {2.80, 1.29, 1.370491683},
      {2.90, 1.38, 1.422048792},
      {3.00, 1.46, 1.505242141},
      {3.10, 1.54, 1.684790135},
      {3.20, 1.60, 1.480232277},
      {3.30, 1.66, 1.577412501},
      {3.40, 1.72, 1.664822534},
      {3.50, 1.76, 1.773580664},
      {3.60, 1.80, 1.941034478},
      {3.70, 1.83, 1.730361986},
      {3.80, 1.86, 1.9785952},
      {3.90, 1.89, 1.946698641},
      {4.00, 1.91, 1.766502803},
      {4.10, 1.92, 1.847756843},
      {4.20, 1.94, 1.894506213},
      {4.30, 1.95, 2.029194724},
      {4.40, 1.96, 2.01830679},
      {4.50, 1.96, 1.924316332},
      {4.60, 1.97, 1.971354792},
      {4.70, 1.98, 1.85665728},
      {4.80, 1.98, 1.831400496},
      {4.90, 1.98, 2.057843156},
      {5.00, 1.99, 2.128769896},
    };

    private static readonly Dataset defaultDataset;
    private static readonly IEnumerable<string> defaultAllowedInputVariables;
    private static readonly string defaultTargetVariable;
    private static readonly ShapeConstraints defaultShapeConstraints;
    private static readonly IntervalCollection defaultVariableRanges;

    private static readonly ShapeConstrainedRegressionProblemData emptyProblemData;
    public new static ShapeConstrainedRegressionProblemData EmptyProblemData => emptyProblemData;

    static ShapeConstrainedRegressionProblemData() {
      defaultDataset = new Dataset(new string[] { "x", "y", "y_noise" }, sigmoid) {
        Name = "Sigmoid function for shape-constrained symbolic regression.",
        Description = "f(x) = 1 + tanh(x - 2.5)"
      };
      defaultAllowedInputVariables = new List<string>() { "x" };
      defaultTargetVariable = "y_noise";
      defaultShapeConstraints = new ShapeConstraints {
        new ShapeConstraint(new Interval(0, 2), 1.0),
        new ShapeConstraint("x", 1, new Interval(0, double.PositiveInfinity), 1.0)
      };
      defaultVariableRanges = defaultDataset.GetVariableRanges();
      defaultVariableRanges.SetInterval("x", new Interval(0, 6));

      var problemData = new ShapeConstrainedRegressionProblemData();
      problemData.Parameters.Clear();
      problemData.Name = "Empty Regression ProblemData";
      problemData.Description = "This ProblemData acts as place holder before the correct problem data is loaded.";
      problemData.isEmpty = true;

      problemData.Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", new Dataset()));
      problemData.Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputVariablesParameterName, ""));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", (IntRange)new IntRange(0, 20).AsReadOnly()));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", (IntRange)new IntRange(20, 40).AsReadOnly()));
      problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(TargetVariableParameterName, new ItemSet<StringValue>()));
      problemData.Parameters.Add(new FixedValueParameter<IntervalCollection>(VariableRangesParameterName, "", new IntervalCollection()));
      problemData.Parameters.Add(new FixedValueParameter<ShapeConstraints>(ShapeConstraintsParameterName, "", new ShapeConstraints()));
      emptyProblemData = problemData;
    }
    #endregion

    public IFixedValueParameter<ShapeConstraints> ShapeConstraintParameter => (IFixedValueParameter<ShapeConstraints>)Parameters[ShapeConstraintsParameterName];
    public ShapeConstraints ShapeConstraints => ShapeConstraintParameter.Value;

    [StorableConstructor]
    protected ShapeConstrainedRegressionProblemData(StorableConstructorFlag _) : base(_) { }

    protected ShapeConstrainedRegressionProblemData(ShapeConstrainedRegressionProblemData original, Cloner cloner) : base(original, cloner) {
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return this == emptyProblemData ? emptyProblemData : new ShapeConstrainedRegressionProblemData(this, cloner);
    }

    public ShapeConstrainedRegressionProblemData() : this(defaultDataset, defaultAllowedInputVariables, defaultTargetVariable,
                                                          trainingPartition: new IntRange(0, defaultDataset.Rows),
                                                          testPartition: new IntRange(0, 0), sc: defaultShapeConstraints, variableRanges: defaultVariableRanges) { } // no test partition for the demo problem

    public ShapeConstrainedRegressionProblemData(IRegressionProblemData regressionProblemData)
      : this(regressionProblemData.Dataset, regressionProblemData.AllowedInputVariables, regressionProblemData.TargetVariable,
          regressionProblemData.TrainingPartition, regressionProblemData.TestPartition, regressionProblemData.Transformations,
          (regressionProblemData is ShapeConstrainedRegressionProblemData) ? ((ShapeConstrainedRegressionProblemData)regressionProblemData).ShapeConstraints : null, 
          regressionProblemData.VariableRanges) {
    }

    public ShapeConstrainedRegressionProblemData(IDataset dataset, IEnumerable<string> allowedInputVariables, string targetVariable,
                                                 IntRange trainingPartition, IntRange testPartition,
                                                 IEnumerable<ITransformation> transformations = null, ShapeConstraints sc = null, IntervalCollection variableRanges = null)
    : base(dataset, allowedInputVariables, targetVariable, transformations ?? Enumerable.Empty<ITransformation>()) {
      TrainingPartition.Start = trainingPartition.Start;
      TrainingPartition.End = trainingPartition.End;
      TestPartition.Start = testPartition.Start;
      TestPartition.End = testPartition.End;
      if (sc == null) sc = new ShapeConstraints();
      Parameters.Add(new FixedValueParameter<ShapeConstraints>(ShapeConstraintsParameterName, "Specifies the shape constraints for the regression problem.", (ShapeConstraints)sc.Clone()));
      foreach (var entry in variableRanges.GetVariableIntervals())
        VariableRanges.SetInterval(entry.Item1, entry.Item2);
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      ShapeConstraints.Changed += ShapeConstraints_Changed;
      ShapeConstraints.CheckedItemsChanged += ShapeConstraints_Changed;
      ShapeConstraints.CollectionReset += ShapeConstraints_Changed;
      ShapeConstraints.ItemsAdded += ShapeConstraints_Changed;
      ShapeConstraints.ItemsRemoved += ShapeConstraints_Changed;
      ShapeConstraints.ItemsMoved += ShapeConstraints_Changed;
      ShapeConstraints.ItemsReplaced += ShapeConstraints_Changed;
    }

    private void ShapeConstraints_Changed(object sender, EventArgs e) {
      OnChanged();
    }
  }
}

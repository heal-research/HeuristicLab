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

    private static double[,] kozaF1 = new double[,] {
          {2.017885919, -1.449165046},
          {1.30060506,  -1.344523885},
          {1.147134798, -1.317989331},
          {0.877182504, -1.266142284},
          {0.852562452, -1.261020794},
          {0.431095788, -1.158793317},
          {0.112586002, -1.050908405},
          {0.04594507,  -1.021989402},
          {0.042572879, -1.020438113},
          {-0.074027291,  -0.959859562},
          {-0.109178553,  -0.938094706},
          {-0.259721109,  -0.803635355},
          {-0.272991057,  -0.387519561},
          {-0.161978191,  -0.193611001},
          {-0.102489983,  -0.114215349},
          {-0.01469968, -0.014918985},
          {-0.008863365,  -0.008942626},
          {0.026751057, 0.026054094},
          {0.166922436, 0.14309643},
          {0.176953808, 0.1504144},
          {0.190233418, 0.159916534},
          {0.199800708, 0.166635331},
          {0.261502822, 0.207600348},
          {0.30182879,  0.232370249},
          {0.83763905,  0.468046718}
    };

    private static readonly Dataset defaultDataset;
    private static readonly IEnumerable<string> defaultAllowedInputVariables;
    private static readonly string defaultTargetVariable;

    private static readonly ShapeConstrainedRegressionProblemData emptyProblemData;
    public new static ShapeConstrainedRegressionProblemData EmptyProblemData {
      get { return emptyProblemData; }
    }

    static ShapeConstrainedRegressionProblemData() {
      defaultDataset = new Dataset(new string[] { "y", "x" }, kozaF1);
      defaultDataset.Name = "Fourth-order Polynomial Function Benchmark Dataset";
      defaultDataset.Description = "f(x) = x^4 + x^3 + x^2 + x^1";
      defaultAllowedInputVariables = new List<string>() { "x" };
      defaultTargetVariable = "y";
      var problemData = new ShapeConstrainedRegressionProblemData();
      problemData.Parameters.Clear(); // parameters are cleared and added below because we cannot set / change parameters of ProblemData
      problemData.Name = "Empty Regression ProblemData";
      problemData.Description = "This ProblemData acts as place holder before the correct problem data is loaded.";
      problemData.isEmpty = true;

      problemData.Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", new Dataset()));
      problemData.Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputVariablesParameterName, ""));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(TargetVariableParameterName, new ItemSet<StringValue>()));
      problemData.Parameters.Add(new FixedValueParameter<IntervalCollection>(VariableRangesParameterName, "", new IntervalCollection()));
      problemData.Parameters.Add(new FixedValueParameter<ShapeConstraints>(ShapeConstraintsParameterName, "", new ShapeConstraints()));
      emptyProblemData = problemData;
    }

    public IFixedValueParameter<ShapeConstraints> ShapeConstraintsParameter => (IFixedValueParameter<ShapeConstraints>)Parameters[ShapeConstraintsParameterName];
    public ShapeConstraints ShapeConstraints => ShapeConstraintsParameter.Value;

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
      if (this == emptyProblemData) return emptyProblemData;
      return new ShapeConstrainedRegressionProblemData(this, cloner);
    }

    public ShapeConstrainedRegressionProblemData() : this(defaultDataset, defaultAllowedInputVariables, defaultTargetVariable,
                                                          trainingPartition: new IntRange(0, defaultDataset.Rows),
                                                          testPartition: new IntRange(0, 0)) { } // no test partition for the demo problem

    public ShapeConstrainedRegressionProblemData(IRegressionProblemData regressionProblemData)
      : this(regressionProblemData.Dataset, regressionProblemData.AllowedInputVariables, regressionProblemData.TargetVariable,
          regressionProblemData.TrainingPartition, regressionProblemData.TestPartition, regressionProblemData.Transformations, null, regressionProblemData.VariableRanges) {
    }

    public ShapeConstrainedRegressionProblemData(IDataset dataset, IEnumerable<string> allowedInputVariables, string targetVariable,
                                                 IntRange trainingPartition, IntRange testPartition,
                                                 IEnumerable<ITransformation> transformations = null, ShapeConstraints sc = null, IntervalCollection variableRanges = null)
    : base(dataset, allowedInputVariables, targetVariable, transformations ?? Enumerable.Empty<ITransformation>(), variableRanges) {
      TrainingPartition.Start = trainingPartition.Start;
      TrainingPartition.End = trainingPartition.End;
      TestPartition.Start = testPartition.Start;
      TestPartition.End = testPartition.End;
      if (sc == null) sc = new ShapeConstraints();
      Parameters.Add(new FixedValueParameter<ShapeConstraints>(ShapeConstraintsParameterName, "Specifies the shape constraints for the regression problem.", (ShapeConstraints)sc.Clone()));

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

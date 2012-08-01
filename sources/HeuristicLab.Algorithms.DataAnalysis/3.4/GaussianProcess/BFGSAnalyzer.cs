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

using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "BFGSAnalyzer", Description = "Analyzer to collect results for the BFGS algorithm.")]
  public sealed class BFGSAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string PointParameterName = "Point";
    private const string QualityGradientsParameterName = "QualityGradients";
    private const string QualityParameterName = "Quality";
    private const string ResultCollectionParameterName = "Results";
    private const string QualitiesTableParameterName = "Qualities";
    private const string PointsTableParameterName = "PointTable";
    private const string QualityGradientsTableParameterName = "QualityGradientsTable";
    private const string BFGSStateParameterName = "BFGSState";

    #region Parameter Properties
    public ILookupParameter<DoubleArray> QualityGradientsParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[QualityGradientsParameterName]; }
    }
    public ILookupParameter<DoubleArray> PointParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[PointParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultCollectionParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultCollectionParameterName]; }
    }
    public ILookupParameter<DataTable> QualitiesTableParameter {
      get { return (ILookupParameter<DataTable>)Parameters[QualitiesTableParameterName]; }
    }
    public ILookupParameter<DataTable> PointsTableParameter {
      get { return (ILookupParameter<DataTable>)Parameters[PointsTableParameterName]; }
    }
    public ILookupParameter<DataTable> QualityGradientsTableParameter {
      get { return (ILookupParameter<DataTable>)Parameters[QualityGradientsTableParameterName]; }
    }
    public ILookupParameter<BFGSState> BFGSStateParameter {
      get { return (ILookupParameter<BFGSState>)Parameters[BFGSStateParameterName]; }
    }
    #endregion

    #region Properties
    private DoubleArray QualityGradients { get { return QualityGradientsParameter.ActualValue; } }
    private DoubleArray Point { get { return PointParameter.ActualValue; } }
    private DoubleValue Quality { get { return QualityParameter.ActualValue; } }
    private ResultCollection ResultCollection { get { return ResultCollectionParameter.ActualValue; } }

    public bool EnabledByDefault {
      get { return true; }
    }

    #endregion

    [StorableConstructor]
    private BFGSAnalyzer(bool deserializing) : base(deserializing) { }
    private BFGSAnalyzer(BFGSAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BFGSAnalyzer()
      : base() {
      // in
      Parameters.Add(new LookupParameter<DoubleArray>(PointParameterName, "The current point of the function to optimize."));
      Parameters.Add(new LookupParameter<DoubleArray>(QualityGradientsParameterName, "The current gradients of the function to optimize."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The current value of the function to optimize."));
      Parameters.Add(new LookupParameter<DataTable>(QualitiesTableParameterName, "The table of all visited quality values."));
      Parameters.Add(new LookupParameter<DataTable>(PointsTableParameterName, "The table of all visited points."));
      Parameters.Add(new LookupParameter<DataTable>(QualityGradientsTableParameterName, "The table of all visited gradient values."));
      Parameters.Add(new LookupParameter<BFGSState>(BFGSStateParameterName, "The state of the BFGS optimization algorithm."));
      // in & out
      Parameters.Add(new LookupParameter<ResultCollection>(ResultCollectionParameterName, "The result collection of the algorithm."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BFGSAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      if (BFGSStateParameter.ActualValue.State.xupdated) {
        var f = Quality.Value;
        var g = QualityGradients.ToArray();
        var x = Point.ToArray();
        var resultCollection = ResultCollection;

        // create and add tables on the first time
        if (QualitiesTableParameter.ActualValue == null) {
          QualitiesTableParameter.ActualValue = new DataTable(QualityParameter.ActualName);
          PointsTableParameter.ActualValue = new DataTable(PointParameter.ActualName);
          QualityGradientsTableParameter.ActualValue = new DataTable(QualityGradientsParameter.ActualName);

          QualitiesTableParameter.ActualValue.Rows.Add(new DataRow(QualityParameter.ActualName));

          resultCollection.Add(new Result(QualitiesTableParameter.ActualName,
                                          QualitiesTableParameter.ActualValue));
          resultCollection.Add(new Result(PointsTableParameter.ActualName,
                                          PointsTableParameter.ActualValue));
          resultCollection.Add(new Result(QualityGradientsTableParameter.ActualName,
                                          QualityGradientsTableParameter.ActualValue));
          resultCollection.Add(new Result(QualityParameter.ActualName, QualityParameter.ActualValue));
        }

        // update
        var functionValueRow = QualitiesTableParameter.ActualValue.Rows[QualityParameter.ActualName];
        resultCollection[QualityParameter.ActualName].Value = Quality;
        functionValueRow.Values.Add(f);

        AddValues(g, QualityGradientsTableParameter.ActualValue);
        AddValues(x, PointsTableParameter.ActualValue);
      }
      return base.Apply();
    }

    private void AddValues(double[] x, DataTable dataTable) {
      if (!dataTable.Rows.Any()) {
        for (int i = 0; i < x.Length; i++) {
          var newRow = new DataRow("x" + i);
          newRow.Values.Add(x[i]);
          dataTable.Rows.Add(newRow);
        }
      } else {
        for (int i = 0; i < x.Length; i++) {
          dataTable.Rows.ElementAt(i).Values.Add(x[i]);
        }
      }
    }
  }
}

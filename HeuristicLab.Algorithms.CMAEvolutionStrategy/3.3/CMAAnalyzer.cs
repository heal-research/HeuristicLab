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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("CMAAnalyzer", "Analyzes the development of strategy parameters and visualizes the performance of CMA-ES.")]
  [StorableClass]
  public sealed class CMAAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {

    public bool EnabledByDefault {
      get { return false; }
    }

    #region Parameter Properties
    public ILookupParameter<CMAParameters> StrategyParametersParameter {
      get { return (ILookupParameter<CMAParameters>)Parameters["StrategyParameters"]; }
    }

    public ILookupParameter<RealVector> MeanParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Mean"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    #endregion

    [StorableConstructor]
    private CMAAnalyzer(bool deserializing) : base(deserializing) { }
    private CMAAnalyzer(CMAAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public CMAAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<CMAParameters>("StrategyParameters", "The CMA strategy parameters to be analyzed."));
      Parameters.Add(new LookupParameter<RealVector>("Mean", "The mean real vector that is being optimized."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The collection to store the results in."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMAAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var sp = StrategyParametersParameter.ActualValue;
      var vector = MeanParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;
      double min = qualities[0].Value, max = qualities[0].Value, avg = qualities[0].Value;
      for (int i = 1; i < qualities.Length; i++) {
        if (qualities[i].Value < min) min = qualities[i].Value;
        if (qualities[i].Value > max) max = qualities[i].Value;
        avg += qualities[i].Value;
      }
      avg /= qualities.Length;

      DataTable progress;
      if (results.ContainsKey("Progress")) {
        progress = (DataTable)results["Progress"].Value;
      } else {
        progress = new DataTable("Progress");
        progress.Rows.Add(new DataRow("AxisRatio"));
        progress.Rows.Add(new DataRow("Sigma"));
        progress.Rows.Add(new DataRow("Min Quality"));
        progress.Rows.Add(new DataRow("Max Quality"));
        progress.Rows.Add(new DataRow("Avg Quality"));
        progress.VisualProperties.YAxisLogScale = true;
        results.Add(new Result("Progress", progress));
      }
      progress.Rows["AxisRatio"].Values.Add(sp.AxisRatio);
      progress.Rows["Sigma"].Values.Add(sp.Sigma);
      progress.Rows["Min Quality"].Values.Add(min);
      progress.Rows["Max Quality"].Values.Add(max);
      progress.Rows["Avg Quality"].Values.Add(avg);

      DataTable scaling;
      if (results.ContainsKey("Scaling")) {
        scaling = (DataTable)results["Scaling"].Value;
      } else {
        scaling = new DataTable("Scaling");
        scaling.VisualProperties.YAxisLogScale = true;
        for (int i = 0; i < sp.C.GetLength(0); i++)
          scaling.Rows.Add(new DataRow("Axis" + i.ToString()));
        results.Add(new Result("Scaling", scaling));
      }
      for (int i = 0; i < sp.C.GetLength(0); i++)
        scaling.Rows["Axis" + i.ToString()].Values.Add(sp.D[i]);

      DataTable realVector;
      if (results.ContainsKey("Object Variables")) {
        realVector = (DataTable)results["Object Variables"].Value;
      } else {
        realVector = new DataTable("Object Variables");
        for (int i = 0; i < vector.Length; i++)
          realVector.Rows.Add(new DataRow("Axis" + i.ToString()));
        results.Add(new Result("Object Variables", realVector));
      }
      for (int i = 0; i < vector.Length; i++)
        realVector.Rows["Axis" + i.ToString()].Values.Add(vector[i]);

      DataTable stdDevs;
      if (results.ContainsKey("Standard Deviations")) {
        stdDevs = (DataTable)results["Standard Deviations"].Value;
      } else {
        stdDevs = new DataTable("Standard Deviations");
        stdDevs.VisualProperties.YAxisLogScale = true;
        stdDevs.Rows.Add(new DataRow("MinStdDev"));
        stdDevs.Rows.Add(new DataRow("MaxStdDev"));
        for (int i = 0; i < vector.Length; i++)
          stdDevs.Rows.Add(new DataRow("Axis" + i.ToString()));
        results.Add(new Result("Standard Deviations", stdDevs));
      }
      for (int i = 0; i < vector.Length; i++)
        stdDevs.Rows["Axis" + i.ToString()].Values.Add(Math.Sqrt(sp.C[i, i]));
      stdDevs.Rows["MinStdDev"].Values.Add(sp.D.Min() * sp.Sigma);
      stdDevs.Rows["MaxStdDev"].Values.Add(sp.D.Max() * sp.Sigma);

      return base.Apply();
    }
  }
}
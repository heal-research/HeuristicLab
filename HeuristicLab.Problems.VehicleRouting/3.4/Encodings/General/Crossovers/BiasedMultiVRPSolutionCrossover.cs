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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("BiasedMultiVRPSolutionCrossover", "Randomly selects and applies one of its crossovers every time it is called based on the success progress.")]
  [StorableClass]
  public class BiasedMultiVRPSolutionCrossover : MultiVRPSolutionCrossover {
    public ValueLookupParameter<DoubleArray> ActualProbabilitiesParameter {
      get { return (ValueLookupParameter<DoubleArray>)Parameters["ActualProbabilities"]; }
    }

    public ValueLookupParameter<StringValue> SuccessProgressAnalyisis {
      get { return (ValueLookupParameter<StringValue>)Parameters["SuccessProgressAnalysis"]; }
    }

    public ValueLookupParameter<DoubleValue> Factor {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Factor"]; }
    }

    public ValueParameter<DoubleValue> LowerBoundParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["LowerBound"]; }
    }

    public ValueParameter<IntValue> DepthParameter {
      get { return (ValueParameter<IntValue>)Parameters["Depth"]; }
    }

    [StorableConstructor]
    protected BiasedMultiVRPSolutionCrossover(bool deserializing) : base(deserializing) { }
    protected BiasedMultiVRPSolutionCrossover(BiasedMultiVRPSolutionCrossover original, Cloner cloner) : base(original, cloner) { }
    public BiasedMultiVRPSolutionCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleArray>("ActualProbabilities", "The array of relative probabilities for each operator."));
      Parameters.Add(new ValueLookupParameter<StringValue>("SuccessProgressAnalysis", "The success progress analyisis to be considered",
        new StringValue("ExecutedCrossoverOperator")));

      Parameters.Add(new ValueLookupParameter<DoubleValue>("Factor", "The factor with which the probabilities should be updated", new DoubleValue(0.2)));
      Parameters.Add(new ValueParameter<DoubleValue>("LowerBound", "The depth of the individuals in the scope tree.", new DoubleValue(0.01)));
      Parameters.Add(new ValueParameter<IntValue>("Depth", "The depth of the individuals in the scope tree.", new IntValue(1)));

      SelectedOperatorParameter.ActualName = "SelectedCrossoverOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BiasedMultiVRPSolutionCrossover(this, cloner);
    }

    public override void InitializeState() {
      base.InitializeState();

      ActualProbabilitiesParameter.Value = null;
    }

    public override IOperation InstrumentedApply() {
      IOperator successor = null;

      if (ActualProbabilitiesParameter.ActualValue == null) {
        ActualProbabilitiesParameter.Value = ProbabilitiesParameter.ActualValue.Clone() as DoubleArray;
      } else {
        String key = "SuccessfulOffspringAnalyzer Results";

        ResultCollection results = null;
        IScope scope = ExecutionContext.Parent.Scope;
        int depth = 1;
        while (scope != null && depth < DepthParameter.Value.Value) {
          scope = scope.Parent;
          depth++;
        }
        if (scope != null)
          results = scope.Variables["Results"].Value as ResultCollection;

        if (results != null && results.ContainsKey(key)) {
          ResultCollection successProgressAnalysisResult = results[key].Value as ResultCollection;
          key = SuccessProgressAnalyisis.Value.Value;

          if (successProgressAnalysisResult.ContainsKey(key)) {
            DataTable successProgressAnalysis = successProgressAnalysisResult[key].Value as DataTable;

            for (int i = 0; i < Operators.Count; i++) {
              IOperator current = Operators[i];

              if (successProgressAnalysis.Rows.ContainsKey(current.Name)) {
                DataRow row = successProgressAnalysis.Rows[current.Name];

                double sum = 0.0;
                ObservableList<double> usages = row.Values;

                sum += (double)usages.Last();

                ActualProbabilitiesParameter.ActualValue[i] += (sum / ActualProbabilitiesParameter.ActualValue[i]) * Factor.Value.Value;
              }
            }
          }
        }

        //normalize
        double max = ActualProbabilitiesParameter.ActualValue.Max();
        for (int i = 0; i < ActualProbabilitiesParameter.ActualValue.Length; i++) {
          ActualProbabilitiesParameter.ActualValue[i] /= max;
          ActualProbabilitiesParameter.ActualValue[i] =
            Math.Max(LowerBoundParameter.Value.Value,
            ActualProbabilitiesParameter.ActualValue[i]);
        }
      }

      //////////////// code has to be duplicated since ActualProbabilitiesParameter.ActualValue are updated and used for operator selection
      IRandom random = RandomParameter.ActualValue;
      DoubleArray probabilities = ActualProbabilitiesParameter.ActualValue;
      if (probabilities.Length != Operators.Count) {
        throw new InvalidOperationException(Name + ": The list of probabilities has to match the number of operators");
      }
      var checkedOperators = Operators.CheckedItems;
      if (checkedOperators.Count() > 0) {
        // select a random operator from the checked operators
        successor = checkedOperators.SampleProportional(random, 1, checkedOperators.Select(x => probabilities[x.Index]), false, false).First().Value;
      }

      IOperation successorOp = null;
      if (Successor != null)
        successorOp = ExecutionContext.CreateOperation(Successor);
      OperationCollection next = new OperationCollection(successorOp);
      if (successor != null) {
        SelectedOperatorParameter.ActualValue = new StringValue(successor.Name);

        if (CreateChildOperation)
          next.Insert(0, ExecutionContext.CreateChildOperation(successor));
        else next.Insert(0, ExecutionContext.CreateOperation(successor));
      } else {
        SelectedOperatorParameter.ActualValue = new StringValue("");
      }

      return next;
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An operator for analyzing the solution diversity in a population.
  /// </summary>
  [Item("SuccessfulOffspringAnalyzer", "An operator for analyzing certain properties in the successful offspring. The properties to be analyzed can be specified in the CollectedValues parameter.")]
  [StorableClass]
  public sealed class SuccessfulOffspringAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public bool EnabledByDefault {
      get { return false; }
    }

    public ValueParameter<StringValue> SuccessfulOffspringFlagParameter {
      get { return (ValueParameter<StringValue>)Parameters["SuccessfulOffspringFlag"]; }
    }

    public ValueParameter<ItemCollection<StringValue>> CollectedValuesParameter {
      get { return (ValueParameter<ItemCollection<StringValue>>)Parameters["CollectedValues"]; }
    }

    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public LookupParameter<ResultCollection> SuccessfulOffspringAnalysisParameter {
      get { return (LookupParameter<ResultCollection>)Parameters["SuccessfulOffspringAnalysis"]; }
    }

    public ILookupParameter<IntValue> GenerationsParameter {
      get { return (LookupParameter<IntValue>)Parameters["Generations"]; }
    }

    public ValueParameter<IntValue> DepthParameter {
      get { return (ValueParameter<IntValue>)Parameters["Depth"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SuccessfulOffspringAnalyzer(this, cloner);
    }
    [StorableConstructor]
    private SuccessfulOffspringAnalyzer(bool deserializing) : base(deserializing) { }
    private SuccessfulOffspringAnalyzer(SuccessfulOffspringAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SuccessfulOffspringAnalyzer()
      : base() {
      Parameters.Add(new ValueParameter<StringValue>("SuccessfulOffspringFlag", "The name of the flag which indicates if the individual was successful.", new StringValue("SuccessfulOffspring")));
      Parameters.Add(new ValueParameter<ItemCollection<StringValue>>("CollectedValues", "The properties of the successful offspring that should be collected.", new ItemCollection<StringValue>()));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the succedd progress analysis results should be stored."));
      Parameters.Add(new LookupParameter<IntValue>("Generations", "The current number of generations."));
      Parameters.Add(new LookupParameter<ResultCollection>("SuccessfulOffspringAnalysis", "The successful offspring analysis which is created."));
      Parameters.Add(new ValueParameter<IntValue>("Depth", "The depth of the individuals in the scope tree.", new IntValue(1)));

      CollectedValuesParameter.Value.Add(new StringValue("SelectedCrossoverOperator"));
      CollectedValuesParameter.Value.Add(new StringValue("SelectedManipulationOperator"));
    }

    public override IOperation Apply() {
      ResultCollection results = ResultsParameter.ActualValue;

      List<IScope> scopes = new List<IScope>() { ExecutionContext.Scope };
      for (int i = 0; i < DepthParameter.Value.Value; i++)
        scopes = scopes.Select(x => (IEnumerable<IScope>)x.SubScopes).Aggregate((a, b) => a.Concat(b)).ToList();

      ItemCollection<StringValue> collectedValues = CollectedValuesParameter.Value;
      foreach (StringValue collected in collectedValues) {
        //collect the values of the successful offspring
        Dictionary<String, int> counts = new Dictionary<String, int>();
        for (int i = 0; i < scopes.Count; i++) {
          IScope child = scopes[i];
          string successfulOffspringFlag = SuccessfulOffspringFlagParameter.Value.Value;
          if (child.Variables.ContainsKey(collected.Value) &&
              child.Variables.ContainsKey(successfulOffspringFlag) &&
              (child.Variables[successfulOffspringFlag].Value is BoolValue) &&
              (child.Variables[successfulOffspringFlag].Value as BoolValue).Value) {
            String key = child.Variables[collected.Value].Value.ToString();

            if (!counts.ContainsKey(key))
              counts.Add(key, 1);
            else
              counts[key]++;
          }
        }

        if (counts.Count > 0) {
          //create a data table containing the collected values
          ResultCollection successfulOffspringAnalysis;

          if (SuccessfulOffspringAnalysisParameter.ActualValue == null) {
            successfulOffspringAnalysis = new ResultCollection();
            SuccessfulOffspringAnalysisParameter.ActualValue = successfulOffspringAnalysis;
          } else {
            successfulOffspringAnalysis = SuccessfulOffspringAnalysisParameter.ActualValue;
          }

          string resultKey = "SuccessfulOffspringAnalyzer Results";
          if (!results.ContainsKey(resultKey)) {
            results.Add(new Result(resultKey, successfulOffspringAnalysis));
          } else {
            results[resultKey].Value = successfulOffspringAnalysis;
          }

          DataTable successProgressAnalysis;
          if (!successfulOffspringAnalysis.ContainsKey(collected.Value)) {
            successProgressAnalysis = new DataTable();
            successProgressAnalysis.Name = collected.Value;
            successfulOffspringAnalysis.Add(new Result(collected.Value, successProgressAnalysis));
          } else {
            successProgressAnalysis = successfulOffspringAnalysis[collected.Value].Value as DataTable;
          }

          int successfulCount = 0;
          foreach (string key in counts.Keys) {
            successfulCount += counts[key];
          }

          foreach (String value in counts.Keys) {
            DataRow row;
            if (!successProgressAnalysis.Rows.ContainsKey(value)) {
              row = new DataRow(value);
              int iterations = GenerationsParameter.ActualValue.Value;

              //fill up all values seen the first time
              for (int i = 1; i < iterations; i++)
                row.Values.Add(0);

              successProgressAnalysis.Rows.Add(row);
            } else {
              row = successProgressAnalysis.Rows[value];
            }

            row.Values.Add(counts[value] / (double)successfulCount);
          }

          //fill up all values that are not present in the current generation
          foreach (DataRow row in successProgressAnalysis.Rows) {
            if (!counts.ContainsKey(row.Name))
              row.Values.Add(0);
          }
        }
      }

      return base.Apply();
    }
  }
}

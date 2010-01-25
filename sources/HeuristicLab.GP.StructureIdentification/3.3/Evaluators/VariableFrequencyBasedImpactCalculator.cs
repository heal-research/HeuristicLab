#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Linq;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Modeling;

namespace HeuristicLab.GP.StructureIdentification {
  public class VariableFrequencyBasedImpactCalculator : OperatorBase {

    public VariableFrequencyBasedImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("VariableFrequency", "List of variable frequencies over all iterations", typeof(ItemList<ItemList>), VariableKind.In));
      AddVariableInfo(new VariableInfo("RelativeFrequencyVariableImpact", "Variable impacts", typeof(ItemList), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<ItemList> variableFrequencies = GetVariableValue<ItemList<ItemList>>("VariableFrequency", scope, true, false);
      if (variableFrequencies != null) {
        var inputVariables = from x in variableFrequencies[0]
                             select ((StringData)x).Data;

        var frequencies = from x in variableFrequencies.Skip(1)
                          let row = from v in x
                                    select ((DoubleData)v).Data
                          select row;
        Dictionary<string, double> qualityImpacts = Calculate(inputVariables, frequencies);

        ItemList varImpacts = GetVariableValue<ItemList>("RelativeFrequencyVariableImpact", scope, true, false);
        if (varImpacts == null) {
          varImpacts = new ItemList();
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("RelativeFrequencyVariableImpact"), varImpacts));
        }

        varImpacts.Clear();
        foreach (KeyValuePair<string, double> p in qualityImpacts) {
          ItemList row = new ItemList();
          row.Add(new StringData(p.Key));
          row.Add(new DoubleData(p.Value));
          varImpacts.Add(row);
        }

      }
      return null;
    }

    private static Dictionary<string, double> Calculate(
      IEnumerable<string> inputvariables,
      IEnumerable<IEnumerable<double>> frequencies) {
      int nVariables = inputvariables.Count();
      int iterations = frequencies.Count();
      // allocate lists of frequencies for each variable
      List<double>[] frequencyTrajectory = new List<double>[nVariables];
      for (int i = 0; i < nVariables; i++) {
        frequencyTrajectory[i] = new List<double>();
      }
      foreach (var iterationFrequency in frequencies) {
        int varIndex = 0;
        if (iterationFrequency.Count() != frequencyTrajectory.Length) throw new ArgumentException();
        foreach (var varIterationFrequency in iterationFrequency) {
          frequencyTrajectory[varIndex++].Add(varIterationFrequency);
        }
      }

      double[] impacts = new double[nVariables];

      // calculate impacts as integral over the whole run
      for (int variableIndex = 0; variableIndex < nVariables; variableIndex++) {
        double baseLine = frequencyTrajectory[variableIndex][0];
        for (int i = 0; i < iterations; i++) {
          impacts[variableIndex] += (frequencyTrajectory[variableIndex][i] - baseLine);
        }
        impacts[variableIndex] /= iterations - 1;
      }

      Dictionary<string, double> result = new Dictionary<string, double>();
      int j = 0;
      foreach (string variableName in inputvariables) {
        result[variableName] = impacts[j++];
      }
      return result;
    }
  }
}

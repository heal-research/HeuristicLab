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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class RegressionInstanceProvider : ProblemInstanceProvider<IRegressionProblemData> {

    public override IRegressionProblemData LoadData(string path) {
      TableFileParser csvFileParser = new TableFileParser();
      csvFileParser.Parse(path);

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      string targetVar = csvFileParser.VariableNames.Where(x => dataset.DoubleVariables.Contains(x)).Last();

      IEnumerable<string> allowedInputVars = dataset.DoubleVariables.Where(x => !x.Equals(targetVar));

      IRegressionProblemData regData = new RegressionProblemData(dataset, allowedInputVars, targetVar);

      int trainingPartEnd = csvFileParser.Rows * 2 / 3;
      regData.TrainingPartition.Start = 0;
      regData.TrainingPartition.End = trainingPartEnd;
      regData.TestPartition.Start = trainingPartEnd;
      regData.TestPartition.End = csvFileParser.Rows;

      int pos = path.LastIndexOf('\\');
      if (pos < 0)
        regData.Name = path;
      else {
        pos++;
        regData.Name = path.Substring(pos, path.Length - pos);
      }
      return regData;
    }
  }
}

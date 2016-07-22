#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
namespace HeuristicLab.Problems.DataAnalysis {
  public interface IClassificationProblemData : IDataAnalysisProblemData {
    string TargetVariable { get; set; }

    IEnumerable<string> ClassNames { get; }
    string PositiveClass { get; set; }
    IEnumerable<double> ClassValues { get; }
    int Classes { get; }

    string GetClassName(double classValue);
    double GetClassValue(string className);
    void SetClassName(double classValue, string className);

    double GetClassificationPenalty(string correctClass, string estimatedClass);
    double GetClassificationPenalty(double correctClassValue, double estimatedClassValue);
    void SetClassificationPenalty(string correctClassName, string estimatedClassName, double penalty);
    void SetClassificationPenalty(double correctClassValue, double estimatedClassValue, double penalty);
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  [StorableType("938BBDA4-0A01-4144-932D-AF1DA30E9747")]
  public abstract class SymbolicTimeSeriesPrognosisSingleObjectiveEvaluator : SymbolicDataAnalysisSingleObjectiveEvaluator<ITimeSeriesPrognosisProblemData>, ISymbolicTimeSeriesPrognosisSingleObjectiveEvaluator {
    private const string HorizonParameterName = "Horizon";
    public IValueLookupParameter<IntValue> HorizonParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[HorizonParameterName]; }
    }

    [StorableConstructor]
    protected SymbolicTimeSeriesPrognosisSingleObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicTimeSeriesPrognosisSingleObjectiveEvaluator(SymbolicTimeSeriesPrognosisSingleObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicTimeSeriesPrognosisSingleObjectiveEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(HorizonParameterName, "The time interval for which the prognosis should be calculated.", new IntValue(1)));
      ApplyLinearScalingParameter.Hidden = true;
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Parameters;
using HeuristicLab.Optimization;
using HeuristicLab.Operators;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System.Collections.Generic;
using HeuristicLab.Problems.DataAnalysis.MultiVariate;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Interfaces {
  public interface IMultiObjectiveSymbolicVectorRegressionEvaluator : IMultiVariateDataAnalysisEvaluator, IMultiObjectiveEvaluator {
    IValueLookupParameter<IntValue> SamplesStartParameter { get; }
    IValueLookupParameter<IntValue> SamplesEndParameter { get; }
    ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter { get; }
    ILookupParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter { get; }
  }
}

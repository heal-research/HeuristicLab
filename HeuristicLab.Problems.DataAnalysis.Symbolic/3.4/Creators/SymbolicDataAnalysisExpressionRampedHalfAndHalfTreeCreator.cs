#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using System;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("RampedHalfAndHalfTreeCreator", "An operator that creates new symbolic expression trees in an alternate way: half the trees are created usign the 'Grow' method while the other half are created using the 'Full' method")]
  public class SymbolicDataAnalysisExpressionRampedHalfAndHalfTreeCreator : RampedHalfAndHalfTreeCreator, ISymbolicDataAnalysisSolutionCreator {
    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionRampedHalfAndHalfTreeCreator(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisExpressionRampedHalfAndHalfTreeCreator(SymbolicDataAnalysisExpressionRampedHalfAndHalfTreeCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicDataAnalysisExpressionRampedHalfAndHalfTreeCreator(this, cloner); }

    public SymbolicDataAnalysisExpressionRampedHalfAndHalfTreeCreator() : base() { }
  }
}

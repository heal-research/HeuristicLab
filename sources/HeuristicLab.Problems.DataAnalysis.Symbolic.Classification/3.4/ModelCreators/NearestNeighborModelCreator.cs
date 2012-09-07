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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("NearestNeighborModelCreator", "")]
  public sealed class NearestNeighborModelCreator : ParameterizedNamedItem, ISymbolicClassificationModelCreator {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }

    public IFixedValueParameter<IntValue> KParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["K"]; }
    }

    [StorableConstructor]
    private NearestNeighborModelCreator(bool deserializing) : base(deserializing) { }
    private NearestNeighborModelCreator(NearestNeighborModelCreator original, Cloner cloner) : base(original, cloner) { }
    public NearestNeighborModelCreator()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("K", "The number of neighbours to use to determine the class.", new IntValue(3)));
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new NearestNeighborModelCreator(this, cloner); }


    public ISymbolicClassificationModel CreateSymbolicClassificationModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue) {
      return new SymbolicNearestNeighbourClassificationModel(KParameter.Value.Value, tree, interpreter, lowerEstimationLimit, upperEstimationLimit);
    }

  }
}

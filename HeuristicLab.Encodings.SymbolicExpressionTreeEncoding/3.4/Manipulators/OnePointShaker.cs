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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using System.Collections.Generic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("OnePointShaker", "Selects a random node with local parameters and manipulates the selected node.")]
  public sealed class OnePointShaker : SymbolicExpressionTreeManipulator {
    private const string ShakingFactorParameterName = "ShakingFactor";
    #region parameter properties
    public IValueParameter<DoubleValue> ShakingFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[ShakingFactorParameterName]; }
    }
    #endregion
    #region properties
    public double ShakingFactor {
      get { return ShakingFactorParameter.Value.Value; }
      set { ShakingFactorParameter.Value.Value = value; }
    }
    #endregion
    [StorableConstructor]
    private OnePointShaker(bool deserializing) : base(deserializing) { }
    private OnePointShaker(OnePointShaker original, Cloner cloner) : base(original, cloner) { }
    public OnePointShaker()
      : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(ShakingFactorParameterName, "The shaking factor that should be used for the manipulation of constants (default=1.0).", new DoubleValue(1.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OnePointShaker(this, cloner);
    }

    protected override void Manipulate(IRandom random, ISymbolicExpressionTree tree) {
      List<ISymbolicExpressionTreeNode> parametricNodes = new List<ISymbolicExpressionTreeNode>();
      tree.Root.ForEachNodePostfix(n => {
        if (n.HasLocalParameters) parametricNodes.Add(n);
      });
      if (parametricNodes.Count > 0) {
        var selectedPoint = parametricNodes.SelectRandom(random);
        selectedPoint.ShakeLocalParameters(random, ShakingFactor);
      }
    }
  }
}

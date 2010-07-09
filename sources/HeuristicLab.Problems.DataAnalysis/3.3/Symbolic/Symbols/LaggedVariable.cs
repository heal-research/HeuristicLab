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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using System.Collections.Generic;
using System;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols {
  [StorableClass]
  [Item("LaggedVariable", "Represents a variable value with a time offset.")]
  public sealed class LaggedVariable : Variable {
    //#region Properties
    //[Storable]
    //private double weightNu;
    //public double WeightNu {
    //  get { return weightNu; }
    //  set { weightNu = value; }
    //}
    //[Storable]
    //private double weightSigma;
    //public double WeightSigma {
    //  get { return weightSigma; }
    //  set {
    //    if (weightSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
    //    weightSigma = value;
    //  }
    //}
    //[Storable]
    //private double weightManipulatorNu;
    //public double WeightManipulatorNu {
    //  get { return weightManipulatorNu; }
    //  set { weightManipulatorNu = value; }
    //}
    //[Storable]
    //private double weightManipulatorSigma;
    //public double WeightManipulatorSigma {
    //  get { return weightManipulatorSigma; }
    //  set {
    //    if (weightManipulatorSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
    //    weightManipulatorSigma = value;
    //  }
    //}
    //private List<string> variableNames;
    //[Storable]
    //public IEnumerable<string> VariableNames {
    //  get { return variableNames; }
    //  set {
    //    if (value == null) throw new ArgumentNullException();
    //    variableNames.Clear();
    //    variableNames.AddRange(value);
    //  }
    //}
    [Storable]
    private int minLag;
    public int MinLag {
      get { return minLag; }
      set { minLag = value; }
    }
    [Storable]
    private int maxLag;
    public int MaxLag {
      get { return maxLag; }
      set { maxLag = value; }
    }
    //#endregion
    public LaggedVariable()
      : base("LaggedVariable", "Represents a variable value with a time offset.") {
      //weightNu = 1.0;
      //weightSigma = 1.0;
      //weightManipulatorNu = 0.0;
      //weightManipulatorSigma = 1.0;
      //variableNames = new List<string>();
      minLag = -1; maxLag = -1;
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new LaggedVariableTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      LaggedVariable clone = (LaggedVariable)base.Clone(cloner);
      //clone.weightNu = weightNu;
      //clone.weightSigma = weightSigma;
      //clone.variableNames = new List<string>(variableNames);
      //clone.weightManipulatorNu = weightManipulatorNu;
      //clone.weightManipulatorSigma = weightManipulatorSigma;
      clone.minLag = minLag;
      clone.maxLag = maxLag;
      return clone;
    }
  }
}

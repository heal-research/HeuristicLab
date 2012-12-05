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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  [Item("TabuMaker", "Base class for all operators that set a move tabu.")]
  [StorableClass]
  public abstract class TabuMaker : SingleSuccessorOperator, ITabuMaker {
    public override bool CanChangeName {
      get { return false; }
    }
    public LookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (LookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    public ValueLookupParameter<IntValue> TabuTenureParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["TabuTenure"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    [StorableConstructor]
    protected TabuMaker(bool deserializing) : base(deserializing) { }
    protected TabuMaker(TabuMaker original, Cloner cloner) : base(original, cloner) { }
    protected TabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list where move attributes are stored."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenure", "The tenure of the tabu list."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the move."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      int tabuTenure = TabuTenureParameter.ActualValue.Value;
      if (tabuTenure == 0) return base.Apply();
      if (tabuTenure < 0) throw new InvalidOperationException("A TabuTenure of less than 0 is not allowed.");

      int overlength = tabuList.Count - tabuTenure;
      if (overlength >= 0) {
        for (int i = 0; i < tabuTenure - 1; i++)
          tabuList[i] = tabuList[i + overlength + 1];
        while (tabuList.Count >= tabuTenure)
          tabuList.RemoveAt(tabuList.Count - 1);
      }

      tabuList.Add(GetTabuAttribute(MaximizationParameter.ActualValue.Value, QualityParameter.ActualValue.Value, MoveQualityParameter.ActualValue.Value));
      return base.Apply();
    }

    protected abstract IItem GetTabuAttribute(bool maximization, double quality, double moveQuality);
  }
}

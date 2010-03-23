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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("TabuMoveMaker", "Base class for all operators that set a move tabu.")]
  [StorableClass]
  public abstract class TabuMoveMaker : SingleSuccessorOperator, ITabuMoveMaker {
    public LookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (LookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    public ValueLookupParameter<IntValue> TabuTenureParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["TabuTenure"]; }
    }

    protected TabuMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list where move attributes are stored."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenure", "The tenure of the tabu list."));
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      int tabuTenure = TabuTenureParameter.ActualValue.Value;

      int overlength = tabuList.Count - tabuTenure;
      if (overlength >= 0) {
        for (int i = 0; i < tabuTenure - 1; i++)
          tabuList[i] = tabuList[i + overlength + 1];
        while (tabuList.Count >= tabuTenure)
          tabuList.RemoveAt(tabuList.Count - 1);
      }

      tabuList.Add(GetTabuAttribute());
      return base.Apply();
    }

    protected abstract IItem GetTabuAttribute();
  }
}

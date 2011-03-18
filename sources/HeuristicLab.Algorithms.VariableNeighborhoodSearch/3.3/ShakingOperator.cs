#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.VariableNeighborhoodSearch {
  /// <summary>
  /// A shaking operator for VNS.
  /// </summary>
  [Item("ShakingOperator", "A shaking operator for VNS.")]
  [StorableClass]
  public class ShakingOperator<T> : CheckedMultiOperator<T>, IShakingOperator where T : class, IManipulator {
    public IValueLookupParameter<IntValue> IndexParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Index"]; }
    }
    
    public ILookupParameter<BoolValue> ContinueParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Continue"]; }
    }
        
    [StorableConstructor]
    protected ShakingOperator(bool deserializing) : base(deserializing) { }
    protected ShakingOperator(ShakingOperator<T> original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShakingOperator<T>(this, cloner);
    }
    public ShakingOperator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("Index", "The index of the operator that should be applied (k)."));
      Parameters.Add(new LookupParameter<BoolValue>("Continue", "True if k <= available operators."));
    }

    public void OnProblemChanged(IProblem problem) {
      Operators.Clear();

      if (problem != null) {
        foreach (T manipulator in problem.Operators.OfType<T>().OrderBy(x => x.Name))
          Operators.Add(manipulator);
      }
    }

    public override IOperation Apply() {
      int index = IndexParameter.ActualValue.Value;    
      var operators = base.Operators.CheckedItems.ToList();

      IOperator successor = null;

      if (index >= operators.Count - 1) {
        ContinueParameter.ActualValue = new BoolValue(false);
      } else {
        ContinueParameter.ActualValue = new BoolValue(true);
      }

      if (index >= 0 && index < operators.Count) {
        successor = operators[index].Value;
      }

      OperationCollection next = new OperationCollection(base.Apply());
      if (successor != null)
        next.Insert(0, ExecutionContext.CreateOperation(successor));

      return next;
    }
  }
}

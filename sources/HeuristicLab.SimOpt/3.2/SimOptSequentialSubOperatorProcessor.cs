#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.SimOpt {
  public class SimOptSequentialSubOperatorProcessor : OperatorBase {
    public override string Description {
      get {
        return @"This operator encapsulates the functionality of processing the parameters of a simulation parameter vector. It works as follows:
1. The parameters are extracted [SimOptParameterExtractor]
2. The parameters are processed [SequentialSubScopeProcessor which applies the SubOperators of this operator on each parameter]
3. The parameters are updated again [SimOptParameterPacker]

Should the packing fail due to constraint violations, the operator will retry as often as specified in the variable MaximumTries.";
      }
    }

    public SimOptSequentialSubOperatorProcessor()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The parameter vector", typeof(ConstrainedItemList), VariableKind.New | VariableKind.In | VariableKind.Out));
      AddVariable(new Variable("ItemsBackup", new NullData()));
      AddVariableInfo(new VariableInfo("MaximumTries", "", typeof(IntData), VariableKind.In));
      GetVariableInfo("MaximumTries").Local = true;
      AddVariable(new Variable("MaximumTries", new IntData(100)));
      AddVariable(new Variable("Tries", new IntData(1)));
    }

    public override IOperation Apply(IScope scope) {

      int tries = (GetVariable("Tries").Value as IntData).Data;
      int maxTries = GetVariableValue<IntData>("MaximumTries", scope, true).Data;

      IVariableInfo itemsInfo = GetVariableInfo("Items");
      SimOptParameterExtractor extractor = new SimOptParameterExtractor();
      extractor.GetVariableInfo("Items").ActualName = itemsInfo.ActualName;

      SequentialSubScopesProcessor sssp = new SequentialSubScopesProcessor();
      foreach (IOperator op in SubOperators)
        sssp.AddSubOperator(op);

      SimOptParameterPacker packer = new SimOptParameterPacker();
      packer.GetVariableInfo("Items").ActualName = itemsInfo.ActualName;
      if (tries < maxTries) {
        SimOptSequentialSubOperatorProcessor sssop = (SimOptSequentialSubOperatorProcessor)this.Clone();
        (sssop.GetVariable("Tries").Value as IntData).Data = tries + 1;
        packer.AddSubOperator(this);
      }

      CompositeOperation operation = new CompositeOperation();
      operation.AddOperation(new AtomicOperation(extractor, scope));
      operation.AddOperation(new AtomicOperation(sssp, scope));
      operation.AddOperation(new AtomicOperation(packer, scope));

      return operation;
    }
  }
}

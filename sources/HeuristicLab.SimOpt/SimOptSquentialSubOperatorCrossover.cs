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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Selection;

namespace HeuristicLab.SimOpt {
  public class SimOptSquentialSubOperatorCrossover : OperatorBase {
    private UniformSequentialSubScopesProcessor usssp, usssp2, usssp3, usssp4;
    private SequentialSubScopesProcessor sssp;
    private SimOptParameterExtractor extractor;
    private SimOptParameterPacker packer;
    private SimOptCrossoverPreparator preparator;
    private MergingReducer merger;
    private bool uptodate;

    public override string Description {
      get {
        return @"This operator encapsulates the functionality of crossing the parameters of a simulation parameter vector. It works as follows:
1. The parameters of all parents are extracted [UniformSequentialSubScopeProcessor which applies a SimOptParameterExtractor]
2. The parents are prepared for crossing by grouping the respective parameters [SimOptCrossoverPreparator]
3. The parameters are crossed [UniformSequentialSubScopeProcessor which applies SequentialSubScopeProcessor which applies the SubOperators of this operator on each parameter group (except for the last one)]
4. Assigning the crossed parameters to the respective children [MergingReducer]
5. Update the parameters [UniformSequentialSubScopeProcessor which applies SimOptParameterPacker]

Should the packing fail due to constraint violations, the operator will execute the last of its suboperators.";
      }
    }

    public SimOptSquentialSubOperatorCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The parameter vector", typeof(ConstrainedItemList), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("Parents", "The number of parents per child", typeof(IntData), VariableKind.In));
      uptodate = false;
    }

    public override IOperation Apply(IScope scope) {
      string itemsActualName = GetVariableInfo("Items").ActualName;
      string parentsActualName = GetVariableInfo("Parents").ActualName;
      int parameters = SubOperators.Count - 1;

      if (!uptodate) {
        usssp = new UniformSequentialSubScopesProcessor();
        extractor = new SimOptParameterExtractor();
        usssp.AddSubOperator(extractor);

        preparator = new SimOptCrossoverPreparator();

        usssp2 = new UniformSequentialSubScopesProcessor();
        sssp = new SequentialSubScopesProcessor();
        for (int i = 0; i < parameters; i++) {
          sssp.AddSubOperator(SubOperators[i]);
        }
        usssp2.AddSubOperator(sssp);

        usssp3 = new UniformSequentialSubScopesProcessor();
        merger = new MergingReducer();
        usssp3.AddSubOperator(merger);

        usssp4 = new UniformSequentialSubScopesProcessor();
        packer = new SimOptParameterPacker();
        packer.AddSubOperator(SubOperators[SubOperators.Count - 1]);
        usssp4.AddSubOperator(packer);
        uptodate = true;
      }
      // Setting the actual names is necessary as the operator does not know if they've changed
      extractor.GetVariableInfo("Items").ActualName = itemsActualName;
      preparator.GetVariableInfo("Parents").ActualName = parentsActualName;
      packer.GetVariableInfo("Items").ActualName = itemsActualName;

      CompositeOperation co = new CompositeOperation();
      co.AddOperation(new AtomicOperation(usssp, scope));
      co.AddOperation(new AtomicOperation(preparator, scope));
      co.AddOperation(new AtomicOperation(usssp2, scope));
      co.AddOperation(new AtomicOperation(usssp3, scope));
      co.AddOperation(new AtomicOperation(usssp4, scope));
      return co;
    }

    protected override void OnSubOperatorAdded(IOperator subOperator, int index) {
      base.OnSubOperatorAdded(subOperator, index);
      uptodate = false;
    }

    protected override void OnSubOperatorRemoved(IOperator subOperator, int index) {
      base.OnSubOperatorRemoved(subOperator, index);
      uptodate = false;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      uptodate = false;
    }
  }
}

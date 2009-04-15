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
using System.Text;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Permutation;
using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Routing.TSP;
using HeuristicLab.Logging;

namespace HeuristicLab.SGA.Hardwired {
  class SGAMainWithHWControllStructures : CombinedOperator {
    public override string Description {
      get { return @"Implements the SGAMain with hardwired control structures. Operators are delegated."; }
    }

    public SGAMainWithHWControllStructures()
      : base() {
      AddVariableInfo(new VariableInfo("Selector", "Selection strategy for SGA", typeof(OperatorBase), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      int nrOfGenerations = 1000;

      OperatorBase selector = (OperatorBase)GetVariableValue("Selector", scope, true);
      OperatorBase createChildren = new CreateChildren();
      OperatorBase createReplacement = new CreateReplacement();
      QualityLogger ql = new QualityLogger();
      
      BestAverageWorstQualityCalculator bawqc = new BestAverageWorstQualityCalculator();
      DataCollector dc = new DataCollector();
      ItemList<StringData> names = dc.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      names.Add(new StringData("AverageQuality"));
      names.Add(new StringData("WorstQuality"));
      
      LinechartInjector lci = new LinechartInjector();
      lci.GetVariableInfo("Linechart").ActualName = "Quality Linechart";
      lci.GetVariable("NumberOfLines").GetValue<IntData>().Data = 3;

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "Generations";

      LessThanComparator ltc = new LessThanComparator();
      ltc.GetVariableInfo("LeftSide").ActualName = "Generations";
      ltc.GetVariableInfo("RightSide").ActualName = "MaximumGenerations";
      ltc.GetVariableInfo("Result").ActualName = "GenerationsCondition";

      for (int i = 0; i < nrOfGenerations; i++) {
        selector.Execute(scope);
        createChildren.Execute(scope.SubScopes[1]);
        createReplacement.Execute(scope);
        ql.Execute(scope);
        bawqc.Equals(scope);
        dc.Execute(scope);
        lci.Execute(scope);
        c.Execute(scope);
        ltc.Execute(scope);
      }
      return null;
    } // Apply
  } // class SGAMainWithHWControllStructures
} // namespace HeuristicLab.SGA.Hardwired

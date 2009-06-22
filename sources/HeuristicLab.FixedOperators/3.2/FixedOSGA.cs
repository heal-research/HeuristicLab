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

#region Using directives
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;
using HeuristicLab.Logging;
using HeuristicLab.Operators;
using HeuristicLab.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Selection.OffspringSelection;
#endregion

namespace HeuristicLab.FixedOperators {
  class FixedOSGAMain : FixedSGAMain {
    WeightedOffspringFitnessComparer wofc; 

    public FixedOSGAMain()
      : base() {
      Name = "FixeOSGAMain";
    }

    /// <summary>
    /// Apply
    /// </summary>
    public override IOperation Apply(IScope scope) {
      base.Apply(scope);

      #region Initialization
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

      IntData maxGenerations = GetVariableValue<IntData>("MaximumGenerations", scope, true);
      IntData nrOfGenerations = GetVariableValue<IntData>("Generations", scope, true);

      IntData subscopeNr;
      try {
        subscopeNr = scope.GetVariableValue<IntData>("SubScopeNr", false);
      }
      catch (Exception) {
        subscopeNr = new IntData(0);
        scope.AddVariable(new Variable("SubScopeNr", subscopeNr));
      }

      GetOperatorsFromScope(scope);

      try {
        sb.RemoveSubOperator(0);
      }
      catch (Exception) {
      }
      sb.AddSubOperator(mutator);


      IScope s;
      IScope s2;
      #endregion


      
      
      Execute(selector, scope);

      ////// Create Children //////
      // ChildrenInitializer
      s = scope.SubScopes[1];

      ChildrenInitializer ci = new ChildrenInitializer();
      Execute(ci, s);

      SaveExecutionPointer();
      // UniformSequentialSubScopesProcessor
      for (; subscopeNr.Data < s.SubScopes.Count; subscopeNr.Data++) {
        SetExecutionPointerToLastSaved();

        s2 = s.SubScopes[subscopeNr.Data];
        Execute(crossover, s2);
        // Stochastic Branch
        Execute(sb, s2);  
        Execute(evaluator, s2);
        Execute(sr, s2);
        Execute(counter, s2);
        Execute(wofc, s2);
        Execute(sr, s2);

      } // foreach

      Execute(sorter, s);
      ////// END Create Children //////

      DoReplacement(scope);
      Execute(ql, scope);
      Execute(bawqc, scope);
      Execute(dc, scope);
      Execute(lci, scope);


    } // Apply
  } // FixedOSGAMain
} // namespace HeuristicLab.FixedOperators

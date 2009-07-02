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
  class FixedOSGAMain : FixedGAMainBase {
    WeightedOffspringFitnessComparer wofc;
    OffspringSelector os;

    public FixedOSGAMain()
      : base() {
      Name = "FixeOSGAMain";
      wofc = new WeightedOffspringFitnessComparer();
      os = new OffspringSelector();
      os.AddSubOperator(new EmptyOperator());
    }

    /// <summary>
    /// Apply
    /// </summary>
    public override IOperation Apply(IScope scope) {
      base.Apply(scope);
      
      #region Initialization
      DataCollector selectionPressureDC = new DataCollector();
      selectionPressureDC.GetVariableInfo("Values").ActualName = "SelPressValues";
      ItemList<StringData> names = selectionPressureDC.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("SelectionPressure"));

      LinechartInjector selectionPressureLCI = new LinechartInjector();
      selectionPressureLCI.GetVariableInfo("Linechart").ActualName = "SelectionPressureLinechart";
      selectionPressureLCI.GetVariable("NumberOfLines").GetValue<IntData>().Data = 1;
      selectionPressureLCI.GetVariableInfo("Values").ActualName = "SelPressValues";

      IScope s;
      IScope s2;
      bool loopSkipped = true;
      bool running = false;

      DoubleData selectionPressure = null;
      DoubleData selectionPressureLimit = new DoubleData();
      try {
        selectionPressureLimit = scope.GetVariableValue<DoubleData>("SelectionPressureLimit", false);
      }
      catch (Exception) {
      }

      #endregion

      tempExePointer = new int[10];
      tempPersExePointer = new int[10];
      try {
        for (; nrOfGenerations.Data < maxGenerations.Data; nrOfGenerations.Data++) {
          SaveExecutionPointer(0);
          do {
            SetExecutionPointerToLastSaved(0);
            Execute(selector, scope);

            ////// Create Children //////
            // ChildrenInitializer
            s = scope.SubScopes[1];

            Execute(ci, s);

            SaveExecutionPointer(1);
            // UniformSequentialSubScopesProcessor
            for (; subscopeNr.Data < s.SubScopes.Count; subscopeNr.Data++) {
              SetExecutionPointerToLastSaved(1);

              s2 = s.SubScopes[subscopeNr.Data];
              Execute(crossover, s2);
              // Stochastic Branch
              Execute(sb, s2);
              Execute(evaluator, s2);
              Execute(counter, s2);
              Execute(wofc, s2);
              Execute(sr, s2);
              loopSkipped = false;
            } // foreach

            // if for loop skipped, we had to add skiped operations
            // to execution pointer.
            if (loopSkipped)
              executionPointer += 5;

            Execute(sorter, s);
            ////// END Create Children //////
            
            running = ExecuteFirstOperator(os, scope);
            if (running) subscopeNr.Data = 0;
          } while (running);

          DoReplacement(scope);
          Execute(ql, scope);
          Execute(bawqc, scope);
          Execute(dc, scope);
          Execute(lci, scope);
          Execute(selectionPressureDC, scope);
          Execute(selectionPressureLCI, scope);
          Console.WriteLine(nrOfGenerations);

          if (selectionPressure == null)
            selectionPressure = scope.GetVariableValue<DoubleData>("SelectionPressure", false);
          if (selectionPressure.Data > selectionPressureLimit.Data)
            break;

          subscopeNr.Data = 0;
          ResetExecutionPointer();
        } // for i generations
      } // try
      catch (CancelException) {
        //Console.WriteLine("Micro engine aborted by cancel flag.");
        return new AtomicOperation(this, scope);
      }
      catch (Exception ex) {
        ex.ToString();
      }

      return null;
    } // Apply
  } // FixedOSGAMain
} // namespace HeuristicLab.FixedOperators

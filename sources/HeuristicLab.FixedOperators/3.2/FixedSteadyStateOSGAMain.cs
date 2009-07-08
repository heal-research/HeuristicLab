using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Permutation;
using HeuristicLab.Evolutionary;
using HeuristicLab.Routing.TSP;
using HeuristicLab.Logging;
using System.Diagnostics;
using HeuristicLab.Selection;
using System.Threading;
using System.IO;
using HeuristicLab.Random;


namespace HeuristicLab.FixedOperators {
  class FixedSteadyStateOSGAMain : FixedOSGAMain {

    public FixedSteadyStateOSGAMain() {
      Name = "FixedSteadyStateOSGAMain";
      os = new ProgrammableOperator();
      os.AddSubOperator(new EmptyOperator());
      ((ProgrammableOperator)os).Code = @"IScope parents = scope.SubScopes[0], children = scope.SubScopes[1];
                                          HeuristicLab.Data.DoubleData selectionPressure = scope.GetVariableValue<HeuristicLab.Data.DoubleData>(""SelectionPressure"", true);
                                          int badCount = 0;
                                          for (int i = 0 ; i < children.SubScopes.Count ; i++) {
                                            IScope child = children.SubScopes[i];
                                            bool successful = child.GetVariableValue<HeuristicLab.Data.BoolData>(""SuccessfulChild"", false).Data;
                                            if (!successful) {
                                              badCount++;
                                              children.RemoveSubScope(child);
                                              i--;
                                            } 
                                          }
                                          selectionPressure.Data += badCount / ((double)parents.SubScopes.Count);
                                          if (children.SubScopes.Count == 0) {
                                            scope.RemoveSubScope(children);
                                            scope.RemoveSubScope(parents);
                                            for (int i = 0; i < parents.SubScopes.Count; i++)
                                              scope.AddSubScope(parents.SubScopes[i]);
                                            return new AtomicOperation(op.SubOperators[0], scope);
                                          }
                                          return null;";



    } // FixedSteadyStateOSGAMain

    protected override void InitReplacement() {
      base.InitReplacement();
      ls.GetVariableInfo("Selected").ActualName = "PopulationSize";
    }

    protected override void DoReplacement(IScope scope) {
      //// SequentialSubScopesProcessor
      Execute(mr, scope);
      Execute(sorter, scope);
      Execute(ls, scope);
      Execute(rr, scope);
    } // DoReplacement
  } // class FixedSteadyStateOSGAMain
} // namespace HeuristicLab.FixedOperators

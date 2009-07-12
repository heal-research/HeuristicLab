using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.ES;
using HeuristicLab.Operators;
using HeuristicLab.Data;


namespace HeuristicLab.FixedOperators {
  class FixedESMain : FixedGAMainBase {
    protected ESRandomSelector childSelector;
    protected OperatorBase recombinator;

    public FixedESMain() {
      Name = this.GetType().Name;
      Init();
      
    } // FixedESMain

    private void Init() {
      childSelector = new ESRandomSelector();
      childSelector.GetVariableInfo("Lambda").ActualName = "ESlambda";
      childSelector.GetVariableInfo("Rho").ActualName = "ESrho";

      ls.GetVariableInfo("Selected").ActualName = "ESmu";
      //ci.GetVariableInfo("ParentsPerChild").ActualName = "ESrho";
      
    
    } // Init

    protected override void GetOperatorsFromScope(IScope scope) {
      mutator = new OperatorExtractor();
      mutator.GetVariableInfo("Operator").ActualName = "Mutator";

      evaluator = new OperatorExtractor();
      evaluator.GetVariableInfo("Operator").ActualName = "Evaluation (Rosenbrock)";

      recombinator = new OperatorExtractor();
      recombinator.GetVariableInfo("Operator").ActualName = "Recombinator";
      
      //mutator = (OperatorBase)GetVariableValue("Mutator", scope, true);
      //evaluator = GetVariableValue<OperatorBase>("Evaluator", scope, true);
      //recombinator = GetVariableValue<OperatorBase>("Recombinator", scope, true);
    }

    public override IOperation Apply(IScope scope) {
      base.Apply(scope);
      IScope s;
      IScope s2;
      bool loopSkipped = true;

      tempExePointer = new int[10];
      tempPersExePointer = new int[10];
      IntData esRho;  

      try {
        for (; nrOfGenerations.Data < maxGenerations.Data; nrOfGenerations.Data++) {

          Execute(childSelector, scope);
          ////// Create Children //////
          s = scope.SubScopes[1];
          esRho = s.GetVariableValue<IntData>("ESrho", true);

          if (esRho.Data > 1) { // Use recombination?
            Execute(ci, s);
            SaveExecutionPointer(0);
            for (; subscopeNr.Data < s.SubScopes.Count; subscopeNr.Data++) {
              SetExecutionPointerToLastSaved(0);
              s2 = s.SubScopes[subscopeNr.Data];
              Execute(recombinator, s2);
              Execute(mutator, s2);
              Execute(evaluator, s2);
              Execute(counter, s2);
              Execute(sr, s2);
              loopSkipped = false;
            } // foreach

            if (loopSkipped)
              executionPointer += 5;
          } // if (esRho.Data > 1)
          else {
            SaveExecutionPointer(0);
            for (; subscopeNr.Data < s.SubScopes.Count; subscopeNr.Data++) {
              SetExecutionPointerToLastSaved(0);
              s2 = s.SubScopes[subscopeNr.Data];
              Execute(mutator, s2);
              Execute(evaluator, s2);
              Execute(counter, s2);
              loopSkipped = false;
            } // foreach

            if (loopSkipped)
              executionPointer += 3;
          } // else

          Execute(sorter, s);


          // if for loop skipped, we had to add skiped operations
          // to execution pointer.
          if (loopSkipped)
            executionPointer += 5;

          ////// END Create Children //////


          // Plus or Comma Replacement
          BoolData plusNotation = scope.GetVariableValue<BoolData>("PlusNotation", true);
          if (plusNotation.Data)
            Execute(mr, scope);
          else
            Execute(rr, scope);

          // TODO: Parents Selection
          Execute(sorter, scope);
          Execute(ls, scope);
          Execute(rr, scope);

          Execute(ql, scope);
          Execute(bawqc, scope);
          Execute(dc, scope);
          Execute(lci, scope);
          subscopeNr.Data = 0;
          ResetExecutionPointer();
        } // for Generations
      } // try
      catch (CancelException) {
        return new AtomicOperation(this, scope);
      }

      return null;
    } // Apply
  } // class FixedESMain
} // namespace HeuristicLab.FixedOperators

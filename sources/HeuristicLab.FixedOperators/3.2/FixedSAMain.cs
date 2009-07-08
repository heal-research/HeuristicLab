using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Selection;
using HeuristicLab.Operators;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.SA;
using HeuristicLab.Logging;
using HeuristicLab.Data;

namespace HeuristicLab.FixedOperators {
  class FixedSAMain : FixedOperatorBase{
    protected LeftSelector ls;
    protected RightReducer rr;
    protected LeftReducer lr;
    protected EmptyOperator empty;
    protected ProgrammableOperator fastFitnessComparer;
    protected ProgrammableOperator moduloComparer;
    protected MultiplicativeAnnealing multiAnnealing;
    protected Counter iterationCounter;
    protected Counter evalSolutionsCounter;
    protected OperatorBase mutator;
    protected OperatorBase evaluator;
    protected BestAverageWorstQualityCalculator bawqc;
    protected DataCollector dc;
    protected LinechartInjector lci;
    protected ConditionalBranch acceptMutant;
    protected ConditionalBranch logInterval;

    public FixedSAMain() {
      Name = this.GetType().Name;
      Init();
    } // FixedSAMain

    private void Init() {
      ls = new LeftSelector();
      ls.GetVariable("CopySelected").GetValue<BoolData>().Data = true;
      ls.GetVariableInfo("Selected").Local = true;
      ls.AddVariable(new Variable("Selected", new IntData(1)));

      rr = new RightReducer();
      lr = new LeftReducer();
      empty = new EmptyOperator();
      
      fastFitnessComparer = new ProgrammableOperator();
      fastFitnessComparer.Code = @"string qualityString = scope.TranslateName(""Quality"");
      IScope mutant = scope.SubScopes[1].SubScopes[0];
      double mutantQuality = mutant.GetVariableValue<DoubleData>(qualityString, false).Data;
      double parentQuality = scope.SubScopes[0].SubScopes[0].GetVariableValue<DoubleData>(qualityString, false).Data;

      if (mutantQuality < parentQuality) {
        BoolData sc = op.GetVariableValue<BoolData>(""AcceptMutant"", scope, false, false);
        if (sc == null) {
          scope.AddVariable(new Variable(""AcceptMutant"", new BoolData(true)));
        } else sc.Data = true;
        return null;
      }


      double temperature = op.GetVariableValue<DoubleData>(""Temperature"", scope, true).Data;
      double probability = Math.Exp(-Math.Abs(mutantQuality - parentQuality) / temperature);
      IRandom random = op.GetVariableValue<IRandom>(""Random"", scope, true);
      bool success = random.NextDouble() < probability;
      BoolData sc2 = op.GetVariableValue<BoolData>(""AcceptMutant"", scope, false, false);
      if (sc2 == null) {
        scope.AddVariable(new Variable(""AcceptMutant"", new BoolData(success)));
      } else sc2.Data = success;
      return null;";
      fastFitnessComparer.AddVariableInfo(new VariableInfo("AcceptMutant", "AcceptMutant", typeof(BoolData), VariableKind.New | VariableKind.Out));
      fastFitnessComparer.AddVariableInfo(new VariableInfo("Random", "Random", typeof(IRandom), VariableKind.In));
      fastFitnessComparer.AddVariableInfo(new VariableInfo("Temperature", "Temperature", typeof(DoubleData), VariableKind.In));

      moduloComparer = new ProgrammableOperator();
      moduloComparer.Code = @"Result.Data = (LeftSide.Data % Periodicity.Data == RightSide.Data);";
      moduloComparer.AddVariableInfo(new VariableInfo("LeftSide", "LeftSide", typeof(IntData), VariableKind.In));
      moduloComparer.GetVariableInfo("LeftSide").ActualName = "Iteration";
      //moduloComparer.AddVariableInfo(new VariableInfo("Result", "LogCondition", typeof(BoolData), VariableKind.New | VariableKind.Out));
      moduloComparer.GetVariableInfo("Result").ActualName = "LogCondition";
      moduloComparer.AddVariableInfo(new VariableInfo("Periodicity", "Periodicity", typeof(IntData), VariableKind.In));
      moduloComparer.GetVariableInfo("Periodicity").ActualName = "LogInterval";
      moduloComparer.AddVariableInfo(new VariableInfo("RightSide", "RightSide", typeof(IntData), VariableKind.In));
      moduloComparer.GetVariableInfo("RightSide").Local = true;
      moduloComparer.AddVariable(new Variable("RightSide", new IntData(0)));


      multiAnnealing = new MultiplicativeAnnealing();
      iterationCounter = new Counter();
      iterationCounter.GetVariableInfo("Value").ActualName = "Iteration";
      evalSolutionsCounter = new Counter();
      evalSolutionsCounter.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";


      bawqc = new BestAverageWorstQualityCalculator();
      bawqc.GetVariableInfo("AverageQuality").Local = true;
      bawqc.GetVariableInfo("WorstQuality").Local = true;
      bawqc.AddVariable(new Variable("AverageQuality", new DoubleData(6601)));
      bawqc.AddVariable(new Variable("WorstQuality", new DoubleData(6601)));
      
      dc = new DataCollector();
      dc.GetVariableInfo("Values").ActualName = "QualityValues";

      ItemList<StringData> names = dc.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));

      lci = new LinechartInjector();
      lci.GetVariableInfo("Linechart").ActualName = "QualityLinechart";
      lci.GetVariableInfo("Values").ActualName = "QualityValues";
      lci.GetVariable("NumberOfLines").GetValue<IntData>().Data = 1;

      acceptMutant = new ConditionalBranch();
      acceptMutant.GetVariableInfo("Condition").ActualName = "AcceptMutant";
      acceptMutant.AddSubOperator(rr);
      acceptMutant.AddSubOperator(lr);

      SequentialProcessor sp = new SequentialProcessor();
      logInterval = new ConditionalBranch();
      logInterval.GetVariableInfo("Condition").ActualName = "LogCondition";
      logInterval.AddSubOperator(sp);
      sp.AddSubOperator(bawqc);
      sp.AddSubOperator(dc);
      sp.AddSubOperator(lci);

    } // Init

    public override IOperation Apply(IScope scope) {
      base.Apply(scope);


      IntData maxIterations = scope.GetVariableValue<IntData>("MaximumIterations", false);
      IntData nrOfIterations = scope.GetVariableValue<IntData>("Iteration", false);
      IntData subscopeNr;

      try {
        subscopeNr = scope.GetVariableValue<IntData>("SubScopeNr", false);
      }
      catch (Exception) {
        subscopeNr = new IntData(0);
        scope.AddVariable(new Variable("SubScopeNr", subscopeNr));
      }

      DoubleData temperature = null;
      DoubleData minTemperature = null;
      try {
        temperature = scope.GetVariableValue<DoubleData>("Temperature", false);
        minTemperature = scope.GetVariableValue<DoubleData>("MinimumTemperature", false);
      }
      catch (Exception) {
      }


      //GetOperatorsFromScope(scope);


      IScope s;
      IScope s2;
      bool loopSkipped = true;
      tempExePointer = new int[10];
      tempPersExePointer = new int[10];

      mutator = scope.GetVariableValue<OperatorBase>("Mutator", false);
      evaluator = scope.GetVariableValue<OperatorBase>("Evaluator", false);

      try {
        for (; nrOfIterations.Data < maxIterations.Data; nrOfIterations.Data++) {

          Execute(ls, scope);
          s = scope.SubScopes[1];

          SaveExecutionPointer(0);
          // UniformSequentialSubScopesProcessor
          for (; subscopeNr.Data < s.SubScopes.Count; subscopeNr.Data++) {
            SetExecutionPointerToLastSaved(0);
            s2 = s.SubScopes[subscopeNr.Data];
            Execute(mutator, s2);
            Execute(evaluator, s2);
            Execute(evalSolutionsCounter, s2);
            loopSkipped = false;
          } // foreach

          // if for loop skipped, we had to add skiped operations
          // to execution pointer.
          if (loopSkipped)
            executionPointer += 3;

          Execute(fastFitnessComparer, scope);
          Execute(acceptMutant, scope);
          Execute(moduloComparer, scope);
          Execute(logInterval, scope);
          Execute(multiAnnealing, scope);
          subscopeNr.Data = 0;
          ResetExecutionPointer();
          if (temperature.Data <= minTemperature.Data)
            break;
        } // for nrOfGenerations
      
      } // try
      catch (CancelException) {
        return new AtomicOperation(this, scope);
      }
      return null;
    } // Apply


  } // class FixedSAMain
} // namespace HeuristicLab.FixedOperators

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
using System.Xml;
using System.Diagnostics;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Logging;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Modeling;

namespace HeuristicLab.SupportVectorMachines {
  public class SupportVectorRegression : ItemBase, IEditable, IAlgorithm {

    public string Name { get { return "SupportVectorRegression"; } }
    public string Description { get { return "TODO"; } }

    private SequentialEngine.SequentialEngine engine;
    public IEngine Engine {
      get { return engine; }
    }

    public IOperator ProblemInjector {
      get {
        IOperator main = GetMainOperator();
        return main.SubOperators[1];
      }
      set {
        IOperator main = GetMainOperator();
        main.RemoveSubOperator(1);
        main.AddSubOperator(value, 1);
      }
    }

    public DoubleArrayData NuList {
      get { return GetVariableInjector().GetVariable("NuList").GetValue<DoubleArrayData>(); }
      set { GetVariableInjector().GetVariable("NuList").Value = value; }
    }

    public int MaxNuIndex {
      get { return GetVariableInjector().GetVariable("MaxNuIndex").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxNuIndex").GetValue<IntData>().Data = value; }
    }

    public DoubleArrayData CostList {
      get { return GetVariableInjector().GetVariable("CostList").GetValue<DoubleArrayData>(); }
      set { GetVariableInjector().GetVariable("CostList").Value = value; }
    }

    public int MaxCostIndex {
      get { return GetVariableInjector().GetVariable("MaxCostIndex").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxCostIndex").GetValue<IntData>().Data = value; }
    }

    public SupportVectorRegression() {
      engine = new SequentialEngine.SequentialEngine();
      CombinedOperator algo = CreateAlgorithm();
      engine.OperatorGraph.AddOperator(algo);
      engine.OperatorGraph.InitialOperator = algo;
      MaxCostIndex = CostList.Data.Length;
      MaxNuIndex = NuList.Data.Length;
    }

    private CombinedOperator CreateAlgorithm() {
      CombinedOperator algo = new CombinedOperator();
      algo.Name = "SupportVectorRegression";
      IOperator main = CreateMainLoop();
      algo.OperatorGraph.AddOperator(main);
      algo.OperatorGraph.InitialOperator = main;
      return algo;
    }

    private IOperator CreateMainLoop() {
      SequentialProcessor main = new SequentialProcessor();
      main.AddSubOperator(CreateGlobalInjector());
      main.AddSubOperator(new ProblemInjector());

      SequentialProcessor nuLoop = new SequentialProcessor();
      nuLoop.Name = "NuLoop";
      SequentialProcessor costLoop = new SequentialProcessor();
      costLoop.Name = "CostLoop";
      main.AddSubOperator(nuLoop);
      nuLoop.AddSubOperator(CreateResetOperator("CostIndex"));
      nuLoop.AddSubOperator(costLoop);
      SubScopesCreater modelScopeCreator = new SubScopesCreater();
      modelScopeCreator.GetVariableInfo("SubScopes").Local = true;
      modelScopeCreator.AddVariable(new HeuristicLab.Core.Variable("SubScopes", new IntData(1)));
      costLoop.AddSubOperator(modelScopeCreator);
      SequentialSubScopesProcessor subScopesProcessor = new SequentialSubScopesProcessor();
      costLoop.AddSubOperator(subScopesProcessor);
      SequentialProcessor modelProcessor = new SequentialProcessor();
      subScopesProcessor.AddSubOperator(modelProcessor);
      modelProcessor.AddSubOperator(CreateSetNextParameterValueOperator("Nu"));
      modelProcessor.AddSubOperator(CreateSetNextParameterValueOperator("Cost"));

      SupportVectorCreator modelCreator = new SupportVectorCreator();
      modelCreator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      modelCreator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      modelCreator.GetVariableInfo("SVMCost").ActualName = "Cost";
      modelCreator.GetVariableInfo("SVMGamma").ActualName = "Gamma";
      modelCreator.GetVariableInfo("SVMKernelType").ActualName = "KernelType";
      modelCreator.GetVariableInfo("SVMModel").ActualName = "Model";
      modelCreator.GetVariableInfo("SVMNu").ActualName = "Nu";
      modelCreator.GetVariableInfo("SVMRangeTransform").ActualName = "RangeTransform";
      modelCreator.GetVariableInfo("SVMType").ActualName = "Type";
      

      modelProcessor.AddSubOperator(modelCreator);
      modelProcessor.AddSubOperator(CreateEvaluator("Training"));
      modelProcessor.AddSubOperator(CreateEvaluator("Validation"));
      modelProcessor.AddSubOperator(CreateEvaluator("Test"));

      DataCollector collector = new DataCollector();
      collector.GetVariableInfo("Values").ActualName = "Log";
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("Nu"));
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("Cost"));
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("ValidationMSE"));
      modelProcessor.AddSubOperator(collector);

      BestSolutionStorer solStorer = new BestSolutionStorer();
      solStorer.GetVariableInfo("Quality").ActualName = "ValidationMSE";
      solStorer.GetVariableInfo("Maximization").Local = true;
      solStorer.GetVariableInfo("BestSolution").ActualName = "BestValidationSolution";
      solStorer.AddVariable(new HeuristicLab.Core.Variable("Maximization", new BoolData(false)));

      costLoop.AddSubOperator(solStorer);
      SubScopesRemover remover = new SubScopesRemover();
      costLoop.AddSubOperator(remover);
      costLoop.AddSubOperator(CreateCounter("Cost"));
      costLoop.AddSubOperator(CreateComparator("Cost"));
      ConditionalBranch costBranch = new ConditionalBranch();
      costBranch.Name = "CostLoop";
      costBranch.GetVariableInfo("Condition").ActualName = "RepeatCostLoop";
      costBranch.AddSubOperator(costLoop);
      costLoop.AddSubOperator(costBranch);

      nuLoop.AddSubOperator(CreateCounter("Nu"));
      nuLoop.AddSubOperator(CreateComparator("Nu"));
      ConditionalBranch nuBranch = new ConditionalBranch();
      nuBranch.Name = "NuLoop";
      nuBranch.GetVariableInfo("Condition").ActualName = "RepeatNuLoop";
      nuBranch.AddSubOperator(nuLoop);
      nuLoop.AddSubOperator(nuBranch);
      return main;
    }

    private IOperator CreateComparator(string p) {
      LessThanComparator comparator = new LessThanComparator();
      comparator.Name = p + "IndexComparator";
      comparator.GetVariableInfo("LeftSide").ActualName = p + "Index";
      comparator.GetVariableInfo("RightSide").ActualName = "Max" + p + "Index";
      comparator.GetVariableInfo("Result").ActualName = "Repeat" + p + "Loop";
      return comparator;
    }

    private IOperator CreateCounter(string p) {
      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = p + "Index";
      c.Name = p + "Counter";
      return c;
    }

    private IOperator CreateEvaluator(string p) {
      CombinedOperator op = new CombinedOperator();
      op.Name = p + "Evaluator";
      SequentialProcessor seqProc = new SequentialProcessor();
     
      SupportVectorEvaluator evaluator = new SupportVectorEvaluator();
      evaluator.Name = p + "SimpleEvaluator";
      evaluator.GetVariableInfo("SVMModel").ActualName = "Model";
      evaluator.GetVariableInfo("SVMRangeTransform").ActualName = "RangeTransform";
      evaluator.GetVariableInfo("SamplesStart").ActualName = p + "SamplesStart";
      evaluator.GetVariableInfo("SamplesEnd").ActualName = p + "SamplesEnd";
      evaluator.GetVariableInfo("Values").ActualName = p + "Values";
      SimpleMSEEvaluator mseEvaluator = new SimpleMSEEvaluator();
      mseEvaluator.Name = p + "MseEvaluator";
      mseEvaluator.GetVariableInfo("Values").ActualName = p + "Values";
      mseEvaluator.GetVariableInfo("MSE").ActualName = p + "MSE";
      SimpleR2Evaluator r2Evaluator = new SimpleR2Evaluator();
      r2Evaluator.Name = p + "R2Evaluator";
      r2Evaluator.GetVariableInfo("Values").ActualName = p + "Values";
      r2Evaluator.GetVariableInfo("R2").ActualName = p + "R2";

      seqProc.AddSubOperator(evaluator);
      seqProc.AddSubOperator(mseEvaluator);
      seqProc.AddSubOperator(r2Evaluator);

      op.OperatorGraph.AddOperator(seqProc);
      op.OperatorGraph.InitialOperator = seqProc;
      return op;
    }

    private IOperator CreateSetNextParameterValueOperator(string paramName) {
      ProgrammableOperator progOp = new ProgrammableOperator();
      progOp.Name = "SetNext" + paramName;
      progOp.RemoveVariableInfo("Result");
      progOp.AddVariableInfo(new VariableInfo("Value", "Value", typeof(DoubleData), VariableKind.New));
      progOp.AddVariableInfo(new VariableInfo("ValueIndex", "ValueIndex", typeof(IntData), VariableKind.In));
      progOp.AddVariableInfo(new VariableInfo("ValueList", "ValueList", typeof(DoubleArrayData), VariableKind.In));
      progOp.Code =
@"
Value.Data = ValueList.Data[ValueIndex.Data];
";

      progOp.GetVariableInfo("Value").ActualName = paramName;
      progOp.GetVariableInfo("ValueIndex").ActualName = paramName + "Index";
      progOp.GetVariableInfo("ValueList").ActualName = paramName + "List";
      return progOp;
    }

    private IOperator CreateResetOperator(string paramName) {
      ProgrammableOperator progOp = new ProgrammableOperator();
      progOp.Name = "Reset" + paramName;
      progOp.RemoveVariableInfo("Result");
      progOp.AddVariableInfo(new VariableInfo("Value", "Value", typeof(IntData), VariableKind.In | VariableKind.Out));
      progOp.Code ="Value.Data = 0;";
      progOp.GetVariableInfo("Value").ActualName = paramName;
      return progOp;
    }

    private IOperator CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("CostIndex", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("CostList", new DoubleArrayData(new double[] { 0.1, 0.25, 0.5, 1.0, 2.0, 4.0, 8.0, 16.0, 32.0, 64.0, 128.0 })));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxCostIndex", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("NuIndex", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("NuList", new DoubleArrayData(new double[] { 0.01, 0.05, 0.1, 0.5, 0.9 })));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxNuIndex", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("Log", new ItemList()));
      injector.AddVariable(new HeuristicLab.Core.Variable("Gamma", new DoubleData(1)));
      injector.AddVariable(new HeuristicLab.Core.Variable("KernelType", new StringData("RBF")));
      injector.AddVariable(new HeuristicLab.Core.Variable("Type", new StringData("NU_SVR")));

      return injector;
    }

    private IOperator GetVariableInjector() {
      return GetMainOperator().SubOperators[0];
    }

    private IOperator GetMainOperator() {
      CombinedOperator svm = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      return svm.OperatorGraph.InitialOperator;
    }

    public override IView CreateView() {
      return engine.CreateView();
    }

    #region IEditable Members

    public IEditor CreateEditor() {
      return engine.CreateEditor();
    }

    #endregion
  }
}

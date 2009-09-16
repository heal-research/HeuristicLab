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
using HeuristicLab.Logging;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Modeling;
using HeuristicLab.Random;
using HeuristicLab.Selection;

namespace HeuristicLab.SupportVectorMachines {
  public class SupportVectorRegression : ItemBase, IEditable, IAlgorithm {

    public virtual string Name { get { return "SupportVectorRegression"; } }
    public virtual string Description { get { return "TODO"; } }

    private SequentialEngine.SequentialEngine engine;
    public IEngine Engine {
      get { return engine; }
    }

    public Dataset Dataset {
      get { return ProblemInjector.GetVariableValue<Dataset>("Dataset", null, false); }
      set { ProblemInjector.GetVariable("Dataset").Value = value; }
    }

    public int TargetVariable {
      get { return ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data = value; }
    }

    public IOperator ProblemInjector {
      get {
        IOperator main = GetMainOperator();
        CombinedOperator probInject = (CombinedOperator)main.SubOperators[0].SubOperators[2];
        return probInject.OperatorGraph.InitialOperator.SubOperators[0];
      }
      set {
        IOperator main = GetMainOperator();
        CombinedOperator probInject = (CombinedOperator)main.SubOperators[0].SubOperators[2];
        probInject.OperatorGraph.InitialOperator.RemoveSubOperator(0);
        probInject.OperatorGraph.InitialOperator.AddSubOperator(value, 0);
      }
    }

    public IAnalyzerModel Model {
      get {
        if (!engine.Terminated) throw new InvalidOperationException("The algorithm is still running. Wait until the algorithm is terminated to retrieve the result.");
        IScope bestModelScope = engine.GlobalScope.SubScopes[0];
        return CreateSVMModel(bestModelScope);
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

    public DoubleArrayData GammaList {
      get { return GetVariableInjector().GetVariable("GammaList").GetValue<DoubleArrayData>(); }
      set { GetVariableInjector().GetVariable("GammaList").Value = value; }
    }

    public int MaxGammaIndex {
      get { return GetVariableInjector().GetVariable("MaxGammaIndex").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxGammaIndex").GetValue<IntData>().Data = value; }
    }

    public string SvmType {
      get { return GetVariableInjector().GetVariable("Type").GetValue<StringData>().Data; }
      set { GetVariableInjector().GetVariable("Type").GetValue<StringData>().Data = value; }
    }

    public SupportVectorRegression() {
      engine = new SequentialEngine.SequentialEngine();
      CombinedOperator algo = CreateAlgorithm();
      engine.OperatorGraph.AddOperator(algo);
      engine.OperatorGraph.InitialOperator = algo;
      MaxCostIndex = CostList.Data.Length;
      MaxNuIndex = NuList.Data.Length;
      MaxGammaIndex = GammaList.Data.Length;
    }

    private CombinedOperator CreateAlgorithm() {
      CombinedOperator algo = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      algo.Name = Name;
      seq.Name = Name;

      IOperator initialization = CreateInitialization();
      IOperator main = CreateMainLoop();
      IOperator postProc = CreatePostProcessingOperator();

      seq.AddSubOperator(initialization);
      seq.AddSubOperator(main);
      seq.AddSubOperator(postProc);

      algo.OperatorGraph.InitialOperator = seq;
      algo.OperatorGraph.AddOperator(seq);

      return algo;
    }

    protected virtual IOperator CreateInitialization() {
      SequentialProcessor seq = new SequentialProcessor();
      seq.Name = "Initialization";
      seq.AddSubOperator(new RandomInjector());
      seq.AddSubOperator(CreateGlobalInjector());
      seq.AddSubOperator(CreateProblemInjector());
      return seq;
    }

    protected virtual IOperator CreateProblemInjector() {
      return DefaultRegressionOperators.CreateProblemInjector();
    }

    private IOperator CreateMainLoop() {
      SequentialProcessor main = new SequentialProcessor();
      main.Name = "Main";
      #region initial solution
      SubScopesCreater modelScopeCreator = new SubScopesCreater();
      modelScopeCreator.GetVariableInfo("SubScopes").Local = true;
      modelScopeCreator.AddVariable(new HeuristicLab.Core.Variable("SubScopes", new IntData(1)));
      main.AddSubOperator(modelScopeCreator);

      SequentialSubScopesProcessor seqSubScopesProc = new SequentialSubScopesProcessor();
      IOperator modelProcessor = CreateModelProcessor();

      seqSubScopesProc.AddSubOperator(modelProcessor);
      main.AddSubOperator(seqSubScopesProc);
      #endregion

      SequentialProcessor nuLoop = new SequentialProcessor();
      nuLoop.Name = "NuLoop";

      SequentialProcessor gammaLoop = new SequentialProcessor();
      gammaLoop.Name = "GammaLoop";

      nuLoop.AddSubOperator(gammaLoop);

      IOperator costCounter = CreateCounter("Cost");
      IOperator costComparator = CreateComparator("Cost");
      gammaLoop.AddSubOperator(costCounter);
      gammaLoop.AddSubOperator(costComparator);

      ConditionalBranch costBranch = new ConditionalBranch();
      costBranch.Name = "IfValidCostIndex";
      costBranch.GetVariableInfo("Condition").ActualName = "RepeatCostLoop";

      // build cost loop
      SequentialProcessor costLoop = new SequentialProcessor();
      costLoop.Name = "CostLoop";

      #region selection of better solution
      costLoop.AddSubOperator(modelScopeCreator);
      SequentialSubScopesProcessor subScopesProcessor = new SequentialSubScopesProcessor();
      costLoop.AddSubOperator(subScopesProcessor);
      subScopesProcessor.AddSubOperator(new EmptyOperator());
      subScopesProcessor.AddSubOperator(modelProcessor);

      Sorter sorter = new Sorter();
      sorter.GetVariableInfo("Value").ActualName = "ValidationQuality";
      sorter.GetVariableInfo("Descending").Local = true;
      sorter.AddVariable(new Variable("Descending", new BoolData(false)));
      costLoop.AddSubOperator(sorter);

      LeftSelector selector = new LeftSelector();
      selector.GetVariableInfo("Selected").Local = true;
      selector.AddVariable(new Variable("Selected", new IntData(1)));
      costLoop.AddSubOperator(selector);

      RightReducer reducer = new RightReducer();
      costLoop.AddSubOperator(reducer);
      #endregion

      costLoop.AddSubOperator(costCounter);
      costLoop.AddSubOperator(costComparator);

      costBranch.AddSubOperator(costLoop);
      costLoop.AddSubOperator(costBranch);

      gammaLoop.AddSubOperator(costBranch); // inner loop
      gammaLoop.AddSubOperator(CreateResetOperator("CostIndex", -1));
      gammaLoop.AddSubOperator(CreateCounter("Gamma"));
      gammaLoop.AddSubOperator(CreateComparator("Gamma"));

      ConditionalBranch gammaBranch = new ConditionalBranch();
      gammaBranch.Name = "GammaLoop";
      gammaBranch.GetVariableInfo("Condition").ActualName = "RepeatGammaLoop";
      gammaBranch.AddSubOperator(gammaLoop);
      gammaLoop.AddSubOperator(gammaBranch);

      nuLoop.AddSubOperator(CreateResetOperator("GammaIndex", 0));

      nuLoop.AddSubOperator(CreateCounter("Nu"));
      nuLoop.AddSubOperator(CreateComparator("Nu"));

      ConditionalBranch nuBranch = new ConditionalBranch();
      nuBranch.Name = "NuLoop";
      nuBranch.GetVariableInfo("Condition").ActualName = "RepeatNuLoop";

      nuBranch.AddSubOperator(nuLoop);
      nuLoop.AddSubOperator(nuBranch);

      main.AddSubOperator(nuLoop);
      return main;
    }

    private IOperator CreateModelProcessor() {
      SequentialProcessor modelProcessor = new SequentialProcessor();
      modelProcessor.AddSubOperator(CreateSetNextParameterValueOperator("Nu"));
      modelProcessor.AddSubOperator(CreateSetNextParameterValueOperator("Cost"));
      modelProcessor.AddSubOperator(CreateSetNextParameterValueOperator("Gamma"));

      SupportVectorCreator modelCreator = new SupportVectorCreator();
      modelCreator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      modelCreator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";
      modelCreator.GetVariableInfo("SVMCost").ActualName = "Cost";
      modelCreator.GetVariableInfo("SVMGamma").ActualName = "Gamma";
      modelCreator.GetVariableInfo("SVMKernelType").ActualName = "KernelType";
      modelCreator.GetVariableInfo("SVMModel").ActualName = "Model";
      modelCreator.GetVariableInfo("SVMNu").ActualName = "Nu";
      modelCreator.GetVariableInfo("SVMType").ActualName = "Type";

      modelProcessor.AddSubOperator(modelCreator);
      CombinedOperator trainingEvaluator = (CombinedOperator)CreateEvaluator("ActualTraining");
      trainingEvaluator.OperatorGraph.InitialOperator.SubOperators[1].GetVariableInfo("MSE").ActualName = "Quality";
      modelProcessor.AddSubOperator(trainingEvaluator);
      modelProcessor.AddSubOperator(CreateEvaluator("Validation"));

      DataCollector collector = new DataCollector();
      collector.GetVariableInfo("Values").ActualName = "Log";
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("Nu"));
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("Cost"));
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("Gamma"));
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("ValidationQuality"));
      modelProcessor.AddSubOperator(collector);
      return modelProcessor;
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

    protected virtual IOperator CreateEvaluator(string p) {
      CombinedOperator op = new CombinedOperator();
      op.Name = p + "Evaluator";
      SequentialProcessor seqProc = new SequentialProcessor();

      SupportVectorEvaluator evaluator = new SupportVectorEvaluator();
      evaluator.Name = p + "SimpleEvaluator";
      evaluator.GetVariableInfo("SVMModel").ActualName = "Model";
      evaluator.GetVariableInfo("SamplesStart").ActualName = p + "SamplesStart";
      evaluator.GetVariableInfo("SamplesEnd").ActualName = p + "SamplesEnd";
      evaluator.GetVariableInfo("Values").ActualName = p + "Values";
      SimpleMSEEvaluator mseEvaluator = new SimpleMSEEvaluator();
      mseEvaluator.Name = p + "MseEvaluator";
      mseEvaluator.GetVariableInfo("Values").ActualName = p + "Values";
      mseEvaluator.GetVariableInfo("MSE").ActualName = p + "Quality";
      SimpleR2Evaluator r2Evaluator = new SimpleR2Evaluator();
      r2Evaluator.Name = p + "R2Evaluator";
      r2Evaluator.GetVariableInfo("Values").ActualName = p + "Values";
      r2Evaluator.GetVariableInfo("R2").ActualName = p + "R2";
      SimpleMeanAbsolutePercentageErrorEvaluator mapeEvaluator = new SimpleMeanAbsolutePercentageErrorEvaluator();
      mapeEvaluator.Name = p + "MAPEEvaluator";
      mapeEvaluator.GetVariableInfo("Values").ActualName = p + "Values";
      mapeEvaluator.GetVariableInfo("MAPE").ActualName = p + "MAPE";
      SimpleMeanAbsolutePercentageOfRangeErrorEvaluator mapreEvaluator = new SimpleMeanAbsolutePercentageOfRangeErrorEvaluator();
      mapreEvaluator.Name = p + "MAPREEvaluator";
      mapreEvaluator.GetVariableInfo("Values").ActualName = p + "Values";
      mapreEvaluator.GetVariableInfo("MAPRE").ActualName = p + "MAPRE";
      SimpleVarianceAccountedForEvaluator vafEvaluator = new SimpleVarianceAccountedForEvaluator();
      vafEvaluator.Name = p + "VAFEvaluator";
      vafEvaluator.GetVariableInfo("Values").ActualName = p + "Values";
      vafEvaluator.GetVariableInfo("VAF").ActualName = p + "VAF";

      seqProc.AddSubOperator(evaluator);
      seqProc.AddSubOperator(mseEvaluator);
      seqProc.AddSubOperator(r2Evaluator);
      seqProc.AddSubOperator(mapeEvaluator);
      seqProc.AddSubOperator(mapreEvaluator);
      seqProc.AddSubOperator(vafEvaluator);

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

    private IOperator CreateResetOperator(string paramName, int value) {
      ProgrammableOperator progOp = new ProgrammableOperator();
      progOp.Name = "Reset" + paramName;
      progOp.RemoveVariableInfo("Result");
      progOp.AddVariableInfo(new VariableInfo("Value", "Value", typeof(IntData), VariableKind.In | VariableKind.Out));
      progOp.Code = "Value.Data = " + value + ";";
      progOp.GetVariableInfo("Value").ActualName = paramName;
      return progOp;
    }

    protected virtual VariableInjector CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("CostIndex", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("CostList", new DoubleArrayData(new double[] { 
        Math.Pow(2,-5),
        Math.Pow(2,-3),
        Math.Pow(2,-1),
        2,
        Math.Pow(2,3),
        Math.Pow(2,5),
        Math.Pow(2,7),
        Math.Pow(2,9),
        Math.Pow(2,11),
        Math.Pow(2,13),
        Math.Pow(2,15)})));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxCostIndex", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("NuIndex", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("NuList", new DoubleArrayData(new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.9, 0.8, 1 })));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxNuIndex", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("Log", new ItemList()));
      injector.AddVariable(new HeuristicLab.Core.Variable("GammaIndex", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("GammaList", new DoubleArrayData(new double[] {
        3.0517578125E-05, 0.0001220703125,0.00048828125,0.001953125,
        0.0078125,0.03125,0.125,0.5,2,4,8})));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxGammaIndex", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("KernelType", new StringData("RBF")));
      injector.AddVariable(new HeuristicLab.Core.Variable("Type", new StringData("NU_SVR")));
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData(1000.0)));
      return injector;
    }

    protected virtual IOperator CreatePostProcessingOperator() {
      CombinedOperator modelAnalyser = new CombinedOperator();
      modelAnalyser.Name = "Model Analyzer";
      SequentialSubScopesProcessor seqSubScopeProc = new SequentialSubScopesProcessor();
      SequentialProcessor seqProc = new SequentialProcessor();

      PredictorBuilder predictorBuilder = new PredictorBuilder();
      predictorBuilder.GetVariableInfo("SVMModel").ActualName = "Model";

      seqProc.AddSubOperator(CreateEvaluator("Test"));
      seqProc.AddSubOperator(CreateEvaluator("Training"));
      seqProc.AddSubOperator(predictorBuilder);
      seqProc.AddSubOperator(CreateModelAnalyzerOperator());

      seqSubScopeProc.AddSubOperator(seqProc);
      modelAnalyser.OperatorGraph.InitialOperator = seqSubScopeProc;
      modelAnalyser.OperatorGraph.AddOperator(seqSubScopeProc);
      return modelAnalyser;
    }

    protected virtual IOperator CreateModelAnalyzerOperator() {
      return DefaultRegressionOperators.CreatePostProcessingOperator();
    }


    protected virtual IAnalyzerModel CreateSVMModel(IScope bestModelScope) {
      AnalyzerModel model = new AnalyzerModel();
      model.SetMetaData("Cost", bestModelScope.GetVariableValue<DoubleData>("Cost", false).Data);
      model.SetMetaData("Nu", bestModelScope.GetVariableValue<DoubleData>("Nu", false).Data);
      DefaultRegressionOperators.PopulateAnalyzerModel(bestModelScope, model);

      return model;
    }

    protected IOperator GetVariableInjector() {
      return GetMainOperator().SubOperators[0].SubOperators[1];
    }

    protected IOperator GetMainOperator() {
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

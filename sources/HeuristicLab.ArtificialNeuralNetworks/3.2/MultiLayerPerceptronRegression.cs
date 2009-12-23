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
using HeuristicLab.Modeling;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Selection;
using HeuristicLab.Operators.Programmable;

namespace HeuristicLab.ArtificialNeuralNetworks {
  public class MultiLayerPerceptronRegression : ItemBase, IEditable, IAlgorithm {

    public virtual string Name { get { return "MultiLayerPerceptronRegression"; } }
    public virtual string Description { get { return "TODO"; } }

    private IEngine engine;
    public virtual IEngine Engine {
      get { return engine; }
    }

    public virtual Dataset Dataset {
      get { return ProblemInjector.GetVariableValue<Dataset>("Dataset", null, false); }
      set { ProblemInjector.GetVariable("Dataset").Value = value; }
    }

    public virtual string TargetVariable {
      get { return ProblemInjector.GetVariableValue<StringData>("TargetVariable", null, false).Data; }
      set { ProblemInjector.GetVariableValue<StringData>("TargetVariable", null, false).Data = value; }
    }

    public virtual IOperator ProblemInjector {
      get {
        IOperator main = GetMainOperator();
        CombinedOperator probInjector = (CombinedOperator)main.SubOperators[2];
        return probInjector.OperatorGraph.InitialOperator.SubOperators[0];
      }
      set {
        IOperator main = GetMainOperator();
        CombinedOperator probInjector = (CombinedOperator)main.SubOperators[2];
        probInjector.OperatorGraph.InitialOperator.RemoveSubOperator(0);
        probInjector.OperatorGraph.InitialOperator.AddSubOperator(value, 0);
      }
    }
    public IEnumerable<string> AllowedVariables {
      get {
        ItemList<StringData> allowedVariables = ProblemInjector.GetVariableValue<ItemList<StringData>>("AllowedFeatures", null, false);
        return allowedVariables.Select(x => x.Data);
      }
      set {
        ItemList<StringData> allowedVariables = ProblemInjector.GetVariableValue<ItemList<StringData>>("AllowedFeatures", null, false);
        foreach (string x in value) allowedVariables.Add(new StringData(x));
      }
    }

    public int TrainingSamplesStart {
      get { return ProblemInjector.GetVariableValue<IntData>("TrainingSamplesStart", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TrainingSamplesStart", null, false).Data = value; }
    }

    public int TrainingSamplesEnd {
      get { return ProblemInjector.GetVariableValue<IntData>("TrainingSamplesEnd", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TrainingSamplesEnd", null, false).Data = value; }
    }

    public int ValidationSamplesStart {
      get { return ProblemInjector.GetVariableValue<IntData>("ValidationSamplesStart", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("ValidationSamplesStart", null, false).Data = value; }
    }

    public int ValidationSamplesEnd {
      get { return ProblemInjector.GetVariableValue<IntData>("ValidationSamplesEnd", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("ValidationSamplesEnd", null, false).Data = value; }
    }

    public int TestSamplesStart {
      get { return ProblemInjector.GetVariableValue<IntData>("TestSamplesStart", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TestSamplesStart", null, false).Data = value; }
    }

    public int TestSamplesEnd {
      get { return ProblemInjector.GetVariableValue<IntData>("TestSamplesEnd", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TestSamplesEnd", null, false).Data = value; }
    }

    public IntArrayData NumberOfHiddenNodesList {
      get { return GetVariableInjector().GetVariable("NumberOfHiddenNodesList").GetValue<IntArrayData>(); }
      set { GetVariableInjector().GetVariable("NumberOfHiddenNodesList").Value = value; }
    }

    public int MaxNumberOfHiddenNodesListIndex {
      get { return GetVariableInjector().GetVariable("MaxNumberOfHiddenNodesIndex").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxNumberOfHiddenNodesIndex").GetValue<IntData>().Data = value; }
    }

    public virtual IAnalyzerModel Model {
      get {
        if (!engine.Terminated) throw new InvalidOperationException("The algorithm is still running. Wait until the algorithm is terminated to retrieve the result.");
        IScope bestModelScope = engine.GlobalScope;
        return CreateMlpModel(bestModelScope);
      }
    }

    public MultiLayerPerceptronRegression() {
      engine = new SequentialEngine.SequentialEngine();
      CombinedOperator algo = CreateAlgorithm();
      engine.OperatorGraph.AddOperator(algo);
      engine.OperatorGraph.InitialOperator = algo;
      MaxNumberOfHiddenNodesListIndex = NumberOfHiddenNodesList.Data.Length;
    }

    protected virtual CombinedOperator CreateAlgorithm() {
      CombinedOperator algo = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      algo.Name = Name;
      seq.Name = Name;

      IOperator globalInjector = CreateGlobalInjector();

      IOperator mainLoop = CreateMainLoop();

      seq.AddSubOperator(globalInjector);
      seq.AddSubOperator(new RandomInjector());
      seq.AddSubOperator(CreateProblemInjector());
      seq.AddSubOperator(mainLoop);
      seq.AddSubOperator(CreatePostProcessingOperator());

      algo.OperatorGraph.InitialOperator = seq;
      algo.OperatorGraph.AddOperator(seq);

      return algo;
    }

    private IOperator CreateMainLoop() {
      SequentialProcessor seq = new SequentialProcessor();

      #region initial solution
      SubScopesCreater modelScopeCreator = new SubScopesCreater();
      modelScopeCreator.GetVariableInfo("SubScopes").Local = true;
      modelScopeCreator.AddVariable(new HeuristicLab.Core.Variable("SubScopes", new IntData(1)));
      seq.AddSubOperator(modelScopeCreator);

      SequentialSubScopesProcessor seqSubScopesProc = new SequentialSubScopesProcessor();
      IOperator modelProcessor = CreateModelProcessor();

      seqSubScopesProc.AddSubOperator(modelProcessor);
      seq.AddSubOperator(seqSubScopesProc);
      #endregion


      Counter nHiddenNodesCounter = new Counter();
      nHiddenNodesCounter.GetVariableInfo("Value").ActualName = "NumberOfHiddenNodesIndex";
      nHiddenNodesCounter.Name = "NumberOfHiddenNodesIndexCounter";

      LessThanComparator comparator = new LessThanComparator();
      comparator.Name = "NumberOfHiddenNodesIndexComparator";
      comparator.GetVariableInfo("LeftSide").ActualName = "NumberOfHiddenNodesIndex";
      comparator.GetVariableInfo("RightSide").ActualName = "MaxNumberOfHiddenNodesIndex";
      comparator.GetVariableInfo("Result").ActualName = "RepeatNumberOfHiddenNodesIndexLoop";

      ConditionalBranch branch = new ConditionalBranch();
      branch.Name = "IfValidNumberOfHiddenNodesIndex";
      branch.GetVariableInfo("Condition").ActualName = "RepeatNumberOfHiddenNodesIndexLoop";



      // build loop
      SequentialProcessor loop = new SequentialProcessor();
      loop.Name = "HiddenNodesLoop";

      #region selection of better solution

      loop.AddSubOperator(modelScopeCreator);
      SequentialSubScopesProcessor subScopesProcessor = new SequentialSubScopesProcessor();
      loop.AddSubOperator(subScopesProcessor);
      subScopesProcessor.AddSubOperator(new EmptyOperator());
      subScopesProcessor.AddSubOperator(CreateModelProcessor());

      Sorter sorter = new Sorter();
      sorter.GetVariableInfo("Value").ActualName = "ValidationQuality";
      sorter.GetVariableInfo("Descending").Local = true;
      sorter.AddVariable(new Variable("Descending", new BoolData(false)));
      loop.AddSubOperator(sorter);

      LeftSelector selector = new LeftSelector();
      selector.GetVariableInfo("Selected").Local = true;
      selector.AddVariable(new Variable("Selected", new IntData(1)));
      loop.AddSubOperator(selector);

      RightReducer reducer = new RightReducer();
      loop.AddSubOperator(reducer);
      #endregion

      loop.AddSubOperator(nHiddenNodesCounter);
      loop.AddSubOperator(comparator);

      branch.AddSubOperator(loop);
      loop.AddSubOperator(branch);


      seq.AddSubOperator(loop);
      return seq;
    }

    private IOperator CreateModelProcessor() {
      SequentialProcessor modelProcessor = new SequentialProcessor();
      modelProcessor.AddSubOperator(CreateSetNextParameterValueOperator("NumberOfHiddenNodes"));

      MultiLayerPerceptronRegressionOperator trainingOperator = new MultiLayerPerceptronRegressionOperator();
      trainingOperator.GetVariableInfo("NumberOfHiddenLayerNeurons").ActualName = "NumberOfHiddenNodes";
      trainingOperator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      trainingOperator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";

      modelProcessor.AddSubOperator(trainingOperator);
      CombinedOperator trainingEvaluator = (CombinedOperator)CreateEvaluator("ActualTraining");
      trainingEvaluator.OperatorGraph.InitialOperator.SubOperators[1].GetVariableInfo("MSE").ActualName = "Quality";
      modelProcessor.AddSubOperator(trainingEvaluator);
      modelProcessor.AddSubOperator(CreateEvaluator("Validation"));

      DataCollector collector = new DataCollector();
      collector.GetVariableInfo("Values").ActualName = "Log";
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("NumberOfHiddenNodes"));
      ((ItemList<StringData>)collector.GetVariable("VariableNames").Value).Add(new StringData("ValidationQuality"));
      modelProcessor.AddSubOperator(collector);
      return modelProcessor;
    }

    protected virtual IOperator CreateEvaluator(string p) {
      CombinedOperator op = new CombinedOperator();
      op.Name = p + "Evaluator";
      SequentialProcessor seqProc = new SequentialProcessor();

      MultiLayerPerceptronEvaluator evaluator = new MultiLayerPerceptronEvaluator();
      evaluator.Name = p + "SimpleEvaluator";
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
      progOp.AddVariableInfo(new VariableInfo("Value", "Value", typeof(IntData), VariableKind.New));
      progOp.AddVariableInfo(new VariableInfo("ValueIndex", "ValueIndex", typeof(IntData), VariableKind.In));
      progOp.AddVariableInfo(new VariableInfo("ValueList", "ValueList", typeof(IntArrayData), VariableKind.In));
      progOp.Code =
@"
Value.Data = ValueList.Data[ValueIndex.Data];
";

      progOp.GetVariableInfo("Value").ActualName = paramName;
      progOp.GetVariableInfo("ValueIndex").ActualName = paramName + "Index";
      progOp.GetVariableInfo("ValueList").ActualName = paramName + "List";
      return progOp;
    }


    protected virtual IOperator CreateProblemInjector() {
      return DefaultRegressionOperators.CreateProblemInjector();
    }

    protected virtual VariableInjector CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();

      injector.AddVariable(new HeuristicLab.Core.Variable("MaxNumberOfTrainingSamples", new IntData(4000)));
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData(1000)));
      injector.AddVariable(new HeuristicLab.Core.Variable("NumberOfHiddenNodesIndex", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxNumberOfHiddenNodesIndex", new IntData(0)));
      injector.AddVariable(new HeuristicLab.Core.Variable("NumberOfHiddenNodesList", new IntArrayData(new int[] { 2, 4, 8, 16, 32, 64, 128 })));
      injector.AddVariable(new HeuristicLab.Core.Variable("Log", new ItemList()));
      return injector;
    }

    protected virtual IOperator CreatePostProcessingOperator() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "Model Analyzer";

      SequentialSubScopesProcessor seqSubScopesProc = new SequentialSubScopesProcessor();
      SequentialProcessor seq = new SequentialProcessor();
      seqSubScopesProc.AddSubOperator(seq);

      #region simple evaluators
      MultiLayerPerceptronEvaluator trainingEvaluator = new MultiLayerPerceptronEvaluator();
      trainingEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      trainingEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";

      MultiLayerPerceptronEvaluator testEvaluator = new MultiLayerPerceptronEvaluator();
      testEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      testEvaluator.GetVariableInfo("Values").ActualName = "TestValues";

      seq.AddSubOperator(trainingEvaluator);
      seq.AddSubOperator(testEvaluator);
      #endregion

      #region variable impacts
      // calculate and set variable impacts

      PredictorBuilder predictorBuilder = new PredictorBuilder();

      seq.AddSubOperator(predictorBuilder);
      VariableQualityImpactCalculator qualityImpactCalculator = new VariableQualityImpactCalculator();
      qualityImpactCalculator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      qualityImpactCalculator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";

      seq.AddSubOperator(qualityImpactCalculator);
      #endregion

      seq.AddSubOperator(CreateModelAnalyzerOperator());
      op.OperatorGraph.AddOperator(seqSubScopesProc);
      op.OperatorGraph.InitialOperator = seqSubScopesProc;
      return op;
    }

    protected virtual IOperator CreateModelAnalyzerOperator() {
      return DefaultRegressionOperators.CreatePostProcessingOperator();
    }

    protected virtual IAnalyzerModel CreateMlpModel(IScope bestModelScope) {
      var model = new AnalyzerModel();
      CreateSpecificMlpModel(bestModelScope, model);
      #region variable impacts
      ItemList qualityImpacts = bestModelScope.GetVariableValue<ItemList>(ModelingResult.VariableQualityImpact.ToString(), false);
      foreach (ItemList row in qualityImpacts) {
        string variableName = ((StringData)row[0]).Data;
        double impact = ((DoubleData)row[1]).Data;
        model.SetVariableResult(ModelingResult.VariableQualityImpact, variableName, impact);
        model.AddInputVariable(variableName);
      }
      #endregion
      return model;
    }

    protected virtual void CreateSpecificMlpModel(IScope bestModelScope, IAnalyzerModel model) {
      DefaultRegressionOperators.PopulateAnalyzerModel(bestModelScope, model);
    }

    protected virtual IOperator GetMainOperator() {
      CombinedOperator lr = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      return lr.OperatorGraph.InitialOperator;
    }

    protected virtual IOperator GetVariableInjector() {
      return GetMainOperator().SubOperators[0];
    }

    public override IView CreateView() {
      return engine.CreateView();
    }

    #region IEditable Members

    public virtual IEditor CreateEditor() {
      return ((SequentialEngine.SequentialEngine)engine).CreateEditor();
    }

    #endregion

    #region persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      MultiLayerPerceptronRegression clone = (MultiLayerPerceptronRegression)base.Clone(clonedObjects);
      clone.engine = (IEngine)Auxiliary.Clone(Engine, clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Engine", engine, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      engine = (IEngine)PersistenceManager.Restore(node.SelectSingleNode("Engine"), restoredObjects);
    }
    #endregion
  }
}

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
using HeuristicLab.Modeling;
using HeuristicLab.GP;
using HeuristicLab.Random;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.LinearRegression {
  public class LinearRegression : ItemBase, IEditable, IAlgorithm {

    public virtual string Name { get { return "LinearRegression"; } }
    public virtual string Description { get { return "TODO"; } }

    private IEngine engine;
    public virtual IEngine Engine {
      get { return engine; }
    }

    public virtual Dataset Dataset {
      get { return ProblemInjector.GetVariableValue<Dataset>("Dataset", null, false); }
      set { ProblemInjector.GetVariable("Dataset").Value = value; }
    }

    public virtual int TargetVariable {
      get { return ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data = value; }
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
    public IEnumerable<int> AllowedVariables {
      get {
        ItemList<IntData> allowedVariables = ProblemInjector.GetVariableValue<ItemList<IntData>>("AllowedFeatures", null, false);
        return allowedVariables.Select(x => x.Data);
      }
      set {
        ItemList<IntData> allowedVariables = ProblemInjector.GetVariableValue<ItemList<IntData>>("AllowedFeatures", null, false);
        foreach (int x in value) allowedVariables.Add(new IntData(x));
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

    public virtual IAnalyzerModel Model {
      get {
        if (!engine.Terminated) throw new InvalidOperationException("The algorithm is still running. Wait until the algorithm is terminated to retrieve the result.");
        IScope bestModelScope = engine.GlobalScope;
        return CreateLRModel(bestModelScope);
      }
    }

    public LinearRegression() {
      engine = new SequentialEngine.SequentialEngine();
      CombinedOperator algo = CreateAlgorithm();
      engine.OperatorGraph.AddOperator(algo);
      engine.OperatorGraph.InitialOperator = algo;
    }

    protected virtual CombinedOperator CreateAlgorithm() {
      CombinedOperator algo = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();
      algo.Name = Name;
      seq.Name = Name;

      IOperator globalInjector = CreateGlobalInjector();

      HL3TreeEvaluatorInjector treeEvaluatorInjector = new HL3TreeEvaluatorInjector();

      LinearRegressionOperator lrOperator = new LinearRegressionOperator();
      lrOperator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      lrOperator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";

      seq.AddSubOperator(globalInjector);
      seq.AddSubOperator(new RandomInjector());
      seq.AddSubOperator(CreateProblemInjector());
      seq.AddSubOperator(treeEvaluatorInjector);
      seq.AddSubOperator(lrOperator);
      seq.AddSubOperator(CreatePostProcessingOperator());

      algo.OperatorGraph.InitialOperator = seq;
      algo.OperatorGraph.AddOperator(seq);

      return algo;
    }

    protected virtual IOperator CreateProblemInjector() {
      return DefaultRegressionOperators.CreateProblemInjector();
    }

    protected virtual VariableInjector CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData(1000)));
      injector.AddVariable(new HeuristicLab.Core.Variable("TotalEvaluatedNodes", new DoubleData(0)));

      return injector;
    }

    protected virtual IOperator CreatePostProcessingOperator() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "Model Analyzer";

      SequentialProcessor seq = new SequentialProcessor();
      HL3TreeEvaluatorInjector evaluatorInjector = new HL3TreeEvaluatorInjector();
      evaluatorInjector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData(1000.0)));
      evaluatorInjector.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";

      #region simple evaluators
      SimpleEvaluator trainingEvaluator = new SimpleEvaluator();
      trainingEvaluator.Name = "TrainingEvaluator";
      trainingEvaluator.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      trainingEvaluator.GetVariableInfo("SamplesStart").ActualName = "TrainingSamplesStart";
      trainingEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TrainingSamplesEnd";
      trainingEvaluator.GetVariableInfo("Values").ActualName = "TrainingValues";
      trainingEvaluator.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";
      SimpleEvaluator validationEvaluator = new SimpleEvaluator();
      validationEvaluator.Name = "ValidationEvaluator";
      validationEvaluator.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      validationEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";
      validationEvaluator.GetVariableInfo("Values").ActualName = "ValidationValues";
      validationEvaluator.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";
      SimpleEvaluator testEvaluator = new SimpleEvaluator();
      testEvaluator.Name = "TestEvaluator";
      testEvaluator.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";
      testEvaluator.GetVariableInfo("SamplesStart").ActualName = "TestSamplesStart";
      testEvaluator.GetVariableInfo("SamplesEnd").ActualName = "TestSamplesEnd";
      testEvaluator.GetVariableInfo("Values").ActualName = "TestValues";
      testEvaluator.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";
      seq.AddSubOperator(evaluatorInjector);
      seq.AddSubOperator(trainingEvaluator);
      seq.AddSubOperator(validationEvaluator);
      seq.AddSubOperator(testEvaluator);
      #endregion

      #region variable impacts
      // calculate and set variable impacts
      VariableNamesExtractor namesExtractor = new VariableNamesExtractor();
      namesExtractor.GetVariableInfo("VariableNames").ActualName = "InputVariableNames";
      namesExtractor.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";

      PredictorBuilder predictorBuilder = new PredictorBuilder();
      predictorBuilder.GetVariableInfo("TreeEvaluator").ActualName = "ModelAnalysisTreeEvaluator";
      predictorBuilder.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";

      seq.AddSubOperator(namesExtractor);
      seq.AddSubOperator(predictorBuilder);
      #endregion

      seq.AddSubOperator(CreateModelAnalyzerOperator());

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    protected virtual IOperator CreateModelAnalyzerOperator() {
      return DefaultRegressionOperators.CreatePostProcessingOperator();
    }

    protected virtual IAnalyzerModel CreateLRModel(IScope bestModelScope) {
      var model = new AnalyzerModel();
      DefaultRegressionOperators.PopulateAnalyzerModel(bestModelScope, model);
      return model;
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
      LinearRegression clone = (LinearRegression) base.Clone(clonedObjects);
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

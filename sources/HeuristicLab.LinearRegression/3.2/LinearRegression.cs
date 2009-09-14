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

    private SequentialEngine.SequentialEngine engine;
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
        return main.SubOperators[1];
      }
      set {
        IOperator main = GetMainOperator();
        main.RemoveSubOperator(1);
        main.AddSubOperator(value, 1);
      }
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
      algo.Name = "LinearRegression";
      seq.Name = "LinearRegression";

      var randomInjector = new RandomInjector();
      randomInjector.Name = "Random Injector";
      IOperator globalInjector = CreateGlobalInjector();
      ProblemInjector problemInjector = new ProblemInjector();
      problemInjector.GetVariableInfo("MaxNumberOfTrainingSamples").Local = true;
      problemInjector.AddVariable(new HeuristicLab.Core.Variable("MaxNumberOfTrainingSamples", new IntData(5000)));

      HL2TreeEvaluatorInjector treeEvaluatorInjector = new HL2TreeEvaluatorInjector();

      IOperator shuffler = new DatasetShuffler();
      shuffler.GetVariableInfo("ShuffleStart").ActualName = "TrainingSamplesStart";
      shuffler.GetVariableInfo("ShuffleEnd").ActualName = "TrainingSamplesEnd";

      LinearRegressionOperator lrOperator = new LinearRegressionOperator();
      lrOperator.GetVariableInfo("SamplesStart").ActualName = "ActualTrainingSamplesStart";
      lrOperator.GetVariableInfo("SamplesEnd").ActualName = "ActualTrainingSamplesEnd";

      seq.AddSubOperator(randomInjector);
      seq.AddSubOperator(problemInjector);
      seq.AddSubOperator(globalInjector);
      seq.AddSubOperator(treeEvaluatorInjector);
      seq.AddSubOperator(shuffler);
      seq.AddSubOperator(lrOperator);
      seq.AddSubOperator(CreateModelAnalyser());

      algo.OperatorGraph.InitialOperator = seq;
      algo.OperatorGraph.AddOperator(seq);

      return algo;
    }

    protected virtual VariableInjector CreateGlobalInjector() {
      VariableInjector injector = new VariableInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData(1000)));
      injector.AddVariable(new HeuristicLab.Core.Variable("TotalEvaluatedNodes", new DoubleData(0)));

      return injector;
    }

    protected virtual IOperator CreateModelAnalyser() {
      CombinedOperator op = new CombinedOperator();
      op.AddVariableInfo(new VariableInfo("FunctionTree", "The model to analyze", typeof(IGeneticProgrammingModel), VariableKind.In));
      op.Name = "Model Analyzer";
      op.GetVariableInfo("FunctionTree").ActualName = "LinearRegressionModel";

      IOperator maOp = DefaultStructureIdentificationAlgorithmOperators.CreatePostProcessingOperator();
      op.OperatorGraph.AddOperator(maOp);
      op.OperatorGraph.InitialOperator = maOp;
      return op;
    }

    protected virtual IAnalyzerModel CreateLRModel(IScope bestModelScope) {
      return DefaultStructureIdentificationAlgorithmOperators.CreateGPModel(bestModelScope);
    }

    protected virtual IOperator GetMainOperator() {
      CombinedOperator lr = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      return lr.OperatorGraph.InitialOperator;
    }

    public override IView CreateView() {
      return engine.CreateView();
    }

    #region IEditable Members

    public virtual IEditor CreateEditor() {
      return engine.CreateEditor();
    }

    #endregion
  }
}

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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Logging;
using HeuristicLab.Selection;
using HeuristicLab.Selection.OffspringSelection;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.ES {
  /// <summary>
  /// Class for the heuristic optimization technique "evolution strategy".
  /// </summary>
  public class ES : ItemBase, IEditable {
    #region Create Operators
    /// <summary>
    /// Creates a new evolution strategy.
    /// </summary>
    /// <param name="engine">The engine of the ES to create.</param>
    public static void Create(IEngine engine) {
      engine.OperatorGraph.Clear();

      CombinedOperator co = CreateES();
      co.Name = "ES";
      engine.OperatorGraph.AddOperator(co);
      engine.OperatorGraph.InitialOperator = co;

      engine.Reset();
    }
    private static CombinedOperator CreateES() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp);
      op.OperatorGraph.InitialOperator = sp;

      CombinedOperator co1 = CreateVariableInjection();
      co1.Name = "Variable Injection";
      op.OperatorGraph.AddOperator(co1);
      sp.AddSubOperator(co1);

      // place holder for ProblemInjector
      EmptyOperator eo1 = new EmptyOperator();
      eo1.Name = "ProblemInjector";
      op.OperatorGraph.AddOperator(eo1);
      co1.AddSubOperator(eo1);

      CombinedOperator co2 = CreatePopulationInitialization();
      co2.Name = "Population Initialization";
      op.OperatorGraph.AddOperator(co2);
      sp.AddSubOperator(co2);

      // place holder for SolutionGenerator
      EmptyOperator eo2 = new EmptyOperator();
      eo2.Name = "SolutionGenerator";
      op.OperatorGraph.AddOperator(eo2);
      co2.AddSubOperator(eo2);

      // place holder for Evaluator
      EmptyOperator eo3 = new EmptyOperator();
      eo3.Name = "Evaluator";
      op.OperatorGraph.AddOperator(eo3);
      co2.AddSubOperator(eo3);

      CombinedOperator co3 = CreateESMain();
      co3.Name = "ES Main";
      op.OperatorGraph.AddOperator(co3);
      sp.AddSubOperator(co3);

      // place holder for Mutator
      EmptyOperator eo4 = new EmptyOperator();
      eo4.Name = "Mutator";
      op.OperatorGraph.AddOperator(eo4);
      co3.AddSubOperator(eo4);

      // place holder for Evaluator
      co3.AddSubOperator(eo3);

      // place holder for Recombinator
      EmptyOperator eo5 = new EmptyOperator();
      eo5.Name = "Recombinator";
      op.OperatorGraph.AddOperator(eo5);
      co3.AddSubOperator(eo5);

      return op;
    }
    private static CombinedOperator CreateVariableInjection() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp);
      op.OperatorGraph.InitialOperator = sp;

      RandomInjector ri = new RandomInjector();
      op.OperatorGraph.AddOperator(ri);
      sp.AddSubOperator(ri);

      OperatorExtractor oe = new OperatorExtractor();
      oe.Name = "ProblemInjector";
      oe.GetVariableInfo("Operator").ActualName = "ProblemInjector";
      op.OperatorGraph.AddOperator(oe);
      sp.AddSubOperator(oe);

      VariableInjector vi = new VariableInjector();
      vi.AddVariable(new Variable("ESmu", new IntData(1)));
      vi.AddVariable(new Variable("ESrho", new IntData(1)));
      vi.AddVariable(new Variable("ESlambda", new IntData(1)));
      vi.AddVariable(new Variable("EvaluatedSolutions", new IntData()));
      vi.AddVariable(new Variable("PlusNotation", new BoolData(true)));
      vi.AddVariable(new Variable("Generations", new IntData()));
      vi.AddVariable(new Variable("MaximumGenerations", new IntData(1000)));
      vi.AddVariable(new Variable("LearningRate", new DoubleData(0.1)));
      vi.AddVariable(new Variable("DampeningFactor", new DoubleData(10.0)));
      vi.AddVariable(new Variable("ShakingFactor", new DoubleData(5.0)));
      vi.AddVariable(new Variable("TargetSuccessProbability", new DoubleData(0.2)));
      vi.AddVariable(new Variable("SuccessProbability", new DoubleData(0.2)));
      vi.AddVariable(new Variable("UseSuccessRule", new BoolData(true)));
      op.OperatorGraph.AddOperator(vi);
      sp.AddSubOperator(vi);

      return op;
    }
    private static CombinedOperator CreatePopulationInitialization() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      op.OperatorGraph.InitialOperator = sp1;

      SubScopesCreater ssc = new SubScopesCreater();
      ssc.GetVariableInfo("SubScopes").ActualName = "ESmu";
      op.OperatorGraph.AddOperator(ssc);
      sp1.AddSubOperator(ssc);

      UniformSequentialSubScopesProcessor ussp = new UniformSequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ussp);
      sp1.AddSubOperator(ussp);

      SequentialProcessor sp2 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp2);
      ussp.AddSubOperator(sp2);

      OperatorExtractor oe1 = new OperatorExtractor();
      oe1.Name = "SolutionGenerator";
      oe1.GetVariableInfo("Operator").ActualName = "SolutionGenerator";
      op.OperatorGraph.AddOperator(oe1);
      sp2.AddSubOperator(oe1);

      OperatorExtractor oe2 = new OperatorExtractor();
      oe2.Name = "Evaluator";
      oe2.GetVariableInfo("Operator").ActualName = "Evaluator";
      op.OperatorGraph.AddOperator(oe2);
      sp2.AddSubOperator(oe2);

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";
      op.OperatorGraph.AddOperator(c);
      sp2.AddSubOperator(c);

      Sorter s = new Sorter();
      s.GetVariableInfo("Descending").ActualName = "Maximization";
      s.GetVariableInfo("Value").ActualName = "Quality";
      op.OperatorGraph.AddOperator(s);
      sp1.AddSubOperator(s);

      return op;
    }
    private static CombinedOperator CreateESMain() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp);
      op.OperatorGraph.InitialOperator = sp;

      ESRandomSelector rs = new ESRandomSelector();
      rs.Name = "Child Selector";
      rs.GetVariableInfo("Lambda").ActualName = "ESlambda";
      rs.GetVariableInfo("Rho").ActualName = "ESrho";
      op.OperatorGraph.AddOperator(rs);
      sp.AddSubOperator(rs);

      SequentialSubScopesProcessor ssp = new SequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ssp);
      sp.AddSubOperator(ssp);

      EmptyOperator eo = new EmptyOperator();
      op.OperatorGraph.AddOperator(eo);
      ssp.AddSubOperator(eo);

      CombinedOperator co1 = CreateCreateChildren();
      co1.Name = "Create Children";
      op.OperatorGraph.AddOperator(co1);
      ssp.AddSubOperator(co1);

      ConditionalBranch cb1 = new ConditionalBranch();
      cb1.Name = "Plus or Comma Replacement";
      cb1.GetVariableInfo("Condition").ActualName = "PlusNotation";
      op.OperatorGraph.AddOperator(cb1);
      sp.AddSubOperator(cb1);

      MergingReducer mr = new MergingReducer();
      mr.Name = "Plus Replacement";
      op.OperatorGraph.AddOperator(mr);
      cb1.AddSubOperator(mr);

      RightReducer rr = new RightReducer();
      rr.Name = "Comma Replacement";
      op.OperatorGraph.AddOperator(rr);
      cb1.AddSubOperator(rr);

      CombinedOperator co2 = CreateReplacement();
      co2.Name = "Parents Selection";
      op.OperatorGraph.AddOperator(co2);
      sp.AddSubOperator(co2);

      QualityLogger ql = new QualityLogger();
      op.OperatorGraph.AddOperator(ql);
      sp.AddSubOperator(ql);

      BestAverageWorstQualityCalculator bawqc = new BestAverageWorstQualityCalculator();
      op.OperatorGraph.AddOperator(bawqc);
      sp.AddSubOperator(bawqc);

      DataCollector dc = new DataCollector();
      ItemList<StringData> names = dc.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      names.Add(new StringData("AverageQuality"));
      names.Add(new StringData("WorstQuality"));
      op.OperatorGraph.AddOperator(dc);
      sp.AddSubOperator(dc);

      LinechartInjector lci = new LinechartInjector();
      lci.GetVariableInfo("Linechart").ActualName = "Quality Linechart";
      lci.GetVariable("NumberOfLines").GetValue<IntData>().Data = 3;
      op.OperatorGraph.AddOperator(lci);
      sp.AddSubOperator(lci);

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "Generations";
      op.OperatorGraph.AddOperator(c);
      sp.AddSubOperator(c);

      LessThanComparator ltc = new LessThanComparator();
      ltc.GetVariableInfo("LeftSide").ActualName = "Generations";
      ltc.GetVariableInfo("RightSide").ActualName = "MaximumGenerations";
      ltc.GetVariableInfo("Result").ActualName = "GenerationsCondition";
      op.OperatorGraph.AddOperator(ltc);
      sp.AddSubOperator(ltc);

      ConditionalBranch cb2 = new ConditionalBranch();
      cb2.GetVariableInfo("Condition").ActualName = "GenerationsCondition";
      op.OperatorGraph.AddOperator(cb2);
      sp.AddSubOperator(cb2);

      cb2.AddSubOperator(sp);

      return op;
    }
    private static CombinedOperator CreateCreateChildren() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      op.OperatorGraph.InitialOperator = sp1;

      ConditionalBranch cb = new ConditionalBranch();
      cb.GetVariableInfo("Condition").ActualName = "UseSuccessRule";
      op.OperatorGraph.AddOperator(cb);
      sp1.AddSubOperator(cb);

      SequentialProcessor sp2 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp2);
      cb.AddSubOperator(sp2);

      OffspringAnalyzer oa = new OffspringAnalyzer();
      oa.Name = "Offspring Analyzer";
      oa.GetVariable("ParentsCount").Value = new IntData(1);
      oa.GetVariable("ComparisonFactor").Value = new DoubleData(0.0);
      op.OperatorGraph.AddOperator(oa);
      sp2.AddSubOperator(oa);

      SequentialProcessor sp3 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp3);
      oa.AddSubOperator(sp3);
      cb.AddSubOperator(sp3);

      OperatorExtractor oe1 = new OperatorExtractor();
      oe1.Name = "Recombinator";
      oe1.GetVariableInfo("Operator").ActualName = "Recombinator";
      op.OperatorGraph.AddOperator(oe1);
      sp3.AddSubOperator(oe1);

      UniformSequentialSubScopesProcessor ussp = new UniformSequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ussp);
      sp3.AddSubOperator(ussp);

      SequentialProcessor sp4 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp4);
      ussp.AddSubOperator(sp4);

      OperatorExtractor oe2 = new OperatorExtractor();
      oe2.Name = "Mutator";
      oe2.GetVariableInfo("Operator").ActualName = "Mutator";
      op.OperatorGraph.AddOperator(oe2);
      sp4.AddSubOperator(oe2);

      OperatorExtractor oe3 = new OperatorExtractor();
      oe3.Name = "Evaluator";
      oe3.GetVariableInfo("Operator").ActualName = "Evaluator";
      op.OperatorGraph.AddOperator(oe3);
      sp4.AddSubOperator(oe3);

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";
      op.OperatorGraph.AddOperator(c);
      sp4.AddSubOperator(c);

      SuccessRuleMutationStrengthAdjuster srmsa = new SuccessRuleMutationStrengthAdjuster();
      srmsa.Name = "SuccessRuleMutationStrengthAdjuster";
      op.OperatorGraph.AddOperator(srmsa);
      sp2.AddSubOperator(srmsa);

      Sorter s = new Sorter();
      s.GetVariableInfo("Value").ActualName = "Quality";
      s.GetVariableInfo("Descending").ActualName = "Maximization";
      op.OperatorGraph.AddOperator(s);
      sp1.AddSubOperator(s);

      return op;
    }
    private static CombinedOperator CreateReplacement() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      op.OperatorGraph.InitialOperator = sp1;

      Sorter s = new Sorter();
      s.GetVariableInfo("Descending").ActualName = "Maximization";
      s.GetVariableInfo("Value").ActualName = "Quality";
      op.OperatorGraph.AddOperator(s);
      sp1.AddSubOperator(s);

      LeftSelector ls = new LeftSelector();
      ls.Name = "Parents Selector";
      ls.GetVariableInfo("Selected").ActualName = "ESmu";
      ls.GetVariable("CopySelected").Value = new BoolData(false);
      op.OperatorGraph.AddOperator(ls);
      sp1.AddSubOperator(ls);

      RightReducer rr = new RightReducer();
      rr.Name = "RightReducer";
      op.OperatorGraph.AddOperator(rr);
      sp1.AddSubOperator(rr);

      return op;
    }
    #endregion

    #region Properties
    private IEngine myEngine;
    /// <summary>
    /// Gets the engine of the current instance.
    /// </summary>
    public IEngine Engine {
      get { return myEngine; }
    }
    private BoolData mySetSeedRandomly;
    /// <summary>
    /// Gets or sets the boolean flag whether to set the seed randomly or not.
    /// </summary>
    public bool SetSeedRandomly {
      get { return mySetSeedRandomly.Data; }
      set { mySetSeedRandomly.Data = value; }
    }
    private IntData mySeed;
    /// <summary>
    /// Gets or sets the seed of the current instance.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ItemBase"/> 
    /// in the setter.</remarks>
    public int Seed {
      get { return mySeed.Data; }
      set {
        mySeed.Data = value;
        OnChanged();
      }
    }
    private IntData myMu;
    /// <summary>
    /// Gets or sets the µ value of the current instance.
    /// </summary>
    /// <remarks>Sets also the lambda and the rho value. Calls <see cref="ItemBase.OnChanged"/> of
    /// base class <see cref="ItemBase"/>.</remarks>
    public int Mu {
      get { return myMu.Data; }
      set {
        if (value > 0) {
          myMu.Data = value;
          if (!PlusNotation && value >= Lambda) myLambda.Data = value + 1;
          if (value < Rho) myRho.Data = value;
          OnChanged();
        }
      }
    }
    private IntData myRho;
    /// <summary>
    /// Gets or sets the rho value of the current instance.
    /// </summary>
    /// <remarks>Sets also the µ value. Calls <see cref="ItemBase.OnChanged"/> of
    /// base class <see cref="ItemBase"/>.</remarks>
    public int Rho {
      get { return myRho.Data; }
      set {
        if (value > 0) {
          myRho.Data = value;
          if (value > Mu) Mu = value;
          OnChanged();
        }
      }
    }
    private IntData myLambda;
    /// <summary>
    /// Gets or sets the lambda value of the current instance.
    /// </summary>
    /// <remarks>May also change the µ value under certain circumstances. 
    /// Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ItemBase"/>.</remarks>
    public int Lambda {
      get { return myLambda.Data; }
      set {
        if (value > 0) {
          if (PlusNotation) myLambda.Data = value;
          else {
            if (value > 1 && value < Mu) {
              myLambda.Data = value;
              myMu.Data = value - 1;
            } else if (value == 1) {
              myMu.Data = 1;
              myLambda.Data = 2;
            } else if (value > Mu) {
              myLambda.Data = value;
            }
          }
          OnChanged();
        }
      }
    }
    private BoolData myPlusNotation;
    /// <summary>
    /// Gets or sets the boolean flag whether it is a plus notation or not.
    /// </summary>
    /// <remarks>May also set the lambda value under certain circumstances. 
    /// Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ItemBase"/>.</remarks>
    public bool PlusNotation {
      get { return myPlusNotation.Data; }
      set {
        if (!value && myPlusNotation.Data) { // from plus to point
          if (Lambda <= Mu) {
            myLambda.Data = Mu + 1;
          }
        }
        myPlusNotation.Data = value;
        OnChanged();
      }
    }
    private DoubleData myShakingFactor;
    /// <summary>
    /// Gets or sets the shaking factor of the current instance.
    /// </summary>
    public double ShakingFactor {
      get { return myShakingFactor.Data; }
      set { myShakingFactor.Data = value; }
    }
    private DoubleData mySuccessProbability;
    
    private DoubleData myTargetSuccessProbability;
    /// <summary>
    /// Gets or sets the success probability.
    /// </summary>
    /// <remarks>Gets the target success probability and sets also the target success probability.</remarks>
    public double SuccessProbability {
      get { return myTargetSuccessProbability.Data; }
      set {
        myTargetSuccessProbability.Data = value;
        mySuccessProbability.Data = value;
      }
    }
    private DoubleData myLearningRate;
    /// <summary>
    /// Gets or sets the learning rate.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ItemBase"/> 
    /// in the setter.</remarks>
    public double LearningRate {
      get { return myLearningRate.Data; }
      set {
        if (value > 0.0 && value <= 1.0) {
          myLearningRate.Data = value;
          OnChanged();
        }
      }
    }
    private DoubleData myDampeningFactor;
    /// <summary>
    /// Gets or sets the dampening factor.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ItemBase"/> 
    /// in the setter.</remarks>
    public double DampeningFactor {
      get { return myDampeningFactor.Data; }
      set {
        if (value >= 1.0) {
          myDampeningFactor.Data = value;
          OnChanged();
        }
      }
    }
    private IntData myMaximumGenerations;
    /// <summary>
    /// Gets or sets the maximum number of generations.
    /// </summary>
    public int MaximumGenerations {
      get { return myMaximumGenerations.Data; }
      set { myMaximumGenerations.Data = value; }
    }
    private BoolData myUseSuccessRule;
    /// <summary>
    /// Gets or sets the boolean flag whether to use the success rule or not.
    /// </summary>
    public bool UseSuccessRule {
      get { return myUseSuccessRule.Data; }
      set { myUseSuccessRule.Data = value; }
    }
    private CombinedOperator myES;
    private IOperator myESMain;
    private IOperator myVariableInjection;
    /// <summary>
    /// Gets or sets the problem injection operator.
    /// </summary>
    public IOperator ProblemInjector {
      get { return myVariableInjection.SubOperators[0]; }
      set {
        value.Name = "ProblemInjector";
        myES.OperatorGraph.RemoveOperator(ProblemInjector.Guid);
        myES.OperatorGraph.AddOperator(value);
        myVariableInjection.AddSubOperator(value, 0);
      }
    }
    private IOperator myPopulationInitialization;
    /// <summary>
    /// Gets or sets the solution generation operator.
    /// </summary>
    public IOperator SolutionGenerator {
      get { return myPopulationInitialization.SubOperators[0]; }
      set {
        value.Name = "SolutionGenerator";
        myES.OperatorGraph.RemoveOperator(SolutionGenerator.Guid);
        myES.OperatorGraph.AddOperator(value);
        myPopulationInitialization.AddSubOperator(value, 0);
      }
    }
    /// <summary>
    /// Gets or sets the evaluation operator.
    /// </summary>
    public IOperator Evaluator {
      get { return myPopulationInitialization.SubOperators[1]; }
      set {
        value.Name = "Evaluator";
        myES.OperatorGraph.RemoveOperator(Evaluator.Guid);
        myES.OperatorGraph.AddOperator(value);
        myPopulationInitialization.AddSubOperator(value, 1);
        myESMain.AddSubOperator(value, 1);
      }
    }
    /// <summary>
    /// Gets or sets the mutation operator. 
    /// </summary>
    public IOperator Mutator {
      get { return myESMain.SubOperators[0]; }
      set {
        value.Name = "Mutator";
        myES.OperatorGraph.RemoveOperator(Mutator.Guid);
        myES.OperatorGraph.AddOperator(value);
        myESMain.AddSubOperator(value, 0);
      }
    }
    /// <summary>
    /// Gets or sets the recombination operator.
    /// </summary>
    public IOperator Recombinator {
      get { return myESMain.SubOperators[2]; }
      set {
        value.Name = "Recombinator";
        myES.OperatorGraph.RemoveOperator(Recombinator.Guid);
        myES.OperatorGraph.AddOperator(value);
        myESMain.AddSubOperator(value, 2);
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of <see cref="ES"/> having a <see cref="SequentialEngine"/> as engine.
    /// </summary>
    public ES() {
      myEngine = new SequentialEngine.SequentialEngine();
      Create(myEngine);
      SetReferences();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ESEditor"/> to display the current instance.
    /// </summary>
    /// <returns>The created view as <see cref="ESEditor"/>.</returns>
    public override IView CreateView() {
      return new ESEditor(this);
    }
    /// <summary>
    /// Creates a new instance of <see cref="ESEditor"/> to display the current instance.
    /// </summary>
    /// <returns>The created editor as <see cref="ESEditor"/>.</returns>
    public virtual IEditor CreateEditor() {
      return new ESEditor(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Uses <see cref="Auxiliary.Clone"/> method of the <see cref="Auxiliary"/> class to 
    /// clone the engine.</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="ES"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ES clone = new ES();
      clonedObjects.Add(Guid, clone);
      clone.myEngine = (IEngine)Auxiliary.Clone(Engine, clonedObjects);
      return clone;
    }

    #region SetReferences Method
    private void SetReferences() {
      // ES
      CombinedOperator co1 = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      myES = co1;
      // SequentialProcessor in ES
      SequentialProcessor sp1 = (SequentialProcessor)co1.OperatorGraph.InitialOperator;
      // Variable Injection
      CombinedOperator co2 = (CombinedOperator)sp1.SubOperators[0];
      myVariableInjection = co2;
      // SequentialProcessor in Variable Injection
      SequentialProcessor sp2 = (SequentialProcessor)co2.OperatorGraph.InitialOperator;
      // RandomInjector
      RandomInjector ri = (RandomInjector)sp2.SubOperators[0];
      mySetSeedRandomly = ri.GetVariable("SetSeedRandomly").GetValue<BoolData>();
      mySeed = ri.GetVariable("Seed").GetValue<IntData>();
      // VariableInjector
      VariableInjector vi = (VariableInjector)sp2.SubOperators[2];
      myMu = vi.GetVariable("ESmu").GetValue<IntData>();
      myRho = vi.GetVariable("ESrho").GetValue<IntData>();
      myLambda = vi.GetVariable("ESlambda").GetValue<IntData>();
      myMaximumGenerations = vi.GetVariable("MaximumGenerations").GetValue<IntData>();
      myPlusNotation = vi.GetVariable("PlusNotation").GetValue<BoolData>();
      myShakingFactor = vi.GetVariable("ShakingFactor").GetValue<DoubleData>();
      myTargetSuccessProbability = vi.GetVariable("TargetSuccessProbability").GetValue<DoubleData>();
      mySuccessProbability = vi.GetVariable("SuccessProbability").GetValue<DoubleData>();
      myLearningRate = vi.GetVariable("LearningRate").GetValue<DoubleData>();
      myDampeningFactor = vi.GetVariable("DampeningFactor").GetValue<DoubleData>();
      myUseSuccessRule = vi.GetVariable("UseSuccessRule").GetValue<BoolData>();
      // Population Initialization
      CombinedOperator co3 = (CombinedOperator)sp1.SubOperators[1];
      myPopulationInitialization = co3;
      // ES Main
      CombinedOperator co4 = (CombinedOperator)sp1.SubOperators[2];
      myESMain = co4;
    }
    #endregion

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.GetXmlNode"/> of base class <see cref="ItemBase"/>. 
    /// <br/>The engine of the current instance is saved as child node with tag name <c>Engine</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Engine", Engine, document, persistedObjects));
      return node;
    }
    /// <summary>
    /// Loads the persisted ES from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.<br/>
    /// The engine must be saved as child node with tag name <c>Engine</c> (see 
    /// <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the value is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myEngine = (IEngine)PersistenceManager.Restore(node.SelectSingleNode("Engine"), restoredObjects);
      SetReferences();
    }
    #endregion
  }
}

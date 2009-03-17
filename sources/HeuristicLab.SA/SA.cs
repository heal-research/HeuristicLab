#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.SA {
  /// <summary>
  /// Class for the heuristic optimization technique "simulated annealing".
  /// </summary>
  public class SA : ItemBase, IEditable {
    #region Create Operators
    /// <summary>
    /// Creates operators for the current instance.
    /// </summary>
    /// <param name="engine">The engine where to add the operators.</param>
    public static void Create(IEngine engine) {
      engine.OperatorGraph.Clear();

      CombinedOperator co = CreateSA();
      co.Name = "SA";
      engine.OperatorGraph.AddOperator(co);
      engine.OperatorGraph.InitialOperator = co;

      engine.Reset();
    }
    private static CombinedOperator CreateSA() {
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

      CombinedOperator co2 = CreateSolutionInitialization();
      co2.Name = "Solution Initialization";
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

      CombinedOperator co3 = CreateSAMain();
      co3.Name = "SA Main";
      op.OperatorGraph.AddOperator(co3);
      sp.AddSubOperator(co3);

      EmptyOperator eo4 = new EmptyOperator();
      eo4.Name = "AnnealingScheme";
      op.OperatorGraph.AddOperator(eo4);
      co3.AddSubOperator(eo4);

      // place holder for Mutator
      EmptyOperator eo5 = new EmptyOperator();
      eo5.Name = "Mutator";
      op.OperatorGraph.AddOperator(eo5);
      co3.AddSubOperator(eo5);

      // place holder for Evaluator
      co3.AddSubOperator(eo3);

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
      vi.AddVariable(new Variable("EvaluatedSolutions", new IntData(0)));
      vi.AddVariable(new Variable("Iteration", new IntData(0)));
      vi.AddVariable(new Variable("MaximumIterations", new IntData(1000)));
      vi.AddVariable(new Variable("Temperature", new DoubleData(100)));
      vi.AddVariable(new Variable("MinimumTemperature", new DoubleData(0)));
      vi.AddVariable(new Variable("AnnealingParameter", new DoubleData(0.95)));
      vi.AddVariable(new Variable("SuccessRatioLimit", new DoubleData(1)));
      vi.AddVariable(new Variable("MaximumIterationEffort", new DoubleData(1)));
      op.OperatorGraph.AddOperator(vi);
      sp.AddSubOperator(vi);

      return op;
    }
    private static CombinedOperator CreateSolutionInitialization() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      op.OperatorGraph.InitialOperator = sp1;

      SubScopesCreater ssc = new SubScopesCreater();
      ssc.GetVariableInfo("SubScopes").Local = true;
      ssc.AddVariable(new Variable("SubScopes", new IntData(1)));
      op.OperatorGraph.AddOperator(ssc);
      sp1.AddSubOperator(ssc);

      UniformSequentialSubScopesProcessor ussp = new UniformSequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ussp);
      sp1.AddSubOperator(ussp);

      SequentialProcessor sp2 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp2);
      ussp.AddSubOperator(sp2);

      OperatorExtractor oe1 = new OperatorExtractor();
      oe1.Name = "SolutionGenerator (Extractor)";
      oe1.GetVariableInfo("Operator").ActualName = "SolutionGenerator";
      op.OperatorGraph.AddOperator(oe1);
      sp2.AddSubOperator(oe1);

      OperatorExtractor oe2 = new OperatorExtractor();
      oe2.Name = "Evaluator (Extractor)";
      oe2.GetVariableInfo("Operator").ActualName = "Evaluator";
      op.OperatorGraph.AddOperator(oe2);
      sp2.AddSubOperator(oe2);

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";
      op.OperatorGraph.AddOperator(c);
      sp2.AddSubOperator(c);

      return op;
    }
    private static CombinedOperator CreateSAMain() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp);
      op.OperatorGraph.InitialOperator = sp;

      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      sp.AddSubOperator(sp1);

      LeftSelector ls = new LeftSelector();
      ls.GetVariable("CopySelected").GetValue<BoolData>().Data = true;
      ls.GetVariableInfo("Selected").Local = true;
      ls.AddVariable(new Variable("Selected", new IntData(1)));
      op.OperatorGraph.AddOperator(ls);
      sp1.AddSubOperator(ls);

      SequentialSubScopesProcessor ssp = new SequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ssp);
      sp1.AddSubOperator(ssp);

      EmptyOperator eo = new EmptyOperator();
      op.OperatorGraph.AddOperator(eo);
      ssp.AddSubOperator(eo);

      CombinedOperator co1 = CreateCreateMutant();
      co1.Name = "Create Mutant";
      op.OperatorGraph.AddOperator(co1);
      ssp.AddSubOperator(co1);

      OffspringSelector oss = new OffspringSelector();
      oss.GetVariableInfo("SelectionPressureLimit").ActualName = "MaximumIterationEffort";
      op.OperatorGraph.AddOperator(oss);
      sp1.AddSubOperator(oss);
      oss.AddSubOperator(sp1);

      LessThanComparator ltc = new LessThanComparator();
      ltc.Name = "SuccessRatio";
      ltc.GetVariableInfo("LeftSide").ActualName = "SuccessRatio";
      ltc.GetVariableInfo("RightSide").ActualName = "SuccessRatioLimit";
      ltc.GetVariableInfo("Result").ActualName = "SuccessfulOS";
      op.OperatorGraph.AddOperator(ltc);
      sp.AddSubOperator(ltc);

      ConditionalBranch cb = new ConditionalBranch();
      cb.GetVariableInfo("Condition").ActualName = "SuccessfulOS";
      op.OperatorGraph.AddOperator(cb);
      sp.AddSubOperator(cb);

      LeftReducer lr = new LeftReducer();
      lr.Name = "No Replacement";
      op.OperatorGraph.AddOperator(lr);
      cb.AddSubOperator(lr);

      RightReducer rr = new RightReducer();
      rr.Name = "Replacement";
      op.OperatorGraph.AddOperator(rr);
      cb.AddSubOperator(rr);

      QualityLogger ql = new QualityLogger();
      op.OperatorGraph.AddOperator(ql);
      sp.AddSubOperator(ql);

      BestAverageWorstQualityCalculator bawqc = new BestAverageWorstQualityCalculator();
      bawqc.GetVariableInfo("AverageQuality").Local = true;
      bawqc.GetVariableInfo("WorstQuality").Local = true;
      op.OperatorGraph.AddOperator(bawqc);
      sp.AddSubOperator(bawqc);

      DataCollector dc = new DataCollector();
      ItemList<StringData> names = dc.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      op.OperatorGraph.AddOperator(dc);
      sp.AddSubOperator(dc);

      LinechartInjector lci = new LinechartInjector();
      lci.GetVariableInfo("Linechart").ActualName = "Quality Linechart";
      lci.GetVariable("NumberOfLines").GetValue<IntData>().Data = 1;
      op.OperatorGraph.AddOperator(lci);
      sp.AddSubOperator(lci);

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "Iteration";
      op.OperatorGraph.AddOperator(c);
      sp.AddSubOperator(c);

      VariableAssigner va = new VariableAssigner();
      va.Name = "Reset SelectionPressure";
      va.GetVariableInfo("Variable").ActualName = "SelectionPressure";
      va.GetVariableInfo("Value").Local = true;
      va.AddVariable(new Variable("Value", new DoubleData(1)));
      op.OperatorGraph.AddOperator(va);
      sp.AddSubOperator(va);

      OperatorExtractor oe = new OperatorExtractor();
      oe.Name = "AnnealingScheme (Extractor)";
      oe.GetVariableInfo("Operator").ActualName = "AnnealingScheme";
      op.OperatorGraph.AddOperator(oe);
      sp.AddSubOperator(oe);

      GreaterThanComparator gtc = new GreaterThanComparator();
      gtc.Name = "Temperature > MinimumTemperature";
      gtc.GetVariableInfo("LeftSide").ActualName = "Temperature";
      gtc.GetVariableInfo("RightSide").ActualName = "MinimumTemperature";
      gtc.GetVariableInfo("Result").ActualName = "TemperatureCondition";
      op.OperatorGraph.AddOperator(gtc);
      sp.AddSubOperator(gtc);

      ConditionalBranch cb1 = new ConditionalBranch();
      cb1.GetVariableInfo("Condition").ActualName = "TemperatureCondition";
      op.OperatorGraph.AddOperator(cb1);
      sp.AddSubOperator(cb1);

      SequentialProcessor sp2 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp2);
      cb1.AddSubOperator(sp2);

      LessThanComparator ltc1 = new LessThanComparator();
      ltc1.Name = "Iteration < MaximumIterations";
      ltc1.GetVariableInfo("LeftSide").ActualName = "Iteration";
      ltc1.GetVariableInfo("RightSide").ActualName = "MaximumIterations";
      ltc1.GetVariableInfo("Result").ActualName = "IterationsCondition";
      op.OperatorGraph.AddOperator(ltc1);
      sp2.AddSubOperator(ltc1);

      ConditionalBranch cb2 = new ConditionalBranch();
      cb2.GetVariableInfo("Condition").ActualName = "IterationsCondition";
      op.OperatorGraph.AddOperator(cb2);
      sp2.AddSubOperator(cb2);

      cb2.AddSubOperator(sp);

      return op;
    }
    private static CombinedOperator CreateCreateMutant() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      op.OperatorGraph.InitialOperator = sp1;

      ChildrenInitializer ci = new ChildrenInitializer();
      ci.GetVariable("ParentsPerChild").GetValue<IntData>().Data = 1;
      op.OperatorGraph.AddOperator(ci);
      sp1.AddSubOperator(ci);

      UniformSequentialSubScopesProcessor ussp = new UniformSequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ussp);
      sp1.AddSubOperator(ussp);

      SequentialProcessor sp2 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp2);
      ussp.AddSubOperator(sp2);

      VariablesCopier vc = new VariablesCopier();
      op.OperatorGraph.AddOperator(vc);
      sp2.AddSubOperator(vc);

      OperatorExtractor oe = new OperatorExtractor();
      oe.Name = "Mutator (Extractor)";
      oe.GetVariableInfo("Operator").ActualName = "Mutator";
      op.OperatorGraph.AddOperator(oe);
      sp2.AddSubOperator(oe);

      OperatorExtractor oe1 = new OperatorExtractor();
      oe1.Name = "Evaluator (Extractor)";
      oe1.GetVariableInfo("Operator").ActualName = "Evaluator";
      op.OperatorGraph.AddOperator(oe1);
      sp2.AddSubOperator(oe1);

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";
      op.OperatorGraph.AddOperator(c);
      sp2.AddSubOperator(c);

      TemperatureBasedFitnessComparer tbfc = new TemperatureBasedFitnessComparer();
      op.OperatorGraph.AddOperator(tbfc);
      sp2.AddSubOperator(tbfc);

      SubScopesRemover sr = new SubScopesRemover();
      sr.GetVariableInfo("SubScopeIndex").Local = true;
      op.OperatorGraph.AddOperator(sr);
      sp2.AddSubOperator(sr);

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
    /// Gets or sets the flag whether to set the seed randomly or not.
    /// </summary>
    public bool SetSeedRandomly {
      get { return mySetSeedRandomly.Data; }
      set { mySetSeedRandomly.Data = value; }
    }
    private IntData mySeed;
    /// <summary>
    /// Gets or sets the value of the seed of the current instance.
    /// </summary>
    public int Seed {
      get { return mySeed.Data; }
      set { mySeed.Data = value; }
    }
    private IntData myMaximumIterations;
    /// <summary>
    /// Gets or sets the number of maximum iterations.
    /// </summary>
    public int MaximumIterations {
      get { return myMaximumIterations.Data; }
      set { myMaximumIterations.Data = value; }
    }
    private DoubleData myTemperature;
    /// <summary>
    /// Gets or sets the initial temperature.
    /// </summary>
    public double Temperature {
      get { return myTemperature.Data; }
      set { myTemperature.Data = value; }
    }
    private DoubleData myMinimumTemperature;
    /// <summary>
    /// Gets or sets the minimum temperature.
    /// </summary>
    public double MinimumTemperature {
      get { return myMinimumTemperature.Data; }
      set { myMinimumTemperature.Data = value; }
    }
    private DoubleData myAnnealingParameter;
    /// <summary>
    /// Gets or sets the annealing parameter.
    /// </summary>
    public double AnnealingParameter {
      get { return myAnnealingParameter.Data; }
      set { myAnnealingParameter.Data = value; }
    }
    private DoubleData myMaximumIterationEffort;
    /// <summary>
    /// Gets or sets how much solutions should be created in one temperature in attempt to find a better one.
    /// </summary>
    public double MaximumIterationEffort {
      get { return myMaximumIterationEffort.Data; }
      set { myMaximumIterationEffort.Data = value; }
    }
    private CombinedOperator mySA;
    private IOperator myVariableInjection;
    /// <summary>
    /// Gets or sets the problem injector of the current instance.
    /// </summary>
    public IOperator ProblemInjector {
      get { return myVariableInjection.SubOperators[0]; }
      set {
        value.Name = "ProblemInjector";
        mySA.OperatorGraph.RemoveOperator(ProblemInjector.Guid);
        mySA.OperatorGraph.AddOperator(value);
        myVariableInjection.AddSubOperator(value, 0);
      }
    }
    private IOperator myPopulationInitialization;
    /// <summary>
    /// Gets or sets the solution generator of the current instance.
    /// </summary>
    public IOperator SolutionGenerator {
      get { return myPopulationInitialization.SubOperators[0]; }
      set {
        value.Name = "SolutionGenerator";
        mySA.OperatorGraph.RemoveOperator(SolutionGenerator.Guid);
        mySA.OperatorGraph.AddOperator(value);
        myPopulationInitialization.AddSubOperator(value, 0);
      }
    }
    /// <summary>
    /// Gets or sets the evaluator of the current instance.
    /// </summary>
    public IOperator Evaluator {
      get { return myPopulationInitialization.SubOperators[1]; }
      set {
        value.Name = "Evaluator";
        mySA.OperatorGraph.RemoveOperator(Evaluator.Guid);
        mySA.OperatorGraph.AddOperator(value);
        myPopulationInitialization.AddSubOperator(value, 1);
        mySAMain.AddSubOperator(value, 2);
      }
    }
    private IOperator mySAMain;
    /// <summary>
    /// Gets or sets the mutation operator of the current instance.
    /// </summary>
    public IOperator Mutator {
      get { return mySAMain.SubOperators[1]; }
      set {
        value.Name = "Mutator";
        mySA.OperatorGraph.RemoveOperator(Mutator.Guid);
        mySA.OperatorGraph.AddOperator(value);
        mySAMain.AddSubOperator(value, 1);
      }
    }
    /// <summary>
    /// Gets or sets the annealing schema to be used
    /// </summary>
    public IOperator AnnealingScheme {
      get { return mySAMain.SubOperators[0]; }
      set {
        value.Name = "AnnealingScheme";
        mySA.OperatorGraph.RemoveOperator(AnnealingScheme.Guid);
        mySA.OperatorGraph.AddOperator(value);
        mySAMain.AddSubOperator(value, 0);
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of <see cref="SA"/>.
    /// </summary>
    public SA() {
      myEngine = new SequentialEngine.SequentialEngine();
      Create(myEngine);
      SetReferences();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SAEditor"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="SAEditor"/>.</returns>
    public override IView CreateView() {
      return new SAEditor(this);
    }
    /// <summary>
    /// Creates a new instance of the <see cref="SAEditor"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="SAEditor"/>.</returns>
    public virtual IEditor CreateEditor() {
      return new SAEditor(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="SA"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SA clone = new SA();
      clonedObjects.Add(Guid, clone);
      clone.myEngine = (IEngine)Auxiliary.Clone(Engine, clonedObjects);
      return clone;
    }

    #region SetReferences Method
    private void SetReferences() {
      // SA
      CombinedOperator co1 = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      mySA = co1;
      // SequentialProcessor in SA
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
      myMaximumIterations = vi.GetVariable("MaximumIterations").GetValue<IntData>();
      myMaximumIterationEffort = vi.GetVariable("MaximumIterationEffort").GetValue<DoubleData>();
      myTemperature = vi.GetVariable("Temperature").GetValue<DoubleData>();
      myMinimumTemperature = vi.GetVariable("MinimumTemperature").GetValue<DoubleData>();
      myAnnealingParameter = vi.GetVariable("AnnealingParameter").GetValue<DoubleData>();
      // Population Initialization
      CombinedOperator co3 = (CombinedOperator)sp1.SubOperators[1];
      myPopulationInitialization = co3;
      // SA Main
      CombinedOperator co4 = (CombinedOperator)sp1.SubOperators[2];
      mySAMain = co4;
    }
    #endregion

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The engine of the current instance is saved as a child node with the tag name 
    /// <c>Engine</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Engine", Engine, document, persistedObjects));
      return node;
    }
    /// <summary>
    /// Loads the persisted instance from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The elements of the current instance must be saved in a special way, see 
    /// <see cref="GetXmlNode"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the instance is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myEngine = (IEngine)PersistenceManager.Restore(node.SelectSingleNode("Engine"), restoredObjects);
      SetReferences();
    }
    #endregion
  }
}

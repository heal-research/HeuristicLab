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

namespace HeuristicLab.SGA {
  /// <summary>
  /// Class for the heuristic optimization technique "simple genetic algorithm".
  /// </summary>
  public class SGA : ItemBase, IEditable {
    #region Create Operators
    /// <summary>
    /// Creates operators for the current instance.
    /// </summary>
    /// <param name="engine">The engine where to add the operators.</param>
    public static void Create(IEngine engine) {
      engine.OperatorGraph.Clear();

      CombinedOperator co = CreateSGA();
      co.Name = "SGA";
      engine.OperatorGraph.AddOperator(co);
      engine.OperatorGraph.InitialOperator = co;

      engine.Reset();
    }
    private static CombinedOperator CreateSGA() {
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

      CombinedOperator co3 = CreateSGAMain();
      co3.Name = "SGA Main";
      op.OperatorGraph.AddOperator(co3);
      sp.AddSubOperator(co3);

      // place holder for Selector
      EmptyOperator eo4 = new EmptyOperator();
      eo4.Name = "Selector";
      op.OperatorGraph.AddOperator(eo4);
      co3.AddSubOperator(eo4);

      // place holder for Crossover
      EmptyOperator eo5 = new EmptyOperator();
      eo5.Name = "Crossover";
      op.OperatorGraph.AddOperator(eo5);
      co3.AddSubOperator(eo5);

      // place holder for Mutator
      EmptyOperator eo6 = new EmptyOperator();
      eo6.Name = "Mutator";
      op.OperatorGraph.AddOperator(eo6);
      co3.AddSubOperator(eo6);

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
      vi.AddVariable(new Variable("PopulationSize", new IntData(100)));
      vi.AddVariable(new Variable("EvaluatedSolutions", new IntData()));
      vi.AddVariable(new Variable("Parents", new IntData(200)));
      vi.AddVariable(new Variable("MutationRate", new DoubleData(0.05)));
      vi.AddVariable(new Variable("Elites", new IntData(1)));
      vi.AddVariable(new Variable("Generations", new IntData()));
      vi.AddVariable(new Variable("MaximumGenerations", new IntData(1000)));
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
      ssc.GetVariableInfo("SubScopes").ActualName = "PopulationSize";
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
    private static CombinedOperator CreateSGAMain() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp);
      op.OperatorGraph.InitialOperator = sp;

      OperatorExtractor oe = new OperatorExtractor();
      oe.Name = "Selector";
      oe.GetVariableInfo("Operator").ActualName = "Selector";
      op.OperatorGraph.AddOperator(oe);
      sp.AddSubOperator(oe);

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

      CombinedOperator co2 = CreateReplacement();
      co2.Name = "Replacement";
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

      ConditionalBranch cb = new ConditionalBranch();
      cb.GetVariableInfo("Condition").ActualName = "GenerationsCondition";
      op.OperatorGraph.AddOperator(cb);
      sp.AddSubOperator(cb);

      cb.AddSubOperator(sp);

      return op;
    }
    private static CombinedOperator CreateCreateChildren() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      op.OperatorGraph.InitialOperator = sp1;

      OperatorExtractor oe1 = new OperatorExtractor();
      oe1.Name = "Crossover";
      oe1.GetVariableInfo("Operator").ActualName = "Crossover";
      op.OperatorGraph.AddOperator(oe1);
      sp1.AddSubOperator(oe1);

      UniformSequentialSubScopesProcessor ussp = new UniformSequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ussp);
      sp1.AddSubOperator(ussp);

      SequentialProcessor sp2 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp2);
      ussp.AddSubOperator(sp2);

      StochasticBranch hb = new StochasticBranch();
      hb.GetVariableInfo("Probability").ActualName = "MutationRate";
      op.OperatorGraph.AddOperator(hb);
      sp2.AddSubOperator(hb);

      OperatorExtractor oe2 = new OperatorExtractor();
      oe2.Name = "Mutator";
      oe2.GetVariableInfo("Operator").ActualName = "Mutator";
      op.OperatorGraph.AddOperator(oe2);
      hb.AddSubOperator(oe2);

      OperatorExtractor oe3 = new OperatorExtractor();
      oe3.Name = "Evaluator";
      oe3.GetVariableInfo("Operator").ActualName = "Evaluator";
      op.OperatorGraph.AddOperator(oe3);
      sp2.AddSubOperator(oe3);

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
    private static CombinedOperator CreateReplacement() {
      CombinedOperator op = new CombinedOperator();
      SequentialProcessor sp1 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp1);
      op.OperatorGraph.InitialOperator = sp1;

      SequentialSubScopesProcessor ssp = new SequentialSubScopesProcessor();
      op.OperatorGraph.AddOperator(ssp);
      sp1.AddSubOperator(ssp);

      SequentialProcessor sp2 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp2);
      ssp.AddSubOperator(sp2);

      LeftSelector ls = new LeftSelector();
      ls.GetVariableInfo("Selected").ActualName = "Elites";
      op.OperatorGraph.AddOperator(ls);
      sp2.AddSubOperator(ls);

      RightReducer rr = new RightReducer();
      op.OperatorGraph.AddOperator(rr);
      sp2.AddSubOperator(rr);

      SequentialProcessor sp3 = new SequentialProcessor();
      op.OperatorGraph.AddOperator(sp3);
      ssp.AddSubOperator(sp3);

      RightSelector rs = new RightSelector();
      rs.GetVariableInfo("Selected").ActualName = "Elites";
      op.OperatorGraph.AddOperator(rs);
      sp3.AddSubOperator(rs);

      LeftReducer lr = new LeftReducer();
      op.OperatorGraph.AddOperator(lr);
      sp3.AddSubOperator(lr);

      MergingReducer mr = new MergingReducer();
      op.OperatorGraph.AddOperator(mr);
      sp1.AddSubOperator(mr);

      Sorter s = new Sorter();
      s.GetVariableInfo("Descending").ActualName = "Maximization";
      s.GetVariableInfo("Value").ActualName = "Quality";
      op.OperatorGraph.AddOperator(s);
      sp1.AddSubOperator(s);

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
    private IntData myPopulationSize;
    private IntData myParents;
    /// <summary>
    /// Gets or sets the population size of the current instance.
    /// </summary>
    /// <remarks>The number of parents is set to two times the population size.</remarks>
    public int PopulationSize {
      get { return myPopulationSize.Data; }
      set {
        myPopulationSize.Data = value;
        myParents.Data = value * 2;
      }
    }
    private IntData myMaximumGenerations;
    /// <summary>
    /// Gets or sets the number of maximum generations.
    /// </summary>
    public int MaximumGenerations {
      get { return myMaximumGenerations.Data; }
      set { myMaximumGenerations.Data = value; }
    }
    private DoubleData myMutationRate;
    /// <summary>
    /// Gets or sets the mutation rate of the current instance.
    /// </summary>
    public double MutationRate {
      get { return myMutationRate.Data; }
      set { myMutationRate.Data = value; }
    }
    private IntData myElites;
    /// <summary>
    /// Gets or sets the elites of the current instance.
    /// </summary>
    public int Elites {
      get { return myElites.Data; }
      set { myElites.Data = value; }
    }
    private CombinedOperator mySGA;
    private IOperator myVariableInjection;
    /// <summary>
    /// Gets or sets the problem injector of the current instance.
    /// </summary>
    public IOperator ProblemInjector {
      get { return myVariableInjection.SubOperators[0]; }
      set {
        value.Name = "ProblemInjector";
        mySGA.OperatorGraph.RemoveOperator(ProblemInjector.Guid);
        mySGA.OperatorGraph.AddOperator(value);
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
        mySGA.OperatorGraph.RemoveOperator(SolutionGenerator.Guid);
        mySGA.OperatorGraph.AddOperator(value);
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
        mySGA.OperatorGraph.RemoveOperator(Evaluator.Guid);
        mySGA.OperatorGraph.AddOperator(value);
        myPopulationInitialization.AddSubOperator(value, 1);
        mySGAMain.AddSubOperator(value, 3);
      }
    }
    private IOperator mySGAMain;
    /// <summary>
    /// Gets or sets the selection operator of the current instance.
    /// </summary>
    public IOperator Selector {
      get { return mySGAMain.SubOperators[0]; }
      set {
        value.Name = "Selector";
        mySGA.OperatorGraph.RemoveOperator(Selector.Guid);
        mySGA.OperatorGraph.AddOperator(value);
        mySGAMain.AddSubOperator(value, 0);
      }
    }
    /// <summary>
    /// Gets or sets the crossover operator of the current instance.
    /// </summary>
    public IOperator Crossover {
      get { return mySGAMain.SubOperators[1]; }
      set {
        value.Name = "Crossover";
        mySGA.OperatorGraph.RemoveOperator(Crossover.Guid);
        mySGA.OperatorGraph.AddOperator(value);
        mySGAMain.AddSubOperator(value, 1);
      }
    }
    /// <summary>
    /// Gets or sets the mutation operator of the current instance.
    /// </summary>
    public IOperator Mutator {
      get { return mySGAMain.SubOperators[2]; }
      set {
        value.Name = "Mutator";
        mySGA.OperatorGraph.RemoveOperator(Mutator.Guid);
        mySGA.OperatorGraph.AddOperator(value);
        mySGAMain.AddSubOperator(value, 2);
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of <see cref="SGA"/>.
    /// </summary>
    public SGA() {
      myEngine = new SequentialEngine.SequentialEngine();
      Create(myEngine);
      SetReferences();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SGAEditor"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="SGAEditor"/>.</returns>
    public override IView CreateView() {
      return new SGAEditor(this);
    }
    /// <summary>
    /// Creates a new instance of the <see cref="SGAEditor"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="SGAEditor"/>.</returns>
    public virtual IEditor CreateEditor() {
      return new SGAEditor(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="SGA"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SGA clone = new SGA();
      clonedObjects.Add(Guid, clone);
      clone.myEngine = (IEngine)Auxiliary.Clone(Engine, clonedObjects);
      return clone;
    }

    #region SetReferences Method
    private void SetReferences() {
      // SGA
      CombinedOperator co1 = (CombinedOperator)Engine.OperatorGraph.InitialOperator;
      mySGA = co1;
      // SequentialProcessor in SGA
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
      myPopulationSize = vi.GetVariable("PopulationSize").GetValue<IntData>();
      myParents = vi.GetVariable("Parents").GetValue<IntData>();
      myMaximumGenerations = vi.GetVariable("MaximumGenerations").GetValue<IntData>();
      myMutationRate = vi.GetVariable("MutationRate").GetValue<DoubleData>();
      myElites = vi.GetVariable("Elites").GetValue<IntData>();
      // Population Initialization
      CombinedOperator co3 = (CombinedOperator)sp1.SubOperators[1];
      myPopulationInitialization = co3;
      // SGA Main
      CombinedOperator co4 = (CombinedOperator)sp1.SubOperators[2];
      mySGAMain = co4;
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

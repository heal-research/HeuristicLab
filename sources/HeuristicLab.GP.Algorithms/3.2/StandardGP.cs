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

using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Selection;
using HeuristicLab.Logging;
using HeuristicLab.Data;
using HeuristicLab.GP.Operators;

namespace HeuristicLab.GP.Algorithms {
  public class StandardGP : AlgorithmBase, IEditable {

    public override string Name { get { return "StandardGP"; } }
    public virtual int TournamentSize {
      get { return GetVariableInjector().GetVariable("TournamentSize").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("TournamentSize").GetValue<IntData>().Data = value; }
    }

    public double FullTreeShakingFactor {
      get { return GetVariableInjector().GetVariable("FullTreeShakingFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("FullTreeShakingFactor").GetValue<DoubleData>().Data = value; }
    }

    public double OnePointShakingFactor {
      get { return GetVariableInjector().GetVariable("OnePointShakingFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("OnePointShakingFactor").GetValue<DoubleData>().Data = value; }
    }

    public override int PopulationSize {
      get {
        return base.PopulationSize;
      }
      set {
        base.PopulationSize = value;
        Parents = 2 * value;
      }
    }

    public StandardGP()
      : base() {
      PopulationSize = 10000;
      MaxGenerations = 500;
      TournamentSize = 7;
      MutationRate = 0.15;
      Elites = 1;
      MaxTreeSize = 100;
      MaxTreeHeight = 10;
      FullTreeShakingFactor = 0.1;
      OnePointShakingFactor = 1.0;
      SetSeedRandomly = true;
    }

    protected override IOperator CreateSelectionOperator() {
      TournamentSelector selector = new TournamentSelector();
      selector.Name = "Selector";
      selector.GetVariableInfo("Selected").ActualName = "Parents";
      selector.GetVariableInfo("GroupSize").Local = false;
      selector.RemoveVariable("GroupSize");
      selector.GetVariableInfo("GroupSize").ActualName = "TournamentSize";
      return selector;
    }

    protected override VariableInjector CreateGlobalInjector() {
      VariableInjector globalInjector = base.CreateGlobalInjector();
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("TournamentSize", new IntData()));
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("FullTreeShakingFactor", new DoubleData()));
      globalInjector.AddVariable(new HeuristicLab.Core.Variable("OnePointShakingFactor", new DoubleData()));
      return globalInjector;
    }

    protected override IOperator CreateManipulationOperator() {
      CombinedOperator manipulator = new CombinedOperator();
      manipulator.Name = "Manipulator";
      StochasticMultiBranch multibranch = new StochasticMultiBranch();
      FullTreeShaker fullTreeShaker = new FullTreeShaker();
      fullTreeShaker.GetVariableInfo("ShakingFactor").ActualName = "FullTreeShakingFactor";

      OnePointShaker onepointShaker = new OnePointShaker();
      onepointShaker.GetVariableInfo("ShakingFactor").ActualName = "OnePointShakingFactor";
      ChangeNodeTypeManipulation changeNodeTypeManipulation = new ChangeNodeTypeManipulation();
      CutOutNodeManipulation cutOutNodeManipulation = new CutOutNodeManipulation();
      DeleteSubTreeManipulation deleteSubTreeManipulation = new DeleteSubTreeManipulation();
      SubstituteSubTreeManipulation substituteSubTreeManipulation = new SubstituteSubTreeManipulation();

      IOperator[] manipulators = new IOperator[] {
        onepointShaker, fullTreeShaker,
        changeNodeTypeManipulation,
        cutOutNodeManipulation,
        deleteSubTreeManipulation,
        substituteSubTreeManipulation};

      DoubleArrayData probabilities = new DoubleArrayData(new double[manipulators.Length]);
      for (int i = 0; i < manipulators.Length; i++) {
        probabilities.Data[i] = 1.0;
        multibranch.AddSubOperator(manipulators[i]);
      }
      multibranch.GetVariableInfo("Probabilities").Local = true;
      multibranch.AddVariable(new HeuristicLab.Core.Variable("Probabilities", probabilities));

      manipulator.OperatorGraph.AddOperator(multibranch);
      manipulator.OperatorGraph.InitialOperator = multibranch;
      return manipulator;
    }

    public virtual IEditor CreateEditor() {
      return new StandardGpEditor(this);
    }

    public override IView CreateView() {
      return new StandardGpEditor(this);
    }
  }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Problems.ArtificialAnt;
using HeuristicLab.Selection;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Optimization;
using System.Threading;
using HeuristicLab.ParallelEngine;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class GeneticProgrammingSamplesTest {
    [TestMethod]
    public void ArtificialAntProblemTest() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region algorithm configuration
      ga.Name = "Genetic Programming - Artificial Ant";
      ga.Description = "A standard genetic programming algorithm to solve the artificial ant problem (Santa-Fe trail)";
      ArtificialAntProblem antProblem = new ArtificialAntProblem();
      ga.Problem = antProblem;
      ga.PopulationSize.Value = 1000;
      ga.MaximumGenerations.Value = 100;
      ga.MutationProbability.Value = 0.15;
      ga.Elites.Value = 1;
      var tSelector = (TournamentSelector)ga.ValidSelectors
        .Single(s => s.Name == "TournamentSelector");
      tSelector.GroupSizeParameter.Value = new IntValue(5);
      ga.Selector = tSelector;
      var mutator = (MultiSymbolicExpressionTreeArchitectureManipulator)ga.ValidMutators
        .Single(m => m.Name == "MultiSymbolicExpressionTreeArchitectureManipulator");
      mutator.Operators.SetItemCheckedState(mutator.Operators.Single(x => x.Name == "FullTreeShaker"), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators.Single(x => x.Name == "OnePointShaker"), false);
      ga.Mutator = mutator;

      ga.SetSeedRandomly.Value = false;
      ga.Seed.Value = 0;
      ga.Engine = new ParallelEngine();
      #endregion
      #region problem configuration
      antProblem.BestKnownQuality.Value = 89;
      antProblem.MaxExpressionDepth.Value = 10;
      antProblem.MaxExpressionLength.Value = 100;
      antProblem.MaxFunctionArguments.Value = 3;
      antProblem.MaxFunctionDefinitions.Value = 3;
      antProblem.MaxTimeSteps.Value = 600;
      #endregion

      XmlGenerator.Serialize(ga, "../../SGP_SantaFe.hl");

      RunAlgorithm(ga);
    }

    private void RunAlgorithm(IAlgorithm a) {
      var trigger = new EventWaitHandle(false, EventResetMode.ManualReset);
      Exception ex = null;
      a.Stopped += (src, e) => { trigger.Set(); };
      a.ExceptionOccurred += (src, e) => { ex = e.Value; };
      a.Prepare();
      a.Start();
      trigger.WaitOne();

      Assert.AreEqual(ex, null);
    }
  }
}

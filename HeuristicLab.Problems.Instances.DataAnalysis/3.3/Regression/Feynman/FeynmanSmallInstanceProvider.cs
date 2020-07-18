using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanSmallInstanceProvider : ArtificialRegressionInstanceProvider {
    public override string Name {
      get { return "'AI Feynman' Benchmark Problems (small)"; }
    }

    public override string Description {
      get { return "Problem instances from \"AI Feynman\" paper (Feynman Symbolic Regression Database)"; }
    }

    public override Uri WebLink {
      get { return new Uri("https://space.mit.edu/home/tegmark/aifeynman.html"); }
    }

    public override string ReferencePublication {
      get { return "Udrescu, Silviu-Marian, and Max Tegmark. \"AI Feynman: A physics-inspired method for symbolic regression.\" Science Advances 6.16 (2020): eaay2631."; }
    }

    public int Seed { get; private set; }

    public FeynmanSmallInstanceProvider() : this((int)DateTime.Now.Ticks) { }

    public FeynmanSmallInstanceProvider(int seed) {
      Seed = seed;
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var descriptorList = new List<IDataDescriptor>();
      var rand = new MersenneTwister((uint)Seed);


      var noiseRatio = new double?[] { null, 1, 10E-2, 10E-4 };

      #region types
      var descriptorTypes = new Type[] {
        typeof(Feynman1),
        typeof(Feynman2),
        typeof(Feynman3),
        typeof(Feynman4),
        typeof(Feynman5),
        typeof(Feynman6),
        typeof(Feynman7),
        typeof(Feynman8),
        typeof(Feynman9),
        typeof(Feynman10),
        typeof(Feynman11),
        typeof(Feynman12),
        typeof(Feynman13),
        typeof(Feynman14),
        typeof(Feynman15),
        typeof(Feynman16),
        typeof(Feynman17),
        typeof(Feynman18),
        typeof(Feynman19),
        typeof(Feynman20),
        typeof(Feynman21),
        typeof(Feynman22),
        typeof(Feynman23),
        typeof(Feynman24),
        typeof(Feynman25),
        typeof(Feynman26),
        typeof(Feynman27),
        typeof(Feynman28),
        typeof(Feynman29),
        typeof(Feynman30),
        typeof(Feynman31),
        typeof(Feynman32),
        typeof(Feynman33),
        typeof(Feynman34),
        typeof(Feynman35),
        typeof(Feynman36),
        typeof(Feynman37),
        typeof(Feynman38),
        typeof(Feynman39),
        typeof(Feynman40),
        typeof(Feynman41),
        typeof(Feynman42),
        typeof(Feynman43),
        typeof(Feynman44),
        typeof(Feynman45),
        typeof(Feynman46),
        typeof(Feynman47),
        typeof(Feynman48),
        typeof(Feynman49),
        typeof(Feynman50),
        typeof(Feynman51),
        typeof(Feynman52),
        typeof(Feynman53),
        typeof(Feynman54),
        typeof(Feynman55),
        typeof(Feynman56),
        typeof(Feynman57),
        typeof(Feynman58),
        typeof(Feynman59),
        typeof(Feynman60),
        typeof(Feynman61),
        typeof(Feynman62),
        typeof(Feynman63),
        typeof(Feynman64),
        typeof(Feynman65),
        typeof(Feynman66),
        typeof(Feynman67),
        typeof(Feynman68),
        typeof(Feynman69),
        typeof(Feynman70),
        typeof(Feynman71),
        typeof(Feynman72),
        typeof(Feynman73),
        typeof(Feynman74),
        typeof(Feynman75),
        typeof(Feynman76),
        typeof(Feynman77),
        typeof(Feynman78),
        typeof(Feynman79),
        typeof(Feynman80),
        typeof(Feynman81),
        typeof(Feynman82),
        typeof(Feynman83),
        typeof(Feynman84),
        typeof(Feynman85),
        typeof(Feynman86),
        typeof(Feynman87),
        typeof(Feynman88),
        typeof(Feynman89),
        typeof(Feynman90),
        typeof(Feynman91),
        typeof(Feynman92),
        typeof(Feynman93),
        typeof(Feynman94),
        typeof(Feynman95),
        typeof(Feynman96),
        typeof(Feynman97),
        typeof(Feynman98),
        typeof(Feynman99),
        typeof(Feynman100),
        typeof(FeynmanBonus1),
        typeof(FeynmanBonus2),
        typeof(FeynmanBonus3),
        typeof(FeynmanBonus4),
        typeof(FeynmanBonus5),
        typeof(FeynmanBonus6),
        typeof(FeynmanBonus7),
        typeof(FeynmanBonus8),
        typeof(FeynmanBonus9),
        typeof(FeynmanBonus10),
        typeof(FeynmanBonus11),
        typeof(FeynmanBonus12),
        typeof(FeynmanBonus13),
        typeof(FeynmanBonus14),
        typeof(FeynmanBonus15),
        typeof(FeynmanBonus16),
        typeof(FeynmanBonus17),
        typeof(FeynmanBonus18),
        typeof(FeynmanBonus19),
        typeof(FeynmanBonus20)
   };
      #endregion


      foreach (var n in noiseRatio) {
        foreach (var type in descriptorTypes) {
          descriptorList.Add((IDataDescriptor)Activator.CreateInstance(type, rand.Next(), 100, 100, n));
        }
      }
      return descriptorList.ToList();
    }
  }
}
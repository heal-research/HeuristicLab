#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Random;
using HeuristicLab.Problems.BinPacking.Random;

namespace HeuristicLab.Problems.BinPacking3D.Instances {
  // make sure that for each class we have a separate entry in the problem instance providers

  public class RandomInstanceClass1Provider : RandomInstanceProvider {
    public RandomInstanceClass1Provider() : base(new SRand48()) { @class = 1; binWidth = binHeight = binDepth = 100; }

  }

  public class RandomInstanceClass2Provider : RandomInstanceProvider {
    public RandomInstanceClass2Provider() : base(new SRand48()) { @class = 2; binWidth = binHeight = binDepth = 100; }

  }
  public class RandomInstanceClass3Provider : RandomInstanceProvider {
    public RandomInstanceClass3Provider() : base(new SRand48()) { @class = 3; binWidth = binHeight = binDepth = 100; }

  }
  public class RandomInstanceClass4Provider : RandomInstanceProvider {
    public RandomInstanceClass4Provider() : base(new SRand48()) { @class = 4; binWidth = binHeight = binDepth = 100; }

  }
  public class RandomInstanceClass5Provider : RandomInstanceProvider {
    public RandomInstanceClass5Provider() : base(new SRand48()) { @class = 5; binWidth = binHeight = binDepth = 100; }

  }

  public class RandomInstanceClass6Provider : RandomInstanceProvider {
    public RandomInstanceClass6Provider() : base(new SRand48()) {
      @class = 6;
      binWidth = binHeight = binDepth = 10;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, 10);
      h = rand.Next(1, 10);
      d = rand.Next(1, 10);
    }
  }
  public class RandomInstanceClass7Provider : RandomInstanceProvider {
    public RandomInstanceClass7Provider() : base(new SRand48()) {
      @class = 7;
      binWidth = binHeight = binDepth = 40;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, 35);
      h = rand.Next(1, 35);
      d = rand.Next(1, 35);
    }
  }
  public class RandomInstanceClass8Provider : RandomInstanceProvider {
    public RandomInstanceClass8Provider() : base(new SRand48()) {
      @class = 8;
      binWidth = binHeight = binDepth = 100;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, 100);
      h = rand.Next(1, 100);
      d = rand.Next(1, 100);
    }
  }

  // class 9 from the paper (all-fill) is not implemented 


  public abstract class RandomInstanceProvider : ProblemInstanceProvider<BPPData>, IProblemInstanceProvider<BPPData> {

    /// <summary>
    /// Number of created test items. This items are used for packing them into the bin
    /// </summary>
    private readonly int[] _numberOfGeneratedTestItems = new int[] { 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200 };

    /// <summary>
    /// Number of instance for which should be created for each instance
    /// </summary>
    private readonly int _numberOfGeneratedInstances;

    /// <summary>
    /// Random generator for creating random packing items.
    /// </summary>
    private readonly IRandom _randomGenerator;


    protected int @class;
    protected int binWidth, binHeight, binDepth;

    #region Common information for displaying on the ui

    public override string Name {
      get { return string.Format("Martello, Pisinger, Vigo (class={0})", @class); }
    }

    public override string Description {
      get { return "Randomly generated 3d bin packing problems as described in Martello, Pisinger, Vigo: 'The Three-Dimensional Bin Packing Problem', Operations Research Vol 48, Issue 2, 2000, pp. 256-267."; }
    }

    public override Uri WebLink {
      get { return null; }
    }

    public override string ReferencePublication {
      get { return "Martello, Pisinger, Vigo: 'The Three-Dimensional Bin Packing Problem', Operations Research Vol 48, Issue 2, 2000, pp. 256-267."; }
    }

    #endregion


    public RandomInstanceProvider(IRandom randomGenerator, int numberOfGeneratedInstances = 10, int[] numberOfGeneratedTestItems = null) : base() {
      _numberOfGeneratedInstances = numberOfGeneratedInstances;
      if (numberOfGeneratedTestItems != null) {
        _numberOfGeneratedTestItems = numberOfGeneratedTestItems;
      }
      _randomGenerator = randomGenerator;
    }


    /// <summary>
    /// Returns a collection of data descriptors. Each descriptor contains the seed for the random generator.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      // 10 classes
      foreach (int numItems in _numberOfGeneratedTestItems) {
        for (int instance = 1; instance <= _numberOfGeneratedInstances; instance++) {
          string name = string.Format("n={0}-id={1:00} (class={2})", numItems, instance, @class);
          /* As in the test programm of Silvano Martello, David Pisinger, Daniele Vigo given, 
           * the seed of the instance provider will be calculated by adding the number of generated items and teh instance number.
           * This guarantees that the instances will always be the same
           */
          yield return new RandomDataDescriptor(name, name, numItems, @class, seed: numItems + instance);
        }
      }
    }

    /// <summary>
    /// Loads the data from the given data descriptor. 
    /// It retuns a bin packing problem data instance with the data of the random instance provider.
    /// </summary>
    /// <param name="dd"></param>
    /// <returns></returns>
    public override BPPData LoadData(IDataDescriptor dd) {
      var randDd = dd as RandomDataDescriptor;
      if (randDd == null) {
        throw new NotSupportedException("Cannot load data descriptor " + dd);
      }

      var data = new BPPData() {
        BinShape = new PackingShape(binWidth, binHeight, binDepth),
        Items = new PackingItem[randDd.NumItems]
      };
      _randomGenerator.Reset(randDd.Seed);
      for (int i = 0; i < randDd.NumItems; i++) {
        int w, h, d;
        SampleItemParameters(_randomGenerator, out w, out h, out d);
        data.Items[i] = new PackingItem(w, h, d, data.BinShape);
      }
      return data;
    }


    /// <summary>
    /// Generates the dimensions for a item by using the given random generator
    /// </summary>
    /// <param name="rand">Given random generator</param>
    /// <param name="w">Calculated width of the item</param>
    /// <param name="h">Calculated height of the item</param>
    /// <param name="d">Calculated depth of the item</param>
    protected virtual void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      Contract.Assert(@class >= 1 && @class <= 5);
      /*var weights = new double[] { 0.1, 0.1, 0.1, 0.1, 0.1 };
      weights[@class - 1] = 0.6;
      var type = Enumerable.Range(1, 5).SampleProportional(rand, 1, weights).First();
      */

      // as by Martello and Vigo
      int type = @class;
      if (type <= 5) {
        var t = rand.Next(1, 10);
        if (t <= 5) {
          type = t;
        }
      }

      switch (type) {
        case 1:
          CreateInstanceDimensionsType1(rand, out w, out h, out d);
          break;
        case 2:
          CreateInstanceDimensionsType2(rand, out w, out h, out d);
          break;
        case 3:
          CreateInstanceDimensionsType3(rand, out w, out h, out d);
          break;
        case 4:
          CreateInstanceDimensionsType4(rand, out w, out h, out d);
          break;
        case 5:
          CreateInstanceDimensionsType5(rand, out w, out h, out d);
          break;
        default:
          throw new InvalidProgramException();
      }
    }


    #region Instance dimensions generators for type 1 - 5
    private void CreateInstanceDimensionsType1(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, binWidth / 2);
      h = rand.Next((binHeight * 2) / 3, binHeight);
      d = rand.Next((binDepth * 2) / 3, binDepth);
    }

    private void CreateInstanceDimensionsType2(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(((binWidth * 2) / 3), binWidth);
      h = rand.Next(1, binHeight / 2);
      d = rand.Next(((binDepth * 2) / 3), binDepth);
    }

    private void CreateInstanceDimensionsType3(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(((binWidth * 2) / 3), binWidth);
      h = rand.Next(((binHeight * 2) / 3), binHeight);
      d = rand.Next(1, binDepth / 2);
    }
    private void CreateInstanceDimensionsType4(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(binWidth / 2, binWidth);
      h = rand.Next(binHeight / 2, binHeight);
      d = rand.Next(binDepth / 2, binDepth);
    }
    private void CreateInstanceDimensionsType5(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, binWidth / 2);
      h = rand.Next(1, binHeight / 2);
      d = rand.Next(1, binDepth / 2);
    }

    #endregion



    public override bool CanImportData {
      get { return false; }
    }
    public override BPPData ImportData(string path) {
      throw new NotSupportedException();
    }

    public override bool CanExportData {
      get { return true; }
    }

    public override void ExportData(BPPData instance, string file) {
      using (Stream stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write)) {
        Export(instance, stream);
      }
    }
    public static void Export(BPPData instance, Stream stream) {

      using (var writer = new StreamWriter(stream)) {
        writer.WriteLine(String.Format("{0,-5} {1,-5} {2,-5}   WBIN,HBIN,DBIN", instance.BinShape.Width, instance.BinShape.Height, instance.BinShape.Depth));
        for (int i = 0; i < instance.NumItems; i++) {
          if (i == 0)
            writer.WriteLine("{0,-5} {1,-5} {2,-5}   W(I),H(I),D(I),I=1,...,N", instance.Items[i].Width, instance.Items[i].Height, instance.Items[i].Depth);
          else
            writer.WriteLine("{0,-5} {1,-5} {2,-5}", instance.Items[i].Width, instance.Items[i].Height, instance.Items[i].Depth);
        }
        writer.Flush();
      }
    }
  }
}

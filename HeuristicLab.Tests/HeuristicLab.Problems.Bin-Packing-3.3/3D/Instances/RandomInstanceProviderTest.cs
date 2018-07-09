using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.BinPacking3D;
using HeuristicLab.Problems.BinPacking3D.Instances;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.BinPacking._3D.Instances.Tests {
  [TestClass]
  public class RandomInstanceProviderTest {

    private struct Dimension {
      public int Id { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
      public int Depth { get; set; }
    }    

    #region TestExtremePointAlgorithm

    /// <summary>
    /// Constants for testing the algorithm
    /// The test parameters are defined in the paper 
    /// </summary>
    private const int NUMBER_OF_TEST_INSTANCES = 10;
    private static readonly int[] TEST_CLASSES = { 1, 2 };
    private static readonly int[] NUMBER_OF_TEST_ITEMS = { 50, 100, 150, 200 };

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass1() {
      TestExtremePointAlgorithm(new RandomInstanceClass1Provider(), 1);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass2() {
      TestExtremePointAlgorithm(new RandomInstanceClass2Provider(), 2);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass3() {
      TestExtremePointAlgorithm(new RandomInstanceClass3Provider(), 3);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass4() {
      TestExtremePointAlgorithm(new RandomInstanceClass4Provider(), 4);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass5() {
      TestExtremePointAlgorithm(new RandomInstanceClass5Provider(), 5);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass6() {
      TestExtremePointAlgorithm(new RandomInstanceClass6Provider(), 6);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass7() {
      TestExtremePointAlgorithm(new RandomInstanceClass7Provider(), 7);
    }

    [TestMethod]
    [TestCategory("Problems.BinPacking.3D")]
    [TestProperty("Time", "long")]
    public void TestExtremePointAlgorithmClass8() {
      TestExtremePointAlgorithm(new RandomInstanceClass8Provider(), 8);
    }

    private void TestExtremePointAlgorithm(RandomInstanceProvider randomInstanceProvider, int @class) {
      foreach (SortingMethod sortingMethod in Enum.GetValues(typeof(SortingMethod))) {
        //foreach (FittingMethod fittingMethod in Enum.GetValues(typeof(FittingMethod))) {
        FittingMethod fittingMethod = FittingMethod.FreeVolumeBestFit;
        TestExtremePointAlgorithmByParameters(randomInstanceProvider, @class, sortingMethod, fittingMethod, ExtremePointCreationMethod.LineProjection);

        /*foreach (ExtremePointCreationMethod epCreationMethod in Enum.GetValues(typeof(ExtremePointCreationMethod))) {
          TestExtremePointAlgorithmByParameters(randomInstanceProvider, @class, sortingMethod, fittingMethod, epCreationMethod);
        }*/

        //}
      }
    }

    private void TestExtremePointAlgorithmByParameters(RandomInstanceProvider randomInstanceProvider, int @class, SortingMethod sortingMethod, FittingMethod fittingMethod, ExtremePointCreationMethod epCreationMethod) {
      var dataDescriptors = randomInstanceProvider.GetDataDescriptors();
      var referenceValues = GetReferenceAlgorithmValues();
      foreach (var numItems in NUMBER_OF_TEST_ITEMS) {
        int sumNumberOfBins = 0;

        double referenceValue = 0.0;
        if (referenceValues.TryGetValue(new Tuple<int, int, SortingMethod>(@class, numItems, sortingMethod), out referenceValue)) {
          for (int instance = 1; instance <= NUMBER_OF_TEST_INSTANCES; instance++) {
            string name = string.Format("n={0}-id={1:00} (class={2})", numItems, instance, @class);
            var selectedDataDescriptor = dataDescriptors.Where(dataDescriptor => dataDescriptor.Name == name);
            Assert.IsNotNull(selectedDataDescriptor?.First());
            var packingData = randomInstanceProvider.LoadData(selectedDataDescriptor.First());

            ExtremePointAlgorithm algorithm = new ExtremePointAlgorithm();
            algorithm.SortingMethodParameter.Value.Value = sortingMethod;
            algorithm.FittingMethodParameter.Value.Value = fittingMethod;
            algorithm.ExtremePointCreationMethodParameter.Value.Value = epCreationMethod;
            algorithm.SortBySequenceGroupParameter.Value.Value = false;

            algorithm.Problem.UseStackingConstraintsParameter.Value.Value = false;
            algorithm.Problem.Load(packingData);

            algorithm.Start();

            PackingPlan<BinPacking3D.PackingPosition, PackingShape, PackingItem> bestPackingPlan = null;
            foreach (Optimization.IResult result in algorithm.Results) {
              if (result.Name == "Best Solution") {
                bestPackingPlan = (PackingPlan<BinPacking3D.PackingPosition, PackingShape, PackingItem>)result.Value;
                break;
              }
            }

            sumNumberOfBins += bestPackingPlan.NrOfBins;
          }
        
          //Console.WriteLine($"{numItems};{@class};{sortingMethod};{epCreationMethod};: \tReference: ;{referenceValue}; \tImplementation: ;{(double)sumNumberOfBins / (double)NUMBER_OF_TEST_INSTANCES}; \t;{(referenceValue - ((double)sumNumberOfBins / (double)NUMBER_OF_TEST_INSTANCES)):F2};");
          var implementation = (double)sumNumberOfBins / (double)NUMBER_OF_TEST_INSTANCES;
          var enhancement = (referenceValue * 100 / implementation) - 100;
          Console.WriteLine($"{@class};{numItems};{sortingMethod};{enhancement:F2} %;{referenceValue};{implementation}");
//          Console.WriteLine($"{numItems}-{@class}-{sortingMethod}-{epCreationMethod}: \tReference: {referenceValue} \tImplementation: {(double)sumNumberOfBins / (double)NUMBER_OF_TEST_INSTANCES} \t{(referenceValue - ((double)sumNumberOfBins / (double)NUMBER_OF_TEST_INSTANCES)):F2}");
          //Assert.AreEqual(referenceValue, (double)sumNumberOfBins / (double)NUMBER_OF_TEST_INSTANCES, 20.0);
        }
      }
    }


    /// <summary>
    /// Returns a dictionary which contains the reference values from table 2 given by the paper https://www.cirrelt.ca/DocumentsTravail/CIRRELT-2007-41.pdf 
    /// Dictionary<Tuple<int, int, SortingMethod>, double> -> Dictionary<Tuple<@class, number of items, SortingMethod>, value>
    /// </summary>
    /// <returns></returns>
    private Dictionary<Tuple<int, int, SortingMethod>, double> GetReferenceAlgorithmValues() {
      Dictionary<Tuple<int, int, SortingMethod>, double> referenceValues = new Dictionary<Tuple<int, int, SortingMethod>, double>();

      AddToReferenceAlgorithmValuesDict(referenceValues, 1, SortingMethod.Given, new double[] { 14.6, 29.2, 40.1, 55.9});
      AddToReferenceAlgorithmValuesDict(referenceValues, 1, SortingMethod.HeightVolume, new double[] { 15, 29.2, 39.9, 55.6 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 1, SortingMethod.VolumeHeight, new double[] { 14.4, 29.5, 40.3, 55.7 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 1, SortingMethod.AreaHeight, new double[] { 14.4, 28.3, 39.2, 53.2 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 1, SortingMethod.HeightArea, new double[] { 15, 29, 39.8, 55.1});
      AddToReferenceAlgorithmValuesDict(referenceValues, 1, SortingMethod.ClusteredAreaHeight, new double[] { 14, 27.9, 38.1, 53 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 1, SortingMethod.ClusteredHeightArea, new double[] { 13.8, 27.4, 37.7, 52.3 });

      AddToReferenceAlgorithmValuesDict(referenceValues, 4, SortingMethod.Given, new double[] { 29.7, 60.2, 88.5, 119.9 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 4, SortingMethod.HeightVolume, new double[] { 30.1, 59.6, 88.3, 120.1 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 4, SortingMethod.VolumeHeight, new double[] { 29.9, 60.4, 88.6, 119.6 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 4, SortingMethod.AreaHeight, new double[] { 30, 59.7, 88.4, 120.3 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 4, SortingMethod.HeightArea, new double[] { 30, 59.6, 88.3, 120 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 4, SortingMethod.ClusteredAreaHeight, new double[] { 29.5, 59, 86.9, 119 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 4, SortingMethod.ClusteredHeightArea, new double[] { 29.5, 59, 86.9, 118.9 });

      AddToReferenceAlgorithmValuesDict(referenceValues, 5, SortingMethod.Given, new double[] { 10.1, 18.1, 24.4, 32.5 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 5, SortingMethod.HeightVolume, new double[] { 9, 16.7, 22.9, 30.7});
      AddToReferenceAlgorithmValuesDict(referenceValues, 5, SortingMethod.VolumeHeight, new double[] { 10, 17.8, 24.5, 32.6 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 5, SortingMethod.AreaHeight, new double[] { 9.2, 16.1, 21.9, 29.5 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 5, SortingMethod.HeightArea, new double[] { 9, 16.6, 22.6, 30.5 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 5, SortingMethod.ClusteredAreaHeight, new double[] { 8.5, 15.7, 21, 28.5 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 5, SortingMethod.ClusteredHeightArea, new double[] { 8.4, 15.4, 21.1, 28.2});

      AddToReferenceAlgorithmValuesDict(referenceValues, 6, SortingMethod.Given, new double[] { 11.7, 21.7, 33, 44.4});
      AddToReferenceAlgorithmValuesDict(referenceValues, 6, SortingMethod.HeightVolume, new double[] { 10.9, 21.2, 31.8, 41.5 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 6, SortingMethod.VolumeHeight, new double[] { 11.7, 22, 34.2, 44 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 6, SortingMethod.AreaHeight, new double[] { 10.6, 20.2, 30.8, 39.5});
      AddToReferenceAlgorithmValuesDict(referenceValues, 6, SortingMethod.HeightArea, new double[] { 10.9, 20.5, 31, 39.8 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 6, SortingMethod.ClusteredAreaHeight, new double[] { 10.2, 19.6, 29.9, 38.6 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 6, SortingMethod.ClusteredHeightArea, new double[] { 10.1, 19.8, 30.2, 38.8 });

      AddToReferenceAlgorithmValuesDict(referenceValues, 7, SortingMethod.Given, new double[] { 9.4, 15.9, 19.3, 30 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 7, SortingMethod.HeightVolume, new double[] { 8.2, 14.6, 19.2, 28.1 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 7, SortingMethod.VolumeHeight, new double[] { 9.3, 15.6, 19.7, 30.2 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 7, SortingMethod.AreaHeight, new double[] { 8.1, 14.1, 18.2, 26.2 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 7, SortingMethod.HeightArea, new double[] { 8.1, 14.1, 18.9, 27,2 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 7, SortingMethod.ClusteredAreaHeight, new double[] { 7.6, 13.4, 16.9, 25 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 7, SortingMethod.ClusteredHeightArea, new double[] { 7.7, 13.3, 16.9, 24.9 });

      AddToReferenceAlgorithmValuesDict(referenceValues, 8, SortingMethod.Given, new double[] { 11.6, 22, 28.5, 35.4 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 8, SortingMethod.HeightVolume, new double[] { 10.5, 20.9, 27.4, 33.9 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 8, SortingMethod.VolumeHeight, new double[] { 11.6, 22.1, 28.4, 35.4});
      AddToReferenceAlgorithmValuesDict(referenceValues, 8, SortingMethod.AreaHeight, new double[] { 10.1, 20.3, 26.4, 32.2 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 8, SortingMethod.HeightArea, new double[] { 10.5, 20.8, 37.7, 33.9 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 8, SortingMethod.ClusteredAreaHeight, new double[] { 9.6, 19.4, 25.4, 31.4 });
      AddToReferenceAlgorithmValuesDict(referenceValues, 8, SortingMethod.ClusteredHeightArea, new double[] { 9.5, 19.7, 25.5, 31.5 });

      return referenceValues;
    }

    private void AddToReferenceAlgorithmValuesDict(Dictionary<Tuple<int, int, SortingMethod>, double> referenceValues, int @class, SortingMethod sortingMethod, double[] values) {
      for (int i = 0; i < values.Length; i++) {
        referenceValues.Add(new Tuple<int, int, SortingMethod>(@class, 50 + (50 * i), sortingMethod), values[i]);
      }
      
    }


    #endregion
  }

}

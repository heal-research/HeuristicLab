#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Algorithms.VariableNeighborhoodSearch;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.TravelingSalesman;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class VnsTspSampleTest {
    private const string SampleFileName = "VNS_TSP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateVnsTspSampleTest() {
      var vns = CreateVnsTspSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(vns, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunVnsTspSampleTest() {
      var vns = CreateVnsTspSample();
      vns.SetSeedRandomly = false;
      SamplesUtils.RunAlgorithm(vns);
      Assert.AreEqual(867, SamplesUtils.GetDoubleResult(vns, "BestQuality"));
      Assert.AreEqual(867, SamplesUtils.GetDoubleResult(vns, "CurrentAverageQuality"));
      Assert.AreEqual(867, SamplesUtils.GetDoubleResult(vns, "CurrentWorstQuality"));
      Assert.AreEqual(12975173, SamplesUtils.GetIntResult(vns, "EvaluatedSolutions"));
    }

    private VariableNeighborhoodSearch CreateVnsTspSample() {
      VariableNeighborhoodSearch vns = new VariableNeighborhoodSearch();
      #region Problem Configuration
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.BestKnownSolution = new Permutation(PermutationTypes.Absolute, new int[] {
117, 65, 73, 74, 75, 76, 82, 86, 87, 94, 100, 106, 115, 120, 124, 107, 101, 108, 109, 102, 97, 90, 96, 95, 88, 89, 84, 78, 69, 57, 68, 56, 44, 55, 45, 36, 46, 37, 38, 47, 48, 59, 49, 58, 70, 77, 83, 79, 50, 80, 85, 98, 103, 110, 116, 121, 125, 133, 132, 138, 139, 146, 147, 159, 168, 169, 175, 182, 188, 201, 213, 189, 214, 221, 230, 246, 262, 276, 284, 275, 274, 261, 245, 229, 220, 228, 243, 259, 273, 282, 272, 258, 242, 257, 293, 292, 302, 310, 319, 320, 327, 326, 333, 340, 346, 339, 345, 344, 337, 338, 332, 325, 318, 309, 301, 291, 271, 251, 270, 233, 250, 269, 268, 280, 290, 300, 415, 440, 416, 417, 441, 458, 479, 418, 419, 395, 420, 442, 421, 396, 397, 422, 423, 461, 481, 502, 460, 501, 459, 480, 500, 517, 531, 516, 530, 499, 478, 457, 439, 414, 413, 412, 438, 456, 477, 498, 515, 529, 538, 547, 558, 559, 560, 548, 539, 549, 561, 562, 551, 550, 532, 540, 533, 541, 518, 534, 542, 552, 553, 554, 555, 535, 543, 556, 544, 536, 522, 505, 521, 520, 504, 519, 503, 482, 462, 463, 464, 483, 443, 465, 484, 506, 485, 507, 508, 487, 467, 486, 466, 445, 428, 444, 424, 425, 426, 427, 398, 399, 400, 381, 382, 371, 372, 401, 429, 446, 430, 402, 383, 366, 356, 357, 352, 385, 384, 403, 431, 447, 469, 468, 488, 489, 490, 470, 471, 448, 432, 433, 404, 405, 386, 373, 374, 367, 376, 375, 387, 491, 509, 537, 510, 492, 472, 449, 388, 389, 406, 450, 407, 377, 368, 359, 354, 350, 335, 324, 330, 390, 434, 451, 473, 493, 511, 523, 545, 563, 565, 567, 570, 569, 578, 577, 576, 575, 574, 573, 572, 580, 584, 583, 582, 587, 586, 585, 581, 579, 571, 568, 566, 564, 557, 546, 527, 513, 526, 525, 524, 512, 495, 494, 474, 452, 436, 409, 435, 453, 475, 496, 514, 528, 497, 455, 476, 454, 437, 411, 410, 394, 393, 392, 380, 370, 379, 408, 391, 378, 369, 364, 365, 361, 355, 351, 343, 336, 331, 317, 299, 286, 287, 278, 263, 264, 265, 223, 202, 248, 266, 279, 288, 289, 281, 267, 249, 232, 224, 216, 215, 204, 192, 193, 194, 186, 179, 185, 203, 191, 190, 177, 171, 161, 128, 135, 140, 149, 162, 150, 163, 172, 178, 173, 164, 152, 151, 141, 153, 165, 154, 142, 155, 143, 137, 136, 130, 129, 118, 114, 113, 105, 119, 123, 131, 144, 156, 157, 145, 158, 166, 167, 174, 180, 181, 187, 195, 205, 217, 226, 236, 225, 234, 252, 235, 253, 254, 255, 238, 239, 240, 241, 256, 237, 206, 207, 208, 196, 197, 198, 209, 199, 200, 211, 212, 219, 210, 218, 227, 244, 260, 283, 294, 295, 303, 296, 311, 304, 297, 298, 305, 285, 306, 314, 329, 321, 313, 312, 328, 334, 341, 347, 348, 353, 358, 362, 363, 360, 349, 342, 322, 323, 315, 316, 308, 307, 277, 247, 231, 222, 184, 183, 176, 170, 160, 148, 134, 127, 126, 111, 104, 92, 91, 71, 60, 51, 52, 40, 32, 23, 21, 20, 18, 17, 16, 14, 13, 11, 10, 7, 6, 5, 2, 1, 0, 3, 4, 31, 39, 25, 30, 35, 34, 33, 43, 54, 42, 27, 28, 29, 9, 8, 12, 15, 19, 22, 24, 26, 41, 67, 66, 64, 63, 53, 62, 61, 72, 81, 93, 99, 112, 122, 
      });
      tspProblem.Coordinates = new DoubleMatrix(new double[,] {
{48, 71}, {49, 71}, {50, 71}, {44, 70}, {45, 70}, {52, 70}, {53, 70}, {54, 70}, {41, 69}, {42, 69}, {55, 69}, {56, 69}, {40, 68}, {56, 68}, {57, 68}, {39, 67}, {57, 67}, {58, 67}, {59, 67}, {38, 66}, {59, 66}, {60, 66}, {37, 65}, {60, 65}, {36, 64}, {43, 64}, {35, 63}, {37, 63}, {41, 63}, {42, 63}, {43, 63}, {47, 63}, {61, 63}, {40, 62}, {41, 62}, {42, 62}, {43, 62}, {45, 62}, {46, 62}, {47, 62}, {62, 62}, {34, 61}, {38, 61}, {39, 61}, {42, 61}, {43, 61}, {44, 61}, {45, 61}, {46, 61}, {47, 61}, {52, 61}, {62, 61}, {63, 61}, {26, 60}, {38, 60}, {42, 60}, {43, 60}, {44, 60}, {46, 60}, {47, 60}, {63, 60}, {23, 59}, {24, 59}, {27, 59}, {29, 59}, {30, 59}, {31, 59}, {33, 59}, {42, 59}, {46, 59}, {47, 59}, {63, 59}, {21, 58}, {32, 58}, {33, 58}, {34, 58}, {35, 58}, {46, 58}, {47, 58}, {48, 58}, {53, 58}, {21, 57}, {35, 57}, {47, 57}, {48, 57}, {53, 57}, {36, 56}, {37, 56}, {46, 56}, {47, 56}, {48, 56}, {64, 56}, {65, 56}, {20, 55}, {38, 55}, {46, 55}, {47, 55}, {48, 55}, {52, 55}, {21, 54}, {40, 54}, {47, 54}, {48, 54}, {52, 54}, {65, 54}, {30, 53}, {41, 53}, {46, 53}, {47, 53}, {48, 53}, {52, 53}, {65, 53}, {21, 52}, {32, 52}, {33, 52}, {42, 52}, {51, 52}, {21, 51}, {33, 51}, {34, 51}, {43, 51}, {51, 51}, {21, 50}, {35, 50}, {44, 50}, {50, 50}, {66, 50}, {67, 50}, {21, 49}, {34, 49}, {36, 49}, {37, 49}, {46, 49}, {49, 49}, {67, 49}, {22, 48}, {36, 48}, {37, 48}, {46, 48}, {47, 48}, {22, 47}, {30, 47}, {34, 47}, {37, 47}, {38, 47}, {39, 47}, {47, 47}, {48, 47}, {67, 47}, {23, 46}, {28, 46}, {29, 46}, {30, 46}, {31, 46}, {32, 46}, {35, 46}, {37, 46}, {38, 46}, {39, 46}, {49, 46}, {67, 46}, {23, 45}, {28, 45}, {29, 45}, {31, 45}, {32, 45}, {40, 45}, {41, 45}, {49, 45}, {50, 45}, {68, 45}, {24, 44}, {29, 44}, {32, 44}, {41, 44}, {51, 44}, {68, 44}, {25, 43}, {30, 43}, {32, 43}, {42, 43}, {43, 43}, {51, 43}, {68, 43}, {69, 43}, {31, 42}, {32, 42}, {43, 42}, {52, 42}, {55, 42}, {26, 41}, {27, 41}, {31, 41}, {32, 41}, {33, 41}, {44, 41}, {45, 41}, {46, 41}, {47, 41}, {48, 41}, {49, 41}, {53, 41}, {25, 40}, {27, 40}, {32, 40}, {43, 40}, {44, 40}, {45, 40}, {46, 40}, {48, 40}, {49, 40}, {50, 40}, {51, 40}, {53, 40}, {56, 40}, {32, 39}, {33, 39}, {43, 39}, {50, 39}, {51, 39}, {54, 39}, {56, 39}, {69, 39}, {24, 38}, {32, 38}, {41, 38}, {42, 38}, {51, 38}, {52, 38}, {54, 38}, {57, 38}, {69, 38}, {31, 37}, {32, 37}, {40, 37}, {41, 37}, {42, 37}, {43, 37}, {44, 37}, {45, 37}, {46, 37}, {47, 37}, {48, 37}, {51, 37}, {52, 37}, {55, 37}, {57, 37}, {69, 37}, {24, 36}, {31, 36}, {32, 36}, {39, 36}, {40, 36}, {41, 36}, {42, 36}, {43, 36}, {45, 36}, {48, 36}, {49, 36}, {51, 36}, {53, 36}, {55, 36}, {58, 36}, {22, 35}, {23, 35}, {24, 35}, {25, 35}, {30, 35}, {31, 35}, {32, 35}, {39, 35}, {41, 35}, {49, 35}, {51, 35}, {55, 35}, {56, 35}, {58, 35}, {71, 35}, {20, 34}, {27, 34}, {30, 34}, {31, 34}, {51, 34}, {53, 34}, {57, 34}, {60, 34}, {18, 33}, {19, 33}, {29, 33}, {30, 33}, {31, 33}, {45, 33}, {46, 33}, {47, 33}, {52, 33}, {53, 33}, {55, 33}, {57, 33}, {58, 33}, {17, 32}, {30, 32}, {44, 32}, {47, 32}, {54, 32}, {57, 32}, {59, 32}, {61, 32}, {71, 32}, {72, 32}, {43, 31}, {47, 31}, {56, 31}, {58, 31}, {59, 31}, {61, 31}, {72, 31}, {74, 31}, {16, 30}, {43, 30}, {46, 30}, {47, 30}, {59, 30}, {63, 30}, {71, 30}, {75, 30}, {43, 29}, {46, 29}, {47, 29}, {59, 29}, {60, 29}, {75, 29}, {15, 28}, {43, 28}, {46, 28}, {61, 28}, {76, 28}, {15, 27}, {43, 27}, {44, 27}, {45, 27}, {46, 27}, {60, 27}, {62, 27}, {15, 26}, {43, 26}, {44, 26}, {46, 26}, {59, 26}, {60, 26}, {64, 26}, {77, 26}, {15, 25}, {58, 25}, {61, 25}, {77, 25}, {15, 24}, {53, 24}, {55, 24}, {61, 24}, {77, 24}, {62, 23}, {16, 22}, {61, 22}, {62, 22}, {15, 21}, {16, 21}, {52, 21}, {63, 21}, {77, 21}, {16, 20}, {17, 20}, {46, 20}, {47, 20}, {60, 20}, {62, 20}, {63, 20}, {65, 20}, {76, 20}, {15, 19}, {17, 19}, {18, 19}, {44, 19}, {45, 19}, {48, 19}, {53, 19}, {56, 19}, {60, 19}, {62, 19}, {67, 19}, {68, 19}, {76, 19}, {15, 18}, {18, 18}, {19, 18}, {20, 18}, {32, 18}, {33, 18}, {34, 18}, {41, 18}, {42, 18}, {43, 18}, {46, 18}, {48, 18}, {53, 18}, {59, 18}, {60, 18}, {69, 18}, {75, 18}, {16, 17}, {17, 17}, {20, 17}, {21, 17}, {22, 17}, {23, 17}, {24, 17}, {26, 17}, {28, 17}, {29, 17}, {30, 17}, {31, 17}, {32, 17}, {34, 17}, {35, 17}, {36, 17}, {37, 17}, {38, 17}, {39, 17}, {40, 17}, {44, 17}, {46, 17}, {48, 17}, {53, 17}, {56, 17}, {58, 17}, {75, 17}, {17, 16}, {18, 16}, {20, 16}, {24, 16}, {26, 16}, {27, 16}, {29, 16}, {33, 16}, {41, 16}, {42, 16}, {44, 16}, {47, 16}, {52, 16}, {57, 16}, {70, 16}, {73, 16}, {74, 16}, {17, 15}, {18, 15}, {20, 15}, {22, 15}, {24, 15}, {27, 15}, {29, 15}, {31, 15}, {33, 15}, {35, 15}, {36, 15}, {38, 15}, {39, 15}, {42, 15}, {45, 15}, {47, 15}, {52, 15}, {53, 15}, {55, 15}, {56, 15}, {70, 15}, {73, 15}, {17, 14}, {19, 14}, {21, 14}, {24, 14}, {26, 14}, {29, 14}, {31, 14}, {34, 14}, {37, 14}, {40, 14}, {42, 14}, {44, 14}, {46, 14}, {47, 14}, {53, 14}, {54, 14}, {55, 14}, {62, 14}, {70, 14}, {72, 14}, {17, 13}, {19, 13}, {21, 13}, {23, 13}, {25, 13}, {27, 13}, {30, 13}, {32, 13}, {34, 13}, {36, 13}, {38, 13}, {41, 13}, {43, 13}, {44, 13}, {45, 13}, {60, 13}, {70, 13}, {71, 13}, {18, 12}, {21, 12}, {23, 12}, {26, 12}, {28, 12}, {31, 12}, {34, 12}, {37, 12}, {39, 12}, {41, 12}, {42, 12}, {70, 12}, {18, 11}, {19, 11}, {20, 11}, {21, 11}, {24, 11}, {25, 11}, {27, 11}, {29, 11}, {31, 11}, {33, 11}, {35, 11}, {38, 11}, {41, 11}, {59, 11}, {26, 10}, {29, 10}, {32, 10}, {34, 10}, {36, 10}, {39, 10}, {40, 10}, {69, 10}, {21, 9}, {26, 9}, {28, 9}, {30, 9}, {32, 9}, {33, 9}, {35, 9}, {36, 9}, {37, 9}, {38, 9}, {39, 9}, {22, 8}, {27, 8}, {28, 8}, {29, 8}, {30, 8}, {31, 8}, {68, 8}, {23, 7}, {66, 7}, {24, 6}, {65, 6}, {25, 5}, {62, 5}, {63, 5}, {26, 4}, {55, 4}, {56, 4}, {57, 4}, {58, 4}, {59, 4}, {60, 4}, {61, 4}, {28, 3}, {53, 3}, {29, 2}, {50, 2}, {51, 2}, {52, 2}, {31, 1}, {32, 1}, {48, 1}
      });
      tspProblem.BestKnownQuality = new DoubleValue(867);

      tspProblem.EvaluatorParameter.Value = new TSPRoundedEuclideanPathEvaluator();
      tspProblem.SolutionCreatorParameter.Value = new RandomPermutationCreator();
      tspProblem.UseDistanceMatrix.Value = true;
      tspProblem.Name = "Funny TSP";
      tspProblem.Description = "Represents a symmetric Traveling Salesman Problem.";
      #endregion
      #region Algorithm Configuration
      vns.Name = "Variable Neighborhood Search - TSP";
      vns.Description = "A variable neighborhood search algorithm which solves a funny TSP instance";
      vns.Problem = tspProblem;

      var localImprovement = vns.LocalImprovementParameter.ValidValues
        .OfType<LocalSearchImprovementOperator>()
        .Single();
      // move generator has to be set first
      localImprovement.MoveGenerator = localImprovement.MoveGeneratorParameter.ValidValues
        .OfType<StochasticInversionMultiMoveGenerator>()
        .Single();
      localImprovement.MoveEvaluator = localImprovement.MoveEvaluatorParameter.ValidValues
        .OfType<TSPInversionMoveRoundedEuclideanPathEvaluator>()
        .Single();
      localImprovement.MoveMaker = localImprovement.MoveMakerParameter.ValidValues
        .OfType<InversionMoveMaker>()
        .Single();
      localImprovement.SampleSizeParameter.Value = new IntValue(500);
      vns.LocalImprovement = localImprovement;

      vns.LocalImprovementMaximumIterations = 150;
      vns.MaximumIterations = 25;
      vns.Seed = 0;
      vns.SetSeedRandomly = true;
      var shakingOperator = vns.ShakingOperatorParameter.ValidValues
        .OfType<PermutationShakingOperator>()
        .Single();
      shakingOperator.Operators.SetItemCheckedState(shakingOperator.Operators
        .OfType<Swap2Manipulator>()
        .Single(), false);
      shakingOperator.Operators.SetItemCheckedState(shakingOperator.Operators
        .OfType<Swap3Manipulator>()
        .Single(), false);
      vns.ShakingOperator = shakingOperator;
      #endregion
      vns.Engine = new ParallelEngine.ParallelEngine();
      return vns;
    }
  }
}

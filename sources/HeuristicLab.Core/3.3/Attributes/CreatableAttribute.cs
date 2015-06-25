#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class CreatableAttribute : Attribute {
    #region Predefined Categories
    public static class Categories {
      public const string SplitToken = "###";
      public const string OrderToken = "$$$";

      public const string Algorithms = "1" + OrderToken + "Algorithms";
      public const string PopulationBasedAlgorithms = Algorithms + SplitToken + "1" + OrderToken + "Population Based";
      public const string SingleSolutionAlgorithms = Algorithms + SplitToken + "2" + OrderToken + "Single Solution";

      public const string Problems = "2" + OrderToken + "Problems";
      public const string CombinatorialProblems = Problems + SplitToken + "1" + OrderToken + "Combinatorial";
      public const string GeneticProgrammingProblems = Problems + SplitToken + "2" + OrderToken + "Genetic Programming";
      public const string ExternalEvaluationProblems = Problems + SplitToken + "3" + OrderToken + "External Evaluation";

      public const string DataAnalysis = "3" + OrderToken + "Data Analysis";
      public const string DataAnalysisRegression = DataAnalysis + SplitToken + "1" + OrderToken + "Regression";
      public const string DataAnalysisClassification = DataAnalysis + SplitToken + "2" + OrderToken + "Classification";
      public const string DataAnalysisEnsembles = DataAnalysis + SplitToken + "3" + OrderToken + "Ensembles";

      public const string TestingAndAnalysis = "4" + OrderToken + "Testing & Analysis";
      public const string TestingAndAnalysisOKB = TestingAndAnalysis + SplitToken + "1" + OrderToken + "OKB";

      public const string Scripts = "5" + OrderToken + "Scripts";

      public static string GetFullName(string rawName) {
        return string.Join(SplitToken, GetTokens(rawName));
      }
      public static string GetName(string rawName) {
        return GetTokens(rawName).Last();
      }
      public static IEnumerable<string> GetParentRawNames(string rawName) {
        var tokens = GetTokensWithOrdering(rawName).ToList();
        return tokens.Take(tokens.Count - 1);
      }

      private static IEnumerable<string> GetTokensWithOrdering(string rawName) {
        return rawName.Split(new[] { SplitToken }, StringSplitOptions.RemoveEmptyEntries);
      }
      private static IEnumerable<string> GetTokens(string rawName) {
        return GetTokensWithOrdering(rawName)
          .Select(t => t.Split(new[] { OrderToken }, StringSplitOptions.RemoveEmptyEntries).Last());
      }
    }
    #endregion

    private string category;
    public string Category {
      get {
        return category;
      }
      set {
        if (value == null) throw new ArgumentNullException("Category", "CreataleAttribute.Category must not be null");
        category = value;
      }
    }

    public int Priority { get; set; }


    public CreatableAttribute() {
      Category = "Other Items";
      Priority = int.MaxValue;
    }
    public CreatableAttribute(object category)
      : this() {
      Category = (string)category;
    }

    public static bool IsCreatable(Type type) {
      object[] attribs = type.GetCustomAttributes(typeof(CreatableAttribute), false);
      return attribs.Length > 0;
    }
    public static string GetCategory(Type type) {
      object[] attribs = type.GetCustomAttributes(typeof(CreatableAttribute), false);
      if (attribs.Length > 0) return ((CreatableAttribute)attribs[0]).Category;
      else return null;
    }

    public static int GetPriority(Type type) {
      var attribs = type.GetCustomAttributes(typeof(CreatableAttribute), false);
      if (attribs.Length > 0) return ((CreatableAttribute)attribs[0]).Priority;
      else return int.MaxValue;
    }
  }
}

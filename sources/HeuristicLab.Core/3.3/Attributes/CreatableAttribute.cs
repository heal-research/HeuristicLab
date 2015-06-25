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

namespace HeuristicLab.Core {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class CreatableAttribute : Attribute {
    #region Predefined Categories
    public static class Categories {
      public const string Splitter = "###";

      public const string Algorithms = "Algorithms";
      public const string PopulationBasedAlgorithms = Algorithms + Splitter + "Population Based";
      public const string SingleSolutionAlgorithms = Algorithms + Splitter + "Single Solution";

      public const string Problems = "Problems";
      public const string CombinatorialProblems = Problems + Splitter + "Combinatorial";
      public const string GeneticProgrammingProblems = Problems + Splitter + "Genetic Programming";
      public const string ExternalEvaluationProblems = Problems + Splitter + "External Evaluation";

      public const string DataAnalysis = "Data Analysis";
      public const string DataAnalysisClassification = DataAnalysis + Splitter + "Classification";
      public const string DataAnalysisRegression = DataAnalysis + Splitter + "Regression";
      public const string DataAnalysisEnsembles = DataAnalysis + Splitter + "Ensembles";

      public const string TestingAndAnalysis = "Testing & Analysis";
      public const string TestingAndAnalysisOKB = TestingAndAnalysis + Splitter + "OKB";

      public const string Scripts = "5 - Scripts";
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

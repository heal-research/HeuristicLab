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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Collections;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;

namespace HeuristicLab.CEDMA.Charting {
  public class ResultList : ItemBase {
    private const string cedmaNS = "http://www.heuristiclab.com/cedma/";

    private const string MAPE_TRAINING = "MAPE (training)";
    private const string MAPE_VALIDATION = "MAPE (validation)";
    private const string MAPE_TEST = "MAPE (test)";
    private const string R2_TRAINING = "R2 (training)";
    private const string R2_VALIDATION = "R2 (validation)";
    private const string R2_TEST = "R2 (test)";
    private const string TARGET_VARIABLE = "Target variable";
    private const string TREE_SIZE = "Tree size";
    private const string TREE_HEIGHT = "Tree height";

    private readonly Entity targetVariablePredicate = new Entity(cedmaNS + "TargetVariable");
    private readonly Entity trainingMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorTraining");
    private readonly Entity validationMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorValidation");
    private readonly Entity testMAPEPredicate = new Entity(cedmaNS + "MeanAbsolutePercentageErrorTest");
    private readonly Entity trainingR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationTraining");
    private readonly Entity validationR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationValidation");
    private readonly Entity testR2Predicate = new Entity(cedmaNS + "CoefficientOfDeterminationTest");
    private readonly Entity treeSizePredicate = new Entity(cedmaNS + "TreeSize");
    private readonly Entity treeHeightPredicate = new Entity(cedmaNS + "TreeHeight");
    private readonly Entity anyEntity = new Entity(null);

    private IStore store;
    public IStore Store {
      get { return store; }
      set {
        store = value;
        Action reloadList = ReloadList;
        reloadList.BeginInvoke(null, null);
      }
    }

    private List<string> variableNames = new List<string>() { TARGET_VARIABLE, TREE_SIZE, TREE_HEIGHT,
    MAPE_TRAINING, MAPE_VALIDATION, MAPE_TEST,
    R2_TRAINING, R2_VALIDATION, R2_TEST};
    public string[] VariableNames {
      get {
        return variableNames.ToArray();
      }
    }
    private Dictionary<string, List<double>> allVariables;

    private void ReloadList() {
      List<double> trainingMAPE = new List<double>();
      List<double> validationMAPE = new List<double>();
      List<double> testMAPE = new List<double>();
      List<double> trainingR2 = new List<double>();
      List<double> validationR2 = new List<double>();
      List<double> testR2 = new List<double>();
      List<double> size = new List<double>();
      List<double> height = new List<double>();
      List<double> targetVariable = new List<double>();

      lock(allVariables) {
        allVariables.Clear();
        allVariables[MAPE_TRAINING] = trainingMAPE;
        allVariables[MAPE_VALIDATION] = validationMAPE;
        allVariables[MAPE_TEST] = testMAPE;
        allVariables[R2_TRAINING] = trainingR2;
        allVariables[R2_VALIDATION] = validationR2;
        allVariables[R2_TEST] = testR2;
        allVariables[TREE_SIZE] = size;
        allVariables[TREE_HEIGHT] = height;
        allVariables[TARGET_VARIABLE] = targetVariable;
      }

      var results = store.Select(new Statement(anyEntity, new Entity(cedmaNS + "instanceOf"), new Literal("class:GpFunctionTree")))
      .Select(x => store.Select(new SelectFilter(
        new Entity[] { new Entity(x.Subject.Uri) },
        new Entity[] { targetVariablePredicate, treeSizePredicate, treeHeightPredicate,
          trainingMAPEPredicate, validationMAPEPredicate, testMAPEPredicate,
          trainingR2Predicate, validationR2Predicate, testR2Predicate },
          new Resource[] { anyEntity })));

      Random random = new Random(); // for adding random noise to int values
      foreach(Statement[] ss in results) {
        lock(allVariables) {
          targetVariable.Add(double.NaN);
          size.Add(double.NaN);
          height.Add(double.NaN);
          trainingMAPE.Add(double.NaN);
          validationMAPE.Add(double.NaN);
          testMAPE.Add(double.NaN);
          trainingR2.Add(double.NaN);
          validationR2.Add(double.NaN);
          testR2.Add(double.NaN);
          foreach(Statement s in ss) {
            if(s.Predicate.Equals(targetVariablePredicate)) {
              ReplaceLastItem(targetVariable, (double)(int)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(treeSizePredicate)) {
              ReplaceLastItem(size, (double)(int)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(treeHeightPredicate)) {
              ReplaceLastItem(height, (double)(int)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(trainingMAPEPredicate)) {
              ReplaceLastItem(trainingMAPE, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(validationMAPEPredicate)) {
              ReplaceLastItem(validationMAPE, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(testMAPEPredicate)) {
              ReplaceLastItem(testMAPE, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(trainingR2Predicate)) {
              ReplaceLastItem(trainingR2, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(validationR2Predicate)) {
              ReplaceLastItem(validationR2, (double)((Literal)s.Property).Value);
            } else if(s.Predicate.Equals(testR2Predicate)) {
              ReplaceLastItem(testR2, (double)((Literal)s.Property).Value);
            }
          }
        }
        FireChanged();
      }
    }

    private void ReplaceLastItem(List<double> xs, double x) {
      xs.RemoveAt(xs.Count - 1); xs.Add(x);
    }

    public ResultList()
      : base() {
      allVariables = new Dictionary<string, List<double>>();
    }

    public override IView CreateView() {
      return new ResultListView(this);
    }

    internal Histogram GetHistogram(string variableName) {
      Histogram h = new Histogram(50);
      lock(allVariables) {
        h.AddValues(allVariables[variableName]);
      }
      return h;
    }

    internal IList<double> GetValues(string variableName) {
      List<double> result = new List<double>();
      lock(allVariables) {
        result.AddRange(allVariables[variableName]);
      }
      return result;
    }
  }
}

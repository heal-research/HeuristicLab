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
using System.Linq;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.GeneticProgramming.ArtificialAnt {
  public class Interpreter {

    public int MaxTimeSteps { get; private set; }
    public int FoodEaten { get; private set; }
    public BoolMatrix World { get; private set; }
    public ISymbolicExpressionTree Expression { get; private set; }

    public int ElapsedTime { get; set; }

    private int currentDirection;
    private int currentAntLocationRow;
    private int currentAntLocationColumn;
    private int nFoodItems;
    private Stack<ISymbolicExpressionTreeNode> nodeStack = new Stack<ISymbolicExpressionTreeNode>();

    public Interpreter(ISymbolicExpressionTree tree, BoolMatrix world, int maxTimeSteps) {
      this.Expression = tree;
      this.MaxTimeSteps = maxTimeSteps;
      // create a clone of the world because the ant will remove the food items it can find.
      World = (BoolMatrix)world.Clone();
      CountFoodItems();
    }

    private void CountFoodItems() {
      nFoodItems = 0;
      for (int i = 0; i < World.Rows; i++) {
        for (int j = 0; j < World.Columns; j++) {
          if (World[i, j]) nFoodItems++;
        }
      }
    }

    public void AntLocation(out int row, out int column) {
      row = currentAntLocationRow;
      column = currentAntLocationColumn;
    }

    public int AntDirection {
      get { return currentDirection; }
    }

    public void Run() {
      while (ElapsedTime < MaxTimeSteps && FoodEaten < nFoodItems) {
        Step();
      }
    }

    public void Step() {
      // expression evaluated completly => start at root again
      if (nodeStack.Count == 0) {
        nodeStack.Push(Expression.Root.GetSubtree(0).GetSubtree(0));
      }

      var currentNode = nodeStack.Pop();
      if (currentNode.Symbol.Name == "Left") {
        currentDirection = (currentDirection + 3) % 4;
        ElapsedTime++;
      } else if (currentNode.Symbol.Name == "Right") {
        currentDirection = (currentDirection + 1) % 4;
        ElapsedTime++;
      } else if (currentNode.Symbol.Name == "Move") {
        MoveAntForward();
        if (World[currentAntLocationRow, currentAntLocationColumn]) {
          FoodEaten++;
          World[currentAntLocationRow, currentAntLocationColumn] = false;
        }
        ElapsedTime++;
      } else if (currentNode.Symbol.Name == "IfFoodAhead") {
        int nextAntLocationRow;
        int nextAntLocationColumn;
        NextField(out nextAntLocationRow, out nextAntLocationColumn);
        if (World[nextAntLocationRow, nextAntLocationColumn]) {
          nodeStack.Push(currentNode.GetSubtree(0));
        } else {
          nodeStack.Push(currentNode.GetSubtree(1));
        }
      } else if (currentNode.Symbol.Name == "Prog2") {
        nodeStack.Push(currentNode.GetSubtree(1));
        nodeStack.Push(currentNode.GetSubtree(0));
      } else if (currentNode.Symbol.Name == "Prog3") {
        nodeStack.Push(currentNode.GetSubtree(2));
        nodeStack.Push(currentNode.GetSubtree(1));
        nodeStack.Push(currentNode.GetSubtree(0));
      } else if (currentNode.Symbol is InvokeFunction) {
        var invokeNode = currentNode as InvokeFunctionTreeNode;
        var functionDefinition = (SymbolicExpressionTreeNode)FindMatchingFunction(invokeNode.Symbol.FunctionName).Clone();
        var argumentCutPoints = (from node in functionDefinition.IterateNodesPrefix()
                                 where node.Subtrees.Count() > 0
                                 from subtree in node.Subtrees
                                 where subtree is ArgumentTreeNode
                                 select new { Parent = node, Argument = subtree.Symbol as Argument, ChildIndex = node.IndexOfSubtree(subtree) }).ToList();
        foreach (var cutPoint in argumentCutPoints) {
          cutPoint.Parent.RemoveSubtree(cutPoint.ChildIndex);
          cutPoint.Parent.InsertSubtree(cutPoint.ChildIndex, (SymbolicExpressionTreeNode)invokeNode.GetSubtree(cutPoint.Argument.ArgumentIndex).Clone());
        }
        nodeStack.Push(functionDefinition.GetSubtree(0));
      } else {
        throw new InvalidOperationException(currentNode.Symbol.ToString());
      }
    }

    private void MoveAntForward() {
      NextField(out currentAntLocationRow, out currentAntLocationColumn);
    }

    private void NextField(out int nextAntLocationRow, out int nextAntLocationColumn) {
      switch (currentDirection) {
        case 0:
          nextAntLocationColumn = (currentAntLocationColumn + 1) % World.Columns; // EAST
          nextAntLocationRow = currentAntLocationRow;
          break;
        case 1:
          nextAntLocationRow = (currentAntLocationRow + 1) % World.Rows; // SOUTH
          nextAntLocationColumn = currentAntLocationColumn;
          break;
        case 2:
          nextAntLocationColumn = (currentAntLocationColumn + World.Columns - 1) % World.Columns; // WEST
          nextAntLocationRow = currentAntLocationRow;
          break;
        case 3:
          nextAntLocationRow = (currentAntLocationRow + World.Rows - 1) % World.Rows; // NORTH
          nextAntLocationColumn = currentAntLocationColumn;
          break;
        default:
          throw new InvalidOperationException();
      }
    }


    private SymbolicExpressionTreeNode FindMatchingFunction(string name) {
      foreach (var defunBranch in Expression.Root.Subtrees.OfType<DefunTreeNode>()) {
        if (defunBranch.FunctionName == name) return defunBranch;
      }
      throw new ArgumentException("Function definition for " + name + " not found.");
    }
  }
}

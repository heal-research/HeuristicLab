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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.LawnMower {
  public class Interpreter {
    private enum Heading {
      South,
      East,
      North,
      West
    };

    private class MowerState {
      public Heading Heading { get; set; }
      public Tuple<uint, uint> Position { get; set; }
      public int Energy { get; set; }
      public MowerState() {
        Heading = Heading.South;
        Position = new Tuple<uint, uint>(0, 0);
        Energy = 0;
      }
    }


    public static bool[,] EvaluateLawnMowerProgram(int length, int width, ISymbolicExpressionTree tree) {

      bool[,] lawn = new bool[length, width];
      var mowerState = new MowerState();
      mowerState.Heading = Heading.South;
      mowerState.Energy = length * width * 2;
      lawn[mowerState.Position.Item1, mowerState.Position.Item2] = true;
      EvaluateLawnMowerProgram(tree.Root, mowerState, lawn, tree.Root.Subtrees.Skip(1).ToArray());
      return lawn;
    }


    private static Tuple<int, int> EvaluateLawnMowerProgram(ISymbolicExpressionTreeNode node, MowerState mowerState, bool[,] lawn, IEnumerable<ISymbolicExpressionTreeNode> adfs) {
      if (mowerState.Energy <= 0) return Tuple.Create(0, 0);

      if (node.Symbol is ProgramRootSymbol) {
        return EvaluateLawnMowerProgram(node.GetSubtree(0), mowerState, lawn, adfs);
      } else if (node.Symbol is StartSymbol) {
        return EvaluateLawnMowerProgram(node.GetSubtree(0), mowerState, lawn, adfs);
      } else if (node.Symbol.Name == "Left") {
        switch (mowerState.Heading) {
          case Heading.East: mowerState.Heading = Heading.North;
            break;
          case Heading.North: mowerState.Heading = Heading.West;
            break;
          case Heading.West: mowerState.Heading = Heading.South;
            break;
          case Heading.South:
            mowerState.Heading = Heading.East;
            break;
        }
        return new Tuple<int, int>(0, 0);
      } else if (node.Symbol.Name == "Forward") {
        int dRow = 0;
        int dCol = 0;
        switch (mowerState.Heading) {
          case Heading.East:
            dCol++;
            break;
          case Heading.North:
            dRow--;
            break;
          case Heading.West:
            dCol--;
            break;
          case Heading.South:
            dRow++;
            break;
        }
        uint newRow = (uint)((mowerState.Position.Item1 + lawn.GetLength(0) + dRow) % lawn.GetLength(0));
        uint newColumn = (uint)((mowerState.Position.Item2 + lawn.GetLength(1) + dCol) % lawn.GetLength(1));
        mowerState.Position = Tuple.Create(newRow, newColumn);
        mowerState.Energy = mowerState.Energy - 1;
        lawn[newRow, newColumn] = true;
        return Tuple.Create(0, 0);
      } else if (node.Symbol.Name == "Sum") {
        var p = EvaluateLawnMowerProgram(node.GetSubtree(0), mowerState, lawn, adfs);
        var q = EvaluateLawnMowerProgram(node.GetSubtree(1), mowerState, lawn, adfs);
        return Tuple.Create(p.Item1 + q.Item1, p.Item2 + q.Item2);
      } else if (node.Symbol.Name == "Prog") {
        EvaluateLawnMowerProgram(node.GetSubtree(0), mowerState, lawn, adfs);
        return EvaluateLawnMowerProgram(node.GetSubtree(1), mowerState, lawn, adfs);
      } else if (node.Symbol.Name == "Frog") {
        var p = EvaluateLawnMowerProgram(node.GetSubtree(0), mowerState, lawn, adfs);
        int x = p.Item1;
        int y = p.Item2;
        while (x < 0) x += lawn.GetLength(0);
        while (y < 0) y += lawn.GetLength(1);
        var newRow = (uint)((mowerState.Position.Item1 + x) % lawn.GetLength(0));
        var newCol = (uint)((mowerState.Position.Item2 + y) % lawn.GetLength(1));
        mowerState.Position = Tuple.Create(newRow, newCol);
        mowerState.Energy = mowerState.Energy - 1;
        lawn[newRow, newCol] = true;
        return Tuple.Create(0, 0);
      } else if (node.Symbol is InvokeFunction) {
        var invokeNode = node as InvokeFunctionTreeNode;

        // find the function definition for the invoke call
        var functionDefinition = (from adf in adfs.Cast<DefunTreeNode>()
                                  where adf.FunctionName == invokeNode.Symbol.FunctionName
                                  select adf).Single();
        // clone the function definition because we are replacing the argument nodes
        functionDefinition = (DefunTreeNode)functionDefinition.Clone();
        // find the argument tree nodes and their parents in the original function definition
        // toList is necessary to prevent that newly inserted branches are iterated
        var argumentCutPoints = (from parent in functionDefinition.IterateNodesPrefix()
                                 from subtree in parent.Subtrees
                                 where subtree is ArgumentTreeNode
                                 select new { Parent = parent, Argument = subtree.Symbol as Argument, ChildIndex = parent.IndexOfSubtree(subtree) })
                                 .ToList();
        // replace all argument tree ndoes with the matching argument of the invoke node
        foreach (var cutPoint in argumentCutPoints) {
          cutPoint.Parent.RemoveSubtree(cutPoint.ChildIndex);
          cutPoint.Parent.InsertSubtree(cutPoint.ChildIndex, (SymbolicExpressionTreeNode)invokeNode.GetSubtree(cutPoint.Argument.ArgumentIndex).Clone());
        }
        return EvaluateLawnMowerProgram(functionDefinition.GetSubtree(0), mowerState, lawn, adfs);
      } else {
        // try to parse as ephemeral random const with format: "x,y" (x, y in [0..32[)
        int x, y;
        var tokens = node.Symbol.Name.Split(',');
        if (tokens.Length == 2 &&
            int.TryParse(tokens[0], out x) &&
            int.TryParse(tokens[1], out y)) {
          return Tuple.Create(x, y);
        } else {
          throw new ArgumentException("Invalid symbol in the lawn mower program.");
        }
      }
    }
  }
}

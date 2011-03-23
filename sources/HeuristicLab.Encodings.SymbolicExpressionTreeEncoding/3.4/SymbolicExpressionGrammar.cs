#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public abstract class SymbolicExpressionGrammar : SymbolicExpressionGrammarBase, ISymbolicExpressionGrammar {
    #region fields & properties
    [Storable]
    private int minimumFunctionDefinitions;
    public int MinimumFunctionDefinitions {
      get { return minimumFunctionDefinitions; }
      set {
        minimumFunctionDefinitions = value;
        UpdateAdfConstraints();
      }
    }
    [Storable]
    private int maximumFunctionDefinitions;
    public int MaximumFunctionDefinitions {
      get { return maximumFunctionDefinitions; }
      set {
        maximumFunctionDefinitions = value;
        UpdateAdfConstraints();
      }
    }
    [Storable]
    private int minimumFunctionArguments;
    public int MinimumFunctionArguments {
      get { return minimumFunctionArguments; }
      set { minimumFunctionArguments = value; }
    }
    [Storable]
    private int maximumFunctionArguments;
    public int MaximumFunctionArguments {
      get { return maximumFunctionArguments; }
      set { maximumFunctionArguments = value; }
    }

    [Storable]
    private ProgramRootSymbol programRootSymbol;
    public ProgramRootSymbol ProgramRootSymbol {
      get { return programRootSymbol; }
    }
    ISymbol ISymbolicExpressionGrammar.ProgramRootSymbol {
      get { return ProgramRootSymbol; }
    }
    [Storable(Name = "ProgramRootSymbol")]
    private ISymbol StorableProgramRootSymbol {
      get { return programRootSymbol; }
      set { programRootSymbol = (ProgramRootSymbol)value; }
    }

    private StartSymbol startSymbol;
    public StartSymbol StartSymbol {
      get { return startSymbol; }
    }
    ISymbol ISymbolicExpressionGrammar.StartSymbol {
      get { return StartSymbol; }
    }
    [Storable(Name = "StartSymbol")]
    private ISymbol StorableStartSymbol {
      get { return startSymbol; }
      set { startSymbol = (StartSymbol)value; }
    }

    [Storable]
    private Defun defunSymbol;
    protected Defun DefunSymbol {
      get { return defunSymbol; }
    }
    [Storable(Name = "DefunSymbol")]
    private ISymbol StorableDefunSymbol {
      get { return defunSymbol; }
      set { defunSymbol = (Defun)value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionGrammar(SymbolicExpressionGrammar original, Cloner cloner)
      : base(original, cloner) {
      programRootSymbol = (ProgramRootSymbol)cloner.Clone(original.programRootSymbol);
      startSymbol = (StartSymbol)cloner.Clone(original.StartSymbol);
      defunSymbol = (Defun)cloner.Clone(original.defunSymbol);
      symbols = original.symbols
        .ToDictionary(x => x.Key, y => (ISymbol)cloner.Clone(y.Value));
      maximumFunctionArguments = original.maximumFunctionArguments;
      minimumFunctionArguments = original.minimumFunctionArguments;
      maximumFunctionDefinitions = original.maximumFunctionDefinitions;
      minimumFunctionDefinitions = original.minimumFunctionDefinitions;
    }

    public SymbolicExpressionGrammar(string name, string description)
      : base(name, description) {
      programRootSymbol = new ProgramRootSymbol();
      AddSymbol(programRootSymbol);
      SetSubtreeCount(programRootSymbol, 1, 1);

      startSymbol = new StartSymbol();
      AddSymbol(startSymbol);
      SetSubtreeCount(startSymbol, 1, 1);

      defunSymbol = new Defun();
      AddSymbol(defunSymbol);
      SetSubtreeCount(defunSymbol, 1, 1);

      AddAllowedChildSymbol(programRootSymbol, startSymbol, 0);
      UpdateAdfConstraints();
    }

    private void UpdateAdfConstraints() {
      SetSubtreeCount(programRootSymbol, minimumFunctionDefinitions + 1, maximumFunctionDefinitions + 1);

      // ADF branches maxFunctionDefinitions 
      for (int argumentIndex = 1; argumentIndex < maximumFunctionDefinitions + 1; argumentIndex++) {
        RemoveAllowedChildSymbol(programRootSymbol, defunSymbol, argumentIndex);
        AddAllowedChildSymbol(programRootSymbol, defunSymbol, argumentIndex);
      }
    }
  }
}

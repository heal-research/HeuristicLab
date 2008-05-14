using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;

namespace HeuristicLab.Functions {
  class EvaluatorSymbolTable : StorableBase{
    public const int ADDITION = 10010;
    public const int AND = 10020;
    public const int AVERAGE = 10030;
    public const int CONSTANT = 10040;
    public const int COSINUS = 10050;
    public const int DIVISION = 10060;
    public const int EQU = 10070;
    public const int EXP = 10080;
    public const int GT = 10090;
    public const int IFTE = 10100;
    public const int LT = 10110;
    public const int LOG = 10120;
    public const int MULTIPLICATION = 10130;
    public const int NOT = 10140;
    public const int OR = 10150;
    public const int POWER = 10160;
    public const int SIGNUM = 10170;
    public const int SINUS = 10180;
    public const int SQRT = 10190;
    public const int SUBSTRACTION = 10200;
    public const int TANGENS = 10210;
    public const int VARIABLE = 10220;
    public const int XOR = 10230;

    private int nextFunctionSymbol = 10240;
    private Dictionary<int, IFunction> table;
    private Dictionary<IFunction, int> reverseTable;
    private Dictionary<Type, int> staticTypes;

    private static EvaluatorSymbolTable symbolTable = new EvaluatorSymbolTable();
    public static EvaluatorSymbolTable SymbolTable {
      get { return EvaluatorSymbolTable.symbolTable; }
    }

    // needs to be public for persistence mechanism (Activator.CreateInstance needs empty constructor)
    public EvaluatorSymbolTable () {
      table = new Dictionary<int, IFunction>();
      reverseTable = new Dictionary<IFunction, int>();
      staticTypes = new Dictionary<Type, int>();
      staticTypes[typeof(Addition)] = ADDITION;
      staticTypes[typeof(And)] = AND;
      staticTypes[typeof(Average)] = AVERAGE;
      staticTypes[typeof(Constant)] = CONSTANT;
      staticTypes[typeof(Cosinus)] = COSINUS;
      staticTypes[typeof(Division)] = DIVISION;
      staticTypes[typeof(Equal)] = EQU;
      staticTypes[typeof(Exponential)] = EXP;
      staticTypes[typeof(GreaterThan)] = GT;
      staticTypes[typeof(IfThenElse)] = IFTE;
      staticTypes[typeof(LessThan)] = LT;
      staticTypes[typeof(Logarithm)] = LOG;
      staticTypes[typeof(Multiplication)] = MULTIPLICATION;
      staticTypes[typeof(Not)] = NOT;
      staticTypes[typeof(Or)] = OR;
      staticTypes[typeof(Power)] = POWER;
      staticTypes[typeof(Signum)] = SIGNUM;
      staticTypes[typeof(Sinus)] = SINUS;
      staticTypes[typeof(Sqrt)] = SQRT;
      staticTypes[typeof(Substraction)] = SUBSTRACTION;
      staticTypes[typeof(Tangens)] = TANGENS;
      staticTypes[typeof(Variable)] = VARIABLE;
      staticTypes[typeof(Xor)] = XOR;
    }

    internal int MapFunction(IFunction function) {
      if(!reverseTable.ContainsKey(function)) {
        int curFunctionSymbol;
        if(staticTypes.ContainsKey(function.GetType())) curFunctionSymbol = staticTypes[function.GetType()];
        else {
          curFunctionSymbol = nextFunctionSymbol;
          nextFunctionSymbol++;
        }
        reverseTable[function] = curFunctionSymbol;
        table[curFunctionSymbol] = function;
      }
      return reverseTable[function];
    }

    internal IFunction MapSymbol(int symbol) {
      return table[symbol];
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node =  base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute nextFunctionSymbolAttribute = document.CreateAttribute("NextFunctionSymbol");
      nextFunctionSymbolAttribute.Value = nextFunctionSymbol.ToString();
      node.Attributes.Append(nextFunctionSymbolAttribute);
      XmlNode symbolTableNode = document.CreateNode(XmlNodeType.Element, "Table", null);
      foreach(KeyValuePair<int, IFunction> entry in table) {
        XmlNode entryNode = PersistenceManager.Persist("Entry", entry.Value, document, persistedObjects);
        XmlAttribute symbolAttr = document.CreateAttribute("Symbol");
        symbolAttr.Value = entry.Key.ToString();
        entryNode.Attributes.Append(symbolAttr);
        symbolTableNode.AppendChild(entryNode);
      }
      node.AppendChild(symbolTableNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      table.Clear();
      reverseTable.Clear();
      nextFunctionSymbol = int.Parse(node.Attributes["NextFunctionSymbol"].Value);
      XmlNode symbolTableNode = node.SelectSingleNode("Table");
      foreach(XmlNode entry in symbolTableNode.ChildNodes) {
        IFunction function = (IFunction)PersistenceManager.Restore(entry, restoredObjects);
        int symbol = int.Parse(entry.Attributes["Symbol"].Value);
        table[symbol] = function;
        reverseTable[function] = symbol;
      }
    }
  }
}

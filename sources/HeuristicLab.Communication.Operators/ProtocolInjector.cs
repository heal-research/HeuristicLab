using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class ProtocolInjector : OperatorBase {
    public override string Description {
      get { return "Injects a protocol into the data tree"; }
    }

    public ProtocolInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Protocol", "The protocol to be used for communication", typeof(Protocol), VariableKind.New));
      AddVariable(new Variable("Protocol", new Protocol()));
      AddVariableInfo(new VariableInfo("Dictionary", "The dictionary to translate received variables names", typeof(ItemDictionary<StringData, StringData>), VariableKind.New));
      AddVariable(new Variable("Dictionary", new ItemDictionary<StringData, StringData>()));
    }

    public override IView CreateView() {
      return new ProtocolInjectorView(this);
    }

    public override IOperation Apply(IScope scope) {
      IDictionary<Guid, object> clonedObjects = new Dictionary<Guid, object>();
      scope.AddVariable(new Variable(scope.TranslateName("Protocol"), (Protocol)GetVariable("Protocol").Value.Clone(clonedObjects)));
      scope.AddVariable(new Variable(scope.TranslateName("Dictionary"), (ItemDictionary<StringData, StringData>)GetVariable("Dictionary").Value.Clone()));
      return null;
    }
  }
}

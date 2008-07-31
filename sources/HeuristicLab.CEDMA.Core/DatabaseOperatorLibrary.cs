using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Core {
  public class DatabaseOperatorLibrary : ItemBase, IOperatorLibrary {

    private OperatorGroup group;
    public IOperatorGroup Group {
      get { return group; }
    }

    private string dbUri;

    public DatabaseOperatorLibrary(string dbUri)
      : base() {
      this.dbUri = dbUri;
      group = new OperatorGroup();
    }


    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      throw new NotSupportedException();
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      throw new NotSupportedException();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      throw new NotSupportedException();
    }

    public override IView CreateView() {
      // return new DatabaseOperatorLibraryView(this);
      return null;
    }
  }
}

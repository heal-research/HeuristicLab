namespace HeuristicLab.Problems.Instances.Orienteering {
  internal class OPDataDescriptor : IDataDescriptor {
    public string Name { get; internal set; }
    public string Description { get; internal set; }

    internal string InstanceIdentifier { get; set; }

    internal OPDataDescriptor(string name, string description, string instanceIdentifier) {
      Name = name;
      Description = description;
      InstanceIdentifier = instanceIdentifier;
    }
  }
}
using System.Collections.Generic;
using System;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using HeuristicLab.Tracing;
using log4net;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Default.Xml {

  struct XmlStringConstants {
    public const string PRIMITIVE = "PRIMITIVE";
    public const string COMPOSITE = "COMPOSITE";
    public const string REFERENCE = "REFERENCE";
    public const string NULL = "NULL";
    public const string TYPECACHE = "TYPECACHE";
    public const string TYPE = "TYPE";
    public const string METAINFO = "METAINFO";
  }
  
}
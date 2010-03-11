using System.Collections.Generic;
using System;
using System.Text;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using HeuristicLab.Tracing;
using HeuristicLab.Persistence.Core.Tokens;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Base class for serialization generators. Provides a common entry point
  /// <code>Format</code> and dispatches to different abstract methods for
  /// every token type.
  /// </summary>  
  public abstract class GeneratorBase<T> {

    public T Format(ISerializationToken token) {
      Type type = token.GetType();
      if (type == typeof(BeginToken))
        return Format((BeginToken)token);
      if (type == typeof(EndToken))
        return Format((EndToken)token);
      if (type == typeof(PrimitiveToken))
        return Format((PrimitiveToken)token);
      if (type == typeof(ReferenceToken))
        return Format((ReferenceToken)token);
      if (type == typeof(NullReferenceToken))
        return Format((NullReferenceToken)token);
      if (type == typeof(MetaInfoBeginToken))
        return Format((MetaInfoBeginToken)token);
      if (type == typeof(MetaInfoEndToken))
        return Format((MetaInfoEndToken)token);
      throw new ApplicationException("Invalid token of type " + type.FullName);
    }

    protected abstract T Format(BeginToken beginToken);
    protected abstract T Format(EndToken endToken);
    protected abstract T Format(PrimitiveToken primitiveToken);
    protected abstract T Format(ReferenceToken referenceToken);
    protected abstract T Format(NullReferenceToken nullReferenceToken);
    protected abstract T Format(MetaInfoBeginToken metaInfoBeginToken);
    protected abstract T Format(MetaInfoEndToken metaInfoEndToken);

  }
}
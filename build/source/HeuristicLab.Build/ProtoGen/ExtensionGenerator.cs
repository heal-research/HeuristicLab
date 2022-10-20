#region Copyright notice and license

// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
// http://github.com/jskeet/dotnet-protobufs/
// Original C++/Java/Python code:
// http://code.google.com/p/protobuf/
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using Google.ProtocolBuffers.Descriptors;

namespace Google.ProtocolBuffers.ProtoGen
{
    internal class ExtensionGenerator : FieldGeneratorBase, ISourceGenerator
    {
        private readonly string extends;
        private readonly string scope;
        private readonly string type;
        private readonly string name;

        internal ExtensionGenerator(FieldDescriptor descriptor)
            : base(descriptor, 0)
        {
            if (Descriptor.ExtensionScope != null)
            {
                scope = GetClassName(Descriptor.ExtensionScope);
            }
            else
            {
                scope = DescriptorUtil.GetFullUmbrellaClassName(Descriptor.File);
            }
            switch (Descriptor.MappedType)
            {
                case MappedType.Message:
                    type = GetClassName(Descriptor.MessageType);
                    break;
                case MappedType.Enum:
                    type = GetClassName(Descriptor.EnumType);
                    break;
                default:
                    type = DescriptorUtil.GetMappedTypeName(Descriptor.MappedType);
                    break;
            }
            extends = GetClassName(Descriptor.ContainingType);
            name = Descriptor.CSharpOptions.PropertyName;
        }

        public void Generate(TextGenerator writer)
        {
            if (Descriptor.File.CSharpOptions.ClsCompliance && GetFieldConstantName(Descriptor).StartsWith("_"))
            {
                writer.WriteLine("[global::System.CLSCompliant(false)]");
            }

            writer.WriteLine("public const int {0} = {1};", GetFieldConstantName(Descriptor), Descriptor.FieldNumber);

            if (UseLiteRuntime)
            {
                if (Descriptor.MappedType == MappedType.Message && Descriptor.MessageType.Options.MessageSetWireFormat)
                {
                    throw new ArgumentException(
                        "option message_set_wire_format = true; is not supported in Lite runtime extensions.");
                }
                if (!Descriptor.IsCLSCompliant && Descriptor.File.CSharpOptions.ClsCompliance)
                {
                    writer.WriteLine("[global::System.CLSCompliant(false)]");
                }
                writer.WriteLine("{0} static pb::{4}<{1}, {2}> {3};", ClassAccessLevel, extends, type, name,
                                 Descriptor.IsRepeated ? "GeneratedRepeatExtensionLite" : "GeneratedExtensionLite");
            }
            else if (Descriptor.IsRepeated)
            {
                if (!Descriptor.IsCLSCompliant && Descriptor.File.CSharpOptions.ClsCompliance)
                {
                    writer.WriteLine("[global::System.CLSCompliant(false)]");
                }
                writer.WriteLine("{0} static pb::GeneratedExtensionBase<scg::IList<{1}>> {2};", ClassAccessLevel, type,
                                 name);
            }
            else
            {
                if (!Descriptor.IsCLSCompliant && Descriptor.File.CSharpOptions.ClsCompliance)
                {
                    writer.WriteLine("[global::System.CLSCompliant(false)]");
                }
                writer.WriteLine("{0} static pb::GeneratedExtensionBase<{1}> {2};", ClassAccessLevel, type, name);
            }
        }

        internal void GenerateStaticVariableInitializers(TextGenerator writer)
        {
            if (UseLiteRuntime)
            {
                writer.WriteLine("{0}.{1} = ", scope, name);
                writer.Indent();
                writer.WriteLine("new pb::{0}<{1}, {2}>(",
                                 Descriptor.IsRepeated ? "GeneratedRepeatExtensionLite" : "GeneratedExtensionLite",
                                 extends, type);
                writer.Indent();
                writer.WriteLine("\"{0}\",", Descriptor.FullName);
                writer.WriteLine("{0}.DefaultInstance,", extends);
                if (!Descriptor.IsRepeated)
                {
                    writer.WriteLine("{0},",
                                     Descriptor.HasDefaultValue
                                         ? DefaultValue
                                         : IsNullableType ? "null" : "default(" + type + ")");
                }
                writer.WriteLine("{0},",
                                 (Descriptor.MappedType == MappedType.Message) ? type + ".DefaultInstance" : "null");
                writer.WriteLine("{0},",
                                 (Descriptor.MappedType == MappedType.Enum) ? "new EnumLiteMap<" + type + ">()" : "null");
                writer.WriteLine("{0}.{1}FieldNumber,", scope, name);
                writer.Write("pbd::FieldType.{0}", Descriptor.FieldType);
                if (Descriptor.IsRepeated)
                {
                    writer.WriteLine(",");
                    writer.Write(Descriptor.IsPacked ? "true" : "false");
                }
                writer.Outdent();
                writer.WriteLine(");");
                writer.Outdent();
            }
            else if (Descriptor.IsRepeated)
            {
                writer.WriteLine(
                    "{0}.{1} = pb::GeneratedRepeatExtension<{2}>.CreateInstance({0}.Descriptor.Extensions[{3}]);", scope,
                    name, type, Descriptor.Index);
            }
            else
            {
                writer.WriteLine(
                    "{0}.{1} = pb::GeneratedSingleExtension<{2}>.CreateInstance({0}.Descriptor.Extensions[{3}]);", scope,
                    name, type, Descriptor.Index);
            }
        }

        internal void GenerateExtensionRegistrationCode(TextGenerator writer)
        {
            writer.WriteLine("registry.Add({0}.{1});", scope, name);
        }

        public override void WriteHash(TextGenerator writer)
        {
        }

        public override void WriteEquals(TextGenerator writer)
        {
        }

        public override void WriteToString(TextGenerator writer)
        {
        }
    }
}
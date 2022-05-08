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
using System;
using System.Collections;
using System.Collections.Generic;
using Google.ProtocolBuffers.Descriptors;

namespace Google.ProtocolBuffers.ProtoGen
{
    /// <summary>
    /// Generator for the class describing the .proto file in general,
    /// containing things like the message descriptor.
    /// </summary>
    internal sealed class UmbrellaClassGenerator : SourceGeneratorBase<FileDescriptor>, ISourceGenerator
    {
        internal UmbrellaClassGenerator(FileDescriptor descriptor)
            : base(descriptor)
        {
        }

        // Recursively searches the given message to see if it contains any extensions.
        private static bool UsesExtensions(IMessage message)
        {
            // We conservatively assume that unknown fields are extensions.
            if (message.UnknownFields.FieldDictionary.Count > 0)
            {
                return true;
            }

            foreach (KeyValuePair<FieldDescriptor, object> keyValue in message.AllFields)
            {
                FieldDescriptor field = keyValue.Key;
                if (field.IsExtension)
                {
                    return true;
                }
                if (field.MappedType == MappedType.Message)
                {
                    if (field.IsRepeated)
                    {
                        foreach (IMessage subMessage in (IEnumerable) keyValue.Value)
                        {
                            if (UsesExtensions(subMessage))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (UsesExtensions((IMessage) keyValue.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Generate(TextGenerator writer)
        {
            WriteIntroduction(writer);
            WriteExtensionRegistration(writer);
            WriteChildren(writer, "Extensions", Descriptor.Extensions);
            writer.WriteLine("#region Static variables");
            foreach (MessageDescriptor message in Descriptor.MessageTypes)
            {
                new MessageGenerator(message).GenerateStaticVariables(writer);
            }
            writer.WriteLine("#endregion");
            if (!UseLiteRuntime)
            {
                WriteDescriptor(writer);
            }
            else
            {
                WriteLiteExtensions(writer);
            }
            // The class declaration either gets closed before or after the children are written.
            if (!Descriptor.CSharpOptions.NestClasses)
            {
                writer.Outdent();
                writer.WriteLine("}");

                // Close the namespace around the umbrella class if defined
                if (!Descriptor.CSharpOptions.NestClasses && Descriptor.CSharpOptions.UmbrellaNamespace != "")
                {
                    writer.Outdent();
                    writer.WriteLine("}");
                }
            }
            WriteChildren(writer, "Enums", Descriptor.EnumTypes);
            WriteChildren(writer, "Messages", Descriptor.MessageTypes);
            WriteChildren(writer, "Services", Descriptor.Services);
            if (Descriptor.CSharpOptions.NestClasses)
            {
                writer.Outdent();
                writer.WriteLine("}");
            }
            if (Descriptor.CSharpOptions.Namespace != "")
            {
                writer.Outdent();
                writer.WriteLine("}");
            }
            writer.WriteLine();
            writer.WriteLine("#endregion Designer generated code");
        }

        private void WriteIntroduction(TextGenerator writer)
        {
            writer.WriteLine("// Generated by {0}.  DO NOT EDIT!", this.GetType().Assembly.FullName);
            writer.WriteLine("#pragma warning disable 1591, 0612");
            writer.WriteLine("#region Designer generated code");

            writer.WriteLine();
            writer.WriteLine("using pb = global::Google.ProtocolBuffers;");
            writer.WriteLine("using pbc = global::Google.ProtocolBuffers.Collections;");
            writer.WriteLine("using pbd = global::Google.ProtocolBuffers.Descriptors;");
            writer.WriteLine("using scg = global::System.Collections.Generic;");

            if (Descriptor.CSharpOptions.Namespace != "")
            {
                writer.WriteLine("namespace {0} {{", Descriptor.CSharpOptions.Namespace);
                writer.Indent();
                writer.WriteLine();
            }
            // Add the namespace around the umbrella class if defined
            if (!Descriptor.CSharpOptions.NestClasses && Descriptor.CSharpOptions.UmbrellaNamespace != "")
            {
                writer.WriteLine("namespace {0} {{", Descriptor.CSharpOptions.UmbrellaNamespace);
                writer.Indent();
                writer.WriteLine();
            }

            if (Descriptor.CSharpOptions.CodeContracts)
            {
                writer.WriteLine("[global::System.Diagnostics.Contracts.ContractVerificationAttribute(false)]");
            }
            writer.WriteLine("[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]");
            writer.WriteLine("[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]");
            writer.WriteLine("[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{0}\", \"{1}\")]",
                             GetType().Assembly.GetName().Name, GetType().Assembly.GetName().Version);
            writer.WriteLine("{0} static partial class {1} {{", ClassAccessLevel,
                             Descriptor.CSharpOptions.UmbrellaClassname);
            writer.WriteLine();
            writer.Indent();
        }

        private void WriteExtensionRegistration(TextGenerator writer)
        {
            writer.WriteLine("#region Extension registration");
            writer.WriteLine("public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {");
            writer.Indent();
            foreach (FieldDescriptor extension in Descriptor.Extensions)
            {
                new ExtensionGenerator(extension).GenerateExtensionRegistrationCode(writer);
            }
            foreach (MessageDescriptor message in Descriptor.MessageTypes)
            {
                new MessageGenerator(message).GenerateExtensionRegistrationCode(writer);
            }
            writer.Outdent();
            writer.WriteLine("}");
            writer.WriteLine("#endregion");
        }

        private void WriteDescriptor(TextGenerator writer)
        {
            writer.WriteLine("#region Descriptor");

            writer.WriteLine("public static pbd::FileDescriptor Descriptor {");
            writer.WriteLine("  get { return descriptor; }");
            writer.WriteLine("}");
            writer.WriteLine("private static pbd::FileDescriptor descriptor;");
            writer.WriteLine();
            writer.WriteLine("static {0}() {{", Descriptor.CSharpOptions.UmbrellaClassname);
            writer.Indent();
            writer.WriteLine("byte[] descriptorData = global::System.Convert.FromBase64String(");
            writer.Indent();
            writer.Indent();

            // TODO(jonskeet): Consider a C#-escaping format here instead of just Base64.
            byte[] bytes = Descriptor.Proto.ToByteArray();
            string base64 = Convert.ToBase64String(bytes);

            while (base64.Length > 60)
            {
                writer.WriteLine("\"{0}\" + ", base64.Substring(0, 60));
                base64 = base64.Substring(60);
            }
            writer.WriteLine("\"{0}\");", base64);
            writer.Outdent();
            writer.Outdent();
            writer.WriteLine(
                "pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {");
            writer.Indent();
            writer.WriteLine("descriptor = root;");
            foreach (MessageDescriptor message in Descriptor.MessageTypes)
            {
                new MessageGenerator(message).GenerateStaticVariableInitializers(writer);
            }
            foreach (FieldDescriptor extension in Descriptor.Extensions)
            {
                new ExtensionGenerator(extension).GenerateStaticVariableInitializers(writer);
            }

            if (UsesExtensions(Descriptor.Proto))
            {
                // Must construct an ExtensionRegistry containing all possible extensions
                // and return it.
                writer.WriteLine("pb::ExtensionRegistry registry = pb::ExtensionRegistry.CreateInstance();");
                writer.WriteLine("RegisterAllExtensions(registry);");
                foreach (FileDescriptor dependency in Descriptor.Dependencies)
                {
                    writer.WriteLine("{0}.RegisterAllExtensions(registry);",
                                     DescriptorUtil.GetFullUmbrellaClassName(dependency));
                }
                writer.WriteLine("return registry;");
            }
            else
            {
                writer.WriteLine("return null;");
            }
            writer.Outdent();
            writer.WriteLine("};");

            // -----------------------------------------------------------------
            // Invoke internalBuildGeneratedFileFrom() to build the file.
            writer.WriteLine("pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,");
            writer.WriteLine("    new pbd::FileDescriptor[] {");
            foreach (FileDescriptor dependency in Descriptor.Dependencies)
            {
                writer.WriteLine("    {0}.Descriptor, ", DescriptorUtil.GetFullUmbrellaClassName(dependency));
            }
            writer.WriteLine("    }, assigner);");
            writer.Outdent();
            writer.WriteLine("}");
            writer.WriteLine("#endregion");
            writer.WriteLine();
        }

        private void WriteLiteExtensions(TextGenerator writer)
        {
            writer.WriteLine("#region Extensions");
            writer.WriteLine("internal static readonly object Descriptor;");
            writer.WriteLine("static {0}() {{", Descriptor.CSharpOptions.UmbrellaClassname);
            writer.Indent();
            writer.WriteLine("Descriptor = null;");

            foreach (MessageDescriptor message in Descriptor.MessageTypes)
            {
                new MessageGenerator(message).GenerateStaticVariableInitializers(writer);
            }
            foreach (FieldDescriptor extension in Descriptor.Extensions)
            {
                new ExtensionGenerator(extension).GenerateStaticVariableInitializers(writer);
            }
            writer.Outdent();
            writer.WriteLine("}");
            writer.WriteLine("#endregion");
            writer.WriteLine();
        }
    }
}
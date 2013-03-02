﻿namespace VisualMutator.Model.Mutations.Operators
{
    using System;
    using System.IO;
    using System.Linq;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;

    public class OperatorUtils : IOperatorUtils
    {
        private readonly ICommonCompilerAssemblies _cci;

        public OperatorUtils(ICommonCompilerAssemblies cci)
        {
            _cci = cci;
        }


        public IModule CompileModuleFromCode(string code, IMetadataReaderHost host)
        {
           // CoreAssembly = Host.LoadAssembly(Host.CoreAssemblySymbolicIdentity);

            var tree = SyntaxTree.ParseText(code);

            var comp = Compilation.Create("MyCompilation",
                new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree)
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location));

            var outputFileName = Path.Combine(Path.GetTempPath(), "MyCompilation.lib");
            var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);

            var result = comp.Emit(ilStream);
            ilStream.Close();
            if (!result.Success)
            {
                var aggregate = result.Diagnostics.Select(a => a.Info.GetMessage()).Aggregate((a, b) => a + "\n" + b);
                throw new InvalidProgramException(aggregate);
            }
            //using (var host = new PeReader.DefaultHost())
           // {
                var module = host.LoadUnitFrom(outputFileName) as IModule;
                if (module == null || module == Dummy.Module || module == Dummy.Assembly)
                {
                    throw new InvalidOperationException(outputFileName + " is not a PE file containing a CLR module or assembly.");
                }


                return Decompiler.GetCodeModelFromMetadataModel(host, module, null);
              

          //  }
        }
    }
}
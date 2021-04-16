using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IoFluently;
using ReactiveProcesses;

namespace LiveLinq.CodeGenerator
{
    class Program
    {
	    static void Main(string[] args)
	    {
		    // var ioService = new IoService(new ReactiveProcessFactory());
		    // var repoRoot = ioService.CurrentDirectory.Ancestors().First(ancestor => (ancestor / ".git").IsFolder());
		    //
		    // var solutionFile = repoRoot / "src" / "LiveLinq.sln";
		    // var liveLinqCsproj = repoRoot / "src" / "LiveLinq" / "LiveLinq.csproj";
		    //
		    // var codeIndexBuilder = new CodeIndexBuilder();
		    // codeIndexBuilder.AddProject(solutionFile.ToString(), liveLinqCsproj.ToString());
		    //
		    // var codeIndex = codeIndexBuilder.Build();
		    //
		    // var readOnlyComposableDictionary = codeIndex.GetValue(new TypeIdentifier
		    // {
			   //  Namespace = "ComposableCollections.Dictionary.Interfaces",
			   //  Name = "IReadOnlyComposableDictionary",
			   //  Arity = 2,
		    // });
	    }
    }
}
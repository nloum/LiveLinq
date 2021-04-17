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
		    var ioService = new IoService(new ReactiveProcessFactory());
		    var repoRoot = ioService.CurrentDirectory.Ancestors().First(ancestor => (ancestor / ".git").IsFolder());

		    var generatedFile = repoRoot / "src" / "LiveLinq" / "Generated.cs";

		    using (var writer = generatedFile.OpenWriter())
		    {
			    writer.WriteLine(@"using System;
using System.Reactive.Subjects;
using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Adapters;
using LiveLinq.Dictionary.Interfaces;");
			    writer.WriteLine();
			    writer.WriteLine("namespace LiveLinq {");
			    writer.WriteLine("public static partial class Extensions {");
			    
			    writer.WriteLine();
			    writer.WriteLine("public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableDictionary<TKey, TValue> source) {");
			    writer.WriteLine("return new ObservableDictionaryAdapter<TKey, TValue>(source);");
			    writer.WriteLine("}");
			    writer.WriteLine();
			    
			    writer.WriteLine();
			    writer.WriteLine("public static IObservableDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableDictionary<TKey, TValue> source, Subject<IDictionaryChangeStrict<TKey, TValue>> subject) {");
			    writer.WriteLine("return new ObservableDictionaryAdapter<TKey, TValue>(source, subject);");
			    writer.WriteLine("}");
			    writer.WriteLine();

			    writer.WriteLine();
			    writer.WriteLine("public static IObservableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableReadOnlyDictionary<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable, Action<IDictionaryChangeStrict<TKey, TValue>> onChange) {");
			    writer.WriteLine("return new ObservableDictionaryAdapter<TKey, TValue>(source, observable, onChange);");
			    writer.WriteLine("}");
			    writer.WriteLine();
			    
			    writer.WriteLine();
			    writer.WriteLine("public static IObservableReadOnlyDictionary<TKey, TValue> WithLiveLinq<TKey, TValue>(this IComposableDictionary<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable) {");
			    writer.WriteLine("return new ObservableReadOnlyDictionaryAdapter<TKey, TValue>(source, observable);");
			    writer.WriteLine("}");
			    writer.WriteLine();
			    
			    writer.WriteLine("}");
			    writer.WriteLine("}");
		    }

		    //var solutionFile = repoRoot / "src" / "LiveLinq.sln";
		    //var liveLinqCsproj = repoRoot / "src" / "LiveLinq" / "LiveLinq.csproj";
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
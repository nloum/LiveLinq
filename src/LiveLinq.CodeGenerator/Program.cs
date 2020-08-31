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
        private static void GenerateWithChangeNotificationsExtensionMethods(TextWriter textWriter)
	    {
		    var interfaces = new List<string>
		    {
			    "ICachedDictionary",
			    "ICachedDisposableDictionary",
			    "ICachedDisposableQueryableDictionary",
			    "ICachedQueryableDictionary",
			    "IComposableDictionary",
			    "IComposableReadOnlyDictionary",
			    "IDisposableDictionary",
			    "IDisposableQueryableDictionary",
			    "IDisposableQueryableReadOnlyDictionary",
			    "IDisposableReadOnlyDictionary",
			    "IQueryableDictionary",
			    "IQueryableReadOnlyDictionary"
		    };

		    textWriter.WriteLine("#region WithChangeNotifications");
		    
		    foreach (var iface in interfaces)
		    {
			    if (iface.Contains("ReadOnly"))
			    {
				    continue;
			    }

			    var observableInterface = "IObservable" + iface.Substring(1).Replace("Composable", "").Replace("Disposable", "");
			    
			    textWriter.WriteLine(
				    $"public static {observableInterface}<TKey, TValue> WithChangeNotifications<TKey, TValue>(this {iface}<TKey, TValue> source) {{");

			    textWriter.WriteLine("var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);");
			    
			    var arguments = new List<string>();

			    arguments.Add("decorated");
			    if (iface.Contains("Cached"))
			    {
				    arguments.Add("source.AsBypassCache");
				    arguments.Add("source.AsNeverFlush");
				    arguments.Add("source.FlushCache");
				    arguments.Add("source.GetWrites");
			    }

			    if (iface.Contains("Disposable"))
			    {
				    arguments.Add("source");
			    }
			    else
			    {
				    arguments.Add("Disposable.Empty");
			    }
			    
			    if (iface.Contains("Queryable"))
			    {
				    arguments.Add("source.Values");
			    }
			    
			    arguments.Add("decorated.ToLiveLinq");
			    
			    var adapterClassName = $"Observable{iface.Substring(1)}Adapter".Replace("Composable", "").Replace("Disposable", "");
			    textWriter.WriteLine($"    return new {adapterClassName}<TKey, TValue>({string.Join(", ", arguments)});");

			    textWriter.WriteLine("}");
		    }

 		    textWriter.WriteLine("#endregion\n");
            
            textWriter.WriteLine("#region WithChangeNotifications with separate observable and observer for sharing changes across dictionaries");
		    
            foreach (var iface in interfaces)
            {
	            if (iface.Contains("ReadOnly"))
	            {
		            continue;
	            }

	            var observableInterface = "IObservable" + iface.Substring(1).Replace("Composable", "").Replace("Disposable", "");
			    
	            textWriter.WriteLine(
		            $"public static {observableInterface}<TKey, TValue> WithChangeNotifications<TKey, TValue>(this {iface}<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable, Action<IDictionaryChangeStrict<TKey, TValue>> onChange) {{");

	            textWriter.WriteLine("var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);");
			    
	            var arguments = new List<string>();

	            arguments.Add("decorated");
	            if (iface.Contains("Cached"))
	            {
		            arguments.Add("source.AsBypassCache");
		            arguments.Add("source.AsNeverFlush");
		            arguments.Add("source.FlushCache");
		            arguments.Add("source.GetWrites");
	            }

	            if (iface.Contains("Disposable"))
	            {
		            arguments.Add("source");
	            }
	            else
	            {
		            arguments.Add("Disposable.Empty");
	            }
			    
	            if (iface.Contains("Queryable"))
	            {
		            arguments.Add("source.Values");
	            }
			    
	            arguments.Add("decorated.ToLiveLinq");
			    
	            var adapterClassName = $"Observable{iface.Substring(1)}Adapter".Replace("Composable", "").Replace("Disposable", "");
	            textWriter.WriteLine($"    return new {adapterClassName}<TKey, TValue>({string.Join(", ", arguments)});");

	            textWriter.WriteLine("}");
            }

            textWriter.WriteLine("#endregion\n");

            textWriter.WriteLine("#region WithChangeNotifications for read-only observables");
		    
            foreach (var iface in interfaces)
            {
	            if (!iface.Contains("ReadOnly"))
	            {
		            continue;
	            }

	            var observableInterface = "IObservable" + iface.Substring(1).Replace("Composable", "").Replace("Disposable", "");
			    
	            textWriter.WriteLine(
		            $"public static {observableInterface}<TKey, TValue> WithChangeNotifications<TKey, TValue>(this {iface}<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> changes) {{");
			    
	            var arguments = new List<string>();

	            arguments.Add("source");
	            if (iface.Contains("Cached"))
	            {
		            arguments.Add("source.AsBypassCache");
		            arguments.Add("source.AsNeverFlush");
		            arguments.Add("source.FlushCache");
		            arguments.Add("source.GetWrites");
	            }

	            if (iface.Contains("Disposable"))
	            {
		            arguments.Add("source");
	            }
	            else
	            {
		            arguments.Add("Disposable.Empty");
	            }
	            if (iface.Contains("Queryable"))
	            {
		            arguments.Add("source.Values");
	            }
	            arguments.Add("() => changes.ToLiveLinq()");

	            var adapterClassName = $"Observable{iface.Substring(1)}Adapter".Replace("Composable", "").Replace("Disposable", "");
	            textWriter.WriteLine($"    return new {adapterClassName}<TKey, TValue>({string.Join(", ", arguments)});");

	            textWriter.WriteLine("}");
            }

            textWriter.WriteLine("#endregion\n");
            
            textWriter.WriteLine("#region WithChangeNotifications - transactional");
		    
            foreach (var iface in interfaces)
            {
	            if (!iface.Contains("Disposable") || iface.Contains("ReadOnly"))
	            {
		            continue;
	            }
			    
	            var parameters = new List<string>();
	            var readOnlyArguments = new List<string>();
	            var readWriteArguments = new List<string>();

		        parameters.Add("IObservable<IDictionaryChangeStrict<TKey, TValue>> observable");
		        parameters.Add("Action<IDictionaryChangeStrict<TKey, TValue>> onChange");
		        
		        readOnlyArguments.Add("observable");
		        readWriteArguments.Add("observable");
		        readWriteArguments.Add("onChange");

	            var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");

	            var readOnlyObservableInterface =
		            $"IObservable{readOnlyInterface.Substring(1)}".Replace("Disposable", "");
	            var readWriteObservableInterface = $"IObservable{iface.Substring(1)}"
		            .Replace("Disposable", "");	            
	            textWriter.WriteLine(
		            $"public static IReadWriteFactory<{readOnlyObservableInterface}<TKey, TValue>, {readWriteObservableInterface}<TKey, TValue>> WithChangeNotifications<TKey, TValue>(this IReadWriteFactory<{readOnlyInterface}<TKey, TValue>, {iface}<TKey, TValue>> source, {string.Join(", ", parameters)}) {{");

	            textWriter.WriteLine(
		            $"return new AnonymousReadWriteFactory<{readOnlyObservableInterface}<TKey, TValue>, {readWriteObservableInterface}<TKey, TValue>>(");
	            textWriter.WriteLine(
		            $"() => source.CreateReader().WithChangeNotifications({string.Join(", ", readOnlyArguments)}),");
	            textWriter.WriteLine(
		            $"() => source.CreateWriter().WithChangeNotifications({string.Join(", ", readWriteArguments)}));");
			    
	            textWriter.WriteLine("}");
            }

            textWriter.WriteLine("#endregion");
	    }
	    
	    static void Main(string[] args)
	    {
		    var ioService = new IoService(new ReactiveProcessFactory());
		    var repoRoot = ioService.CurrentDirectory.Ancestors().First(ancestor => (ancestor / ".git").IsFolder());

		    var dictionaryExtensionsFilePath = repoRoot / "src" / "LiveLinq" / "DictionaryExtensions.g.cs";
		    using (var streamWriter = dictionaryExtensionsFilePath.OpenWriter())
		    {
			    streamWriter.WriteLine(@"using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Adapters;
using LiveLinq.Dictionary.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Decorators;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Sources;
using ComposableCollections.Dictionary.Transactional;
using ComposableCollections.Dictionary.WithBuiltInKey;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;
using UtilityDisposables;

namespace LiveLinq
{
	public static partial class DictionaryExtensions
    {");
			    GenerateWithChangeNotificationsExtensionMethods(streamWriter);
			    streamWriter.WriteLine("}\n}");
		    }
	    }
    }
}
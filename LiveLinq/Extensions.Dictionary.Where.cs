using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveLinq.Dictionary;
using LiveLinq.List;

using MoreCollections;

using static SimpleMonads.Utility;
using static MoreCollections.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IDictionaryChanges<TKey, TValue> Where<TKey, TValue>(
                  this IDictionaryChanges<TKey, TValue> source,
                  Func<TKey, TValue, IObservable<bool>> selector)
        {
            return source.SelectMany((key, value) => selector(key, value)
                    .DistinctUntilChanged()
                    .Select(included => included ? Something(KeyValuePair<TKey, TValue>(key, value)) : Nothing<IKeyValuePair<TKey, TValue>>())
                .ToLiveLinq());
        }
        
        public static IDictionaryChanges<TKey, TValue> Where<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            Func<TKey, TValue, bool> selector)
        {
            return source.Where((key, value) => Observable.Return(selector(key, value)));
        }
    }
}

﻿using System;
using System.Reactive.Linq;
using static SimpleMonads.Utility;

namespace LiveLinq.List
{
    public static partial class Extensions
    {
        public static IListChanges<TSource> Where<TSource>(
               this IListChanges<TSource> source,
               Func<TSource, IObservable<bool>> selector)
        {
            return source.SelectMany(src => selector(src)
                    .Select(included => included ? Something(src) : Nothing<TSource>())
                .ToLiveLinq());
        }

        public static IListChanges<TSource> Where<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IObservable<bool>> selector)
        {
            return source.SelectMany((src, idx) => selector(src, idx)
                    .Select(included => included ? Something(src) : Nothing<TSource>())
                .ToLiveLinq());
        }

        public static IListChanges<TSource> Where<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, bool> selector)
        {
            return source.SelectMany(src =>
                Observable.Return(selector(src) ? Something(src) : Nothing<TSource>()).ToLiveLinq());
        }

        public static IListChanges<TSource> Where<TSource>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, bool> selector)
        {
            return source.SelectMany((src, idx) => Observable.Return(selector(src, idx)
                    ? Something(src) : Nothing<TSource>()).ToLiveLinq());
        }
    }
}

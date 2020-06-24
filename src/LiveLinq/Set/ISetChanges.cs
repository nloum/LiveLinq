using System;

namespace LiveLinq.Set
{
    public interface ISetChanges<out T>
    {
        /// <summary>
        /// An event stream of changes that, when applied to a list, result in a list that represents the most
        /// recent state of the query.
        /// 
        /// Note: the first event should ALWAYS represent the actual state of the list at that time. If the initial state
        /// of the list is empty, the first event will be an empty add event.
        /// </summary>
        /// <returns></returns>
        IObservable<ISetChange<T>> AsObservable();

        /// <summary>
        /// The reason that the OfType method is a member of <see cref="ISetChanges{T}"/>
        /// and not an extension method is because if it were an extension method then it would need two generic parameters,
        /// one of which would not be inferrable by the compiler, resulting in developers having to always specify
        /// both type parameters. This would be clunky and very unlike the LINQ OfType method.
        /// </summary>
        ISetChanges<TTarget> OfType<TTarget>();

        /// <summary>
        /// The reason that the Cast method is a member of <see cref="ISetChanges{T}"/>
        /// and not an extension method is because if it were an extension method then it would need two generic parameters,
        /// one of which would not be inferrable by the compiler, resulting in developers having to always specify
        /// both type parameters. This would be clunky and very unlike the LINQ Cast method.
        /// </summary>
        ISetChanges<TTarget> Cast<TTarget>();
    }
}

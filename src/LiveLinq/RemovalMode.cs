using SimpleMonads;
using System;

namespace LiveLinq
{
    public class RemovalMode : Either<ExplicitRemoval, UnsubscribeRemoval, CompleteRemoval, ErrorRemoval>
    {
        private RemovalMode(ExplicitRemoval item1) : base(item1)
        {
        }

        private RemovalMode(UnsubscribeRemoval item2) : base(item2)
        {
        }

        private RemovalMode(CompleteRemoval item3) : base(item3)
        {
        }

        private RemovalMode(ErrorRemoval item4) : base(item4)
        {
        }

        public static RemovalMode Explicit { get; }
        public static RemovalMode Unsubscribe { get; }
        public static RemovalMode Complete { get; }
        public static RemovalMode Error(Exception exception)
        {
            return new RemovalMode(new ErrorRemoval(exception));
        }
    }

    public class ExplicitRemoval
    {
        public static readonly ExplicitRemoval Default = new ExplicitRemoval();

        private ExplicitRemoval()
        {

        }

        public override string ToString() => "Explicit";
    }

    public class UnsubscribeRemoval
    {
        public static readonly UnsubscribeRemoval Default = new UnsubscribeRemoval();

        private UnsubscribeRemoval()
        {

        }

        public override string ToString() => "Unsubscribe";
    }

    public class CompleteRemoval
    {
        public static readonly CompleteRemoval Default = new CompleteRemoval();

        private CompleteRemoval()
        {

        }

        public override string ToString() => "Complete";
    }

    public class ErrorRemoval
    {
        public ErrorRemoval(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public override string ToString() => $"Error: {Exception}";
    }
}
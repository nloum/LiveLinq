using SimpleMonads;
using System;

namespace LiveLinq
{
    public class ReasonForRemoval : Either<ExplicitRemoval, UnsubscribeRemoval, CompleteRemoval, ErrorRemoval>
    {
        private ReasonForRemoval(ExplicitRemoval item1) : base(item1)
        {
        }

        private ReasonForRemoval(UnsubscribeRemoval item2) : base(item2)
        {
        }

        private ReasonForRemoval(CompleteRemoval item3) : base(item3)
        {
        }

        private ReasonForRemoval(ErrorRemoval item4) : base(item4)
        {
        }

        public static ReasonForRemoval Explicit { get; }
        public static ReasonForRemoval Unsubscribe { get; }
        public static ReasonForRemoval Complete { get; }
        public static ReasonForRemoval Error(Exception exception)
        {
            return new ReasonForRemoval(new ErrorRemoval(exception));
        }
    }

    public class ExplicitRemoval
    {
        public static readonly ExplicitRemoval Default = new ExplicitRemoval();

        private ExplicitRemoval()
        {

        }
    }

    public class UnsubscribeRemoval
    {
        public static readonly UnsubscribeRemoval Default = new UnsubscribeRemoval();

        private UnsubscribeRemoval()
        {

        }
    }

    public class CompleteRemoval
    {
        public static readonly CompleteRemoval Default = new CompleteRemoval();

        private CompleteRemoval()
        {

        }
    }

    public class ErrorRemoval
    {
        public ErrorRemoval(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
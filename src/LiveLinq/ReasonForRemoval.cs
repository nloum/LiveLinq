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

        public static ReasonForRemoval Explicit => new ReasonForRemoval(ExplicitRemoval.Default);
        public static ReasonForRemoval Unsubscribe => new ReasonForRemoval(UnsubscribeRemoval.Default);
        public static ReasonForRemoval Complete => new ReasonForRemoval(CompleteRemoval.Default);
        public static ReasonForRemoval Error(Exception exception)
        {
            return new ReasonForRemoval(new ErrorRemoval(exception));
        }
                
        public bool IsExplicit => Item1 != null;
        public bool IsUnsubscribe => Item2 != null;
        public bool IsComplete => Item3 != null;
        public bool IsError => Item4 != null;

        public override string ToString()
        {
            if (Item1 != null)
            {
                return Item1.ToString();
            }
            if (Item2 != null)
            {
                return Item2.ToString();
            }
            if (Item3 != null)
            {
                return Item3.ToString();
            }

            return Item4.ToString();
        }
        
        public override bool Equals(object other)
        {
            return object.ReferenceEquals(this, other);
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

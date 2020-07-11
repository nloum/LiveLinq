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
        
        public IMaybe<ErrorRemoval> Error => Item4;
        
        public bool IsExplicit => Item1.HasValue;
        public bool IsUnsubscribe => Item2.HasValue;
        public bool IsComplete => Item3.HasValue;
        public bool IsError => Item4.HasValue;

        public override string ToString()
        {
            if (Item1.HasValue)
            {
                return Item1.Value.ToString();
            }
            if (Item2.HasValue)
            {
                return Item2.Value.ToString();
            }
            if (Item3.HasValue)
            {
                return Item3.Value.ToString();
            }

            return Item4.Value.ToString();
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

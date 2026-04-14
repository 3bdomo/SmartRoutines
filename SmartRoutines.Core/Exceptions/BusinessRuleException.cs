using System;

namespace SmartRoutines.Core.Exceptions
{
    public class BusinessRuleException : SmartRoutinesException
    {
        public BusinessRuleException(string message) : base(message)
        {
        }

        public BusinessRuleException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

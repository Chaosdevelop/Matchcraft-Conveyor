// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Diagnostics.CodeAnalysis;
#nullable enable

namespace NetExtender.Types.Exceptions
{
    public class ExceptionWrapper<T> : ExceptionWrapper where T : Exception
    {
        [return: NotNullIfNotNull("exception")]
        public static implicit operator ExceptionWrapper<T>?(T? exception)
        {
            return exception is not null ? new ExceptionWrapper<T>(exception) : null;
        }

        [return: NotNullIfNotNull("exception")]
        public static implicit operator T?(ExceptionWrapper<T>? exception)
        {
            return exception?.Exception;
        }
        
        public new T Exception
        {
            get
            {
                return (T) base.Exception;
            }
        }
        
        public ExceptionWrapper(T exception)
            : base(exception)
        {
        }
    }
    
    public class ExceptionWrapper
    {
        [return: NotNullIfNotNull("exception")]
        public static implicit operator ExceptionWrapper?(Exception? exception)
        {
            return exception is not null ? new ExceptionWrapper(exception) : null;
        }

        [return: NotNullIfNotNull("exception")]
        public static implicit operator Exception?(ExceptionWrapper? exception)
        {
            return exception?.Exception;
        }

        public virtual Exception Exception { get; } = null!;

        protected ExceptionWrapper()
        {
        }

        public ExceptionWrapper(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }
}
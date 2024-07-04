// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NetExtender.Utilities.Serialization;

#nullable enable

namespace NetExtender.Types.Exceptions
{
    [Serializable]
    public abstract class EnumUndefinedException<T> : EnumUndefinedException where T : unmanaged, Enum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumUndefinedException<T> Create(T value)
        {
            return new EnumUndefined(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumUndefinedException<T> Create(T value, String? paramName)
        {
            return new EnumUndefined(value, paramName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumUndefinedException<T> Create(T value, String? message, System.Exception? innerException)
        {
            return new EnumUndefined(value, message, innerException);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumUndefinedException<T> Create(T value, String? paramName, String? message)
        {
            return new EnumUndefined(value, paramName, message);
        }

        public static implicit operator T(EnumUndefinedException<T>? exception)
        {
            return exception?.Value ?? default;
        }
        
        public override Type Type
        {
            get
            {
                return typeof(T);
            }
        }
        
        public abstract T Value { get; }

        public sealed override Enum Enum
        {
            get
            {
                return Value;
            }
        }
        
        protected EnumUndefinedException()
        {
        }

        protected EnumUndefinedException(String? paramName)
            : base(paramName)
        {
        }

        protected EnumUndefinedException(String? message, System.Exception? innerException)
            : base(message, innerException)
        {
        }

        protected EnumUndefinedException(String? paramName, T value, String? message)
            : base(paramName, value, message)
        {
        }

        protected EnumUndefinedException(String? paramName, String? message)
            : base(paramName, message)
        {
        }

        protected EnumUndefinedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [Serializable]
        private sealed class EnumUndefined : EnumUndefinedException<T>
        {
            public override T Value { get; }
            
            public EnumUndefined(T value)
                : this(value, null, (String?) null)
            {
                Value = value;
            }

            public EnumUndefined(T value, String? paramName)
                : this(value, paramName, (String?) null)
            {
            }

            public EnumUndefined(T value, String? message, System.Exception? innerException)
                : base(message ?? $"Specified value '{value}' was out of the range of valid values of enum type '{typeof(T)}'.", innerException)
            {
                Value = value;
            }

            public EnumUndefined(T value, String? paramName, String? message)
                : base(paramName, value, message ?? $"Specified value '{value}' was out of the range of valid values of enum type '{typeof(T)}'.")
            {
                Value = value;
            }

            private EnumUndefined(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                Value = info.GetValue<T>(nameof(Value));
            }

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Value), Value);
            }
        }
    }

    [Serializable]
    public abstract class EnumUndefinedException : ArgumentOutOfRangeException
    {
        [return: NotNullIfNotNull("exception")]
        public static implicit operator Enum?(EnumUndefinedException? exception)
        {
            return exception?.Enum;
        }
        
        public virtual Type Type
        {
            get
            {
                return Enum.GetType();
            }
        }
        
        public abstract Enum Enum { get; }
        
        protected EnumUndefinedException()
        {
        }

        protected EnumUndefinedException(String? paramName)
            : base(paramName)
        {
        }

        protected EnumUndefinedException(String? message, Exception? innerException)
            : base(message ?? "Specified value was out of the range of valid values of enum.", innerException)
        {
        }

        protected EnumUndefinedException(String? paramName, Enum? value, String? message)
            : base(paramName, value, message is null && value is not null ? $"Specified value '{value}' was out of the range of valid values of enum type '{value.GetType()}'." : message)
        {
        }

        protected EnumUndefinedException(String? paramName, String? message)
            : base(paramName, message ?? "Specified value was out of the range of valid values of enum.")
        {
        }

        protected EnumUndefinedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
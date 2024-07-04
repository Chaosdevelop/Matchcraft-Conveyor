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
    public abstract class EnumNotSupportedException<T> : EnumNotSupportedException where T : unmanaged, Enum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumNotSupportedException<T> Create(T value)
        {
            return new EnumNotSupported(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumNotSupportedException<T> Create(T value, String? message)
        {
            return new EnumNotSupported(value, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EnumNotSupportedException<T> Create(T value, String? message, System.Exception? innerException)
        {
            return new EnumNotSupported(value, message, innerException);
        }
        
        public static implicit operator T(EnumNotSupportedException<T>? exception)
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

        protected EnumNotSupportedException()
        {
        }

        protected EnumNotSupportedException(String? message)
            : base(message)
        {
        }

        protected EnumNotSupportedException(String? message, System.Exception? innerException)
            : base(message, innerException)
        {
        }

        protected EnumNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [Serializable]
        private sealed class EnumNotSupported : EnumNotSupportedException<T>
        {
            public override T Value { get; }

            public EnumNotSupported(T value)
                : this(value, null)
            {
            }

            public EnumNotSupported(T value, String? message)
                : base(message ?? $"Specified value '{value}' of enum type '{typeof(T)}' is not supported.")
            {
                Value = value;
            }

            public EnumNotSupported(T value, String? message, System.Exception? innerException)
                : base(message ?? $"Specified value '{value}' of enum type '{typeof(T)}' is not supported.", innerException)
            {
                Value = value;
            }

            private EnumNotSupported(SerializationInfo info, StreamingContext context)
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
    public abstract class EnumNotSupportedException : NotSupportedException
    {
        [return: NotNullIfNotNull("exception")]
        public static implicit operator Enum?(EnumNotSupportedException? exception)
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
        
        protected EnumNotSupportedException()
        {
        }

        protected EnumNotSupportedException(String? message)
            : base(message)
        {
        }

        protected EnumNotSupportedException(String? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected EnumNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
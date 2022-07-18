using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using RedSharp.Reactive.Bindings.Entities;
using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Reactive.Bindings.Structures;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Helpers
{
    public delegate BindingChain<TInput, TOutput> ChainCreatorCallback<TInput, TOutput>(BindingChain<TInput, TInput> input);

    public static class Expressions
    {
        public static BindingChain<TValue, TValue> Start<TValue>()
        {
            var resultNode = new InputValueBindingNode<TValue>();

            return BindingChain.Begin(resultNode);
        }

        public static BindingChain<TValue, TValue> Bind<TValue>(TValue value)
        {
            var resultNode = new InputValueBindingNode<TValue>();

            resultNode.Value = value;

            return BindingChain.Begin(resultNode);
        }

        public static BindingChain<TMain, TOutput> Select<TMain, TOutput>(this TMain item, Func<TMain, TOutput> getter, string name) where TMain : INotifyPropertyChanged
        {
            ArgumentsGuard.ThrowIfNull(item);
            ArgumentsGuard.ThrowIfNull(getter);
            ArgumentsGuard.ThrowIfNullOrEmpty(name);

            var startNode = new InputValueBindingNode<TMain>();
            var resultNode = new NotifyPropertyChangedBindingNode<TMain, TOutput>(name, getter);

            startNode.Value = item;
            startNode.Following = resultNode;

            return BindingChain.Begin(startNode).Append(resultNode);
        }

        public static BindingChain<TMain, TOutput> Select<TMain, TOutput>(this TMain item, Expression<Func<TMain, TOutput>> getter) where TMain : INotifyPropertyChanged
        {
            return Select(item, getter.Compile(), ((MemberExpression)getter.Body).Member.Name);
        }

        public static BindingChain<TMain, TOutput> Select<TMain, TInput, TOutput>
            (this BindingChain<TMain, TInput> builder, Func<TInput, TOutput> getter, Action<TInput, TOutput> setter, string name) where TInput : INotifyPropertyChanged
        {
            ArgumentsGuard.ThrowIfNull(getter);
            ArgumentsGuard.ThrowIfNullOrEmpty(name);

            var resultNode = new NotifyPropertyChangedBindingNode<TInput, TOutput>(name, getter, setter);

            return builder.Append(resultNode);
        }

        public static BindingChain<TMain, TOutput> Select<TMain, TInput, TOutput>(this BindingChain<TMain, TInput> builder, Func<TInput, TOutput> getter, string name) where TInput : INotifyPropertyChanged
        {
            return Select(builder, getter, null, name);
        }

        public static BindingChain<TMain, TOutput> Select<TMain, TInput, TOutput>(this BindingChain<TMain, TInput> builder, Expression<Func<TInput, TOutput>> getter) where TInput : INotifyPropertyChanged
        {            
            return Select(builder, getter.Compile(), ((MemberExpression)getter.Body).Member.Name);
        }

        public static BindingChain<TMain, TOutput> Convert<TMain, TInput, TOutput>(this BindingChain<TMain, TInput> builder, IBindingConverter<TInput, TOutput> converter)
        {
            ArgumentsGuard.ThrowIfNull(converter);

            var resultNode = new ConvertorBindingNode<TInput, TOutput>(converter);

            return builder.Append(resultNode);
        }

        public static BindingChain<TMain, TOutput> Convert<TMain, TInput, TOutput>(this BindingChain<TMain, TInput> builder, Func<TInput, TOutput> forward, Func<TOutput, TInput> backward = null)
        {
            var converter = new AnonymousDirectConverter<TInput, TOutput>(forward, backward);

            return Convert(builder, converter);
        }

        public static BindingChain<TMain, TOutput> Unite<TMain, TInput, TOutput>(this BindingChain<TMain, IList<TInput>> builder, IListConverter<TInput, TOutput> converter)
        {
            ArgumentsGuard.ThrowIfNull(converter);

            var resultNode = new ListConverterBindingNode<TInput, TOutput>(converter);

            return builder.Append(resultNode);
        }

        public static BindingChain<TMain, TOutput> Unite<TMain, TInput, TOutput>(this BindingChain<TMain, IList<TInput>> builder, Func<IList<TInput>, TOutput> forward, Action<IList<TInput>, TOutput> backward = null)
        {
            var converter = new AnonymousListConverter<TInput, TOutput>(forward, backward);

            return Unite(builder, converter);
        }

        public static BindingChain<TMain, TOutput> WithCallback<TMain, TOutput>(this BindingChain<TMain, TOutput> builder, Action<TOutput> callback)
        {
            ArgumentsGuard.ThrowIfNull(callback);

            var resultNode = new CallbackBindingNode<TOutput>(callback);

            return builder.Append(resultNode);
        }

        public static BindingChain<TMain, IList<TOutput>> ForEachStraight<TMain, TInput, TOutput, TCollection>
            (this BindingChain<TMain, TCollection> builder, ChainCreatorCallback<TInput, TOutput> chainCallback) where TCollection : IEnumerable<TInput>
        {
            ArgumentsGuard.ThrowIfNull(chainCallback);

            var inputNode = new InputValueBindingNode<TInput>();
            var startChain = BindingChain.Begin(inputNode);

            var completeChain = chainCallback.Invoke(startChain);

            var resultNode = new CollectionItemsBindingNode<TInput, TOutput>(completeChain);

            return builder.Append(resultNode);
        }

                                                                                                
        public static BindingChain<TMain, IList<TOutput>> ForEachNotify<TMain, TInput, TOutput, TCollection>
            (this BindingChain<TMain, TCollection> builder, ChainCreatorCallback<TInput, TOutput> chainCallback) where TCollection : IEnumerable<TInput>, INotifyCollectionChanged
        {
            ArgumentsGuard.ThrowIfNull(chainCallback);

            var inputNode = new InputValueBindingNode<TInput>();
            var startChain = BindingChain.Begin(inputNode);

            var completeChain = chainCallback.Invoke(startChain);

            var resultNode = new NotifyCollectionChangedBindingNode<TInput, TOutput, TCollection>(completeChain);

            return builder.Append(resultNode);
        }

        public static IBindingExpression<TInput, TOutput> Commit<TInput, TOutput>(this BindingChain<TInput, TOutput> builder)
        {
            var result = new BindingExpression<TInput, TOutput>(builder);

            result.Freeze();

            return result;
        }
    }
}

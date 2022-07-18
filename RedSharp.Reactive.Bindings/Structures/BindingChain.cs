using RedSharp.Reactive.Bindings.Interfaces;
using RedSharp.Sys.Helpers;

namespace RedSharp.Reactive.Bindings.Structures
{
    public static class BindingChain
    {
        public static BindingChain<TResult, TResult> Begin<TResult>(IBindingNode<TResult> item)
        {
            return new BindingChain<TResult, TResult>(item, item);
        }

        public static BindingChain<TInput, TResult> Append<TInput, TOutput, TResult>(this BindingChain<TInput, TOutput> chain, IBindingNode<TResult> item)
        {
            chain.Current.Following = item;

            return new BindingChain<TInput, TResult>(chain.Start, item);
        }
    }

    public struct BindingChain<TInput, TOutput>
    {
        public readonly IBindingNode<TInput> Start;

        public readonly IBindingNode<TOutput> Current;

        public BindingChain(IBindingNode<TInput> start, IBindingNode<TOutput> current)
        {
            ArgumentsGuard.ThrowIfNull(start);
            ArgumentsGuard.ThrowIfNull(current);

            Start = start;
            Current = current;
        }
    }
}

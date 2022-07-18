namespace RedSharp.Reactive.Bindings.Interfaces
{
    /// <summary>
    /// Simple values converter.
    /// </summary>
    /// <remarks>
    /// This is not a fundamental interface for the binding system 
    /// (you can make your own converter node that will use whatever you want), 
    /// but it is used by one of the standard nodes, so I put it here.
    /// </remarks>
    public interface IBindingConverter<TInput, TOutput>
    {
        /// <summary>
        /// Means that this converter can convert value back.
        /// </summary>
        /// <remarks>
        /// This is obviously that a converter has to be able 
        /// to convert at least in forward direction.
        /// </remarks>
        bool IsTwoSide { get; }

        /// <summary>
        /// Forward converting.
        /// </summary>
        /// <remarks>
        /// Has to be implemented.
        /// </remarks>
        TOutput Forward(TInput value);

        /// <summary>
        /// Backward converting.
        /// </summary>
        /// <remarks>
        /// Can be not implemented.
        /// <br/>Mark <see cref="IsTwoSide"/> as false in this case.
        /// </remarks>
        TInput Backward(TOutput value);
    }
}

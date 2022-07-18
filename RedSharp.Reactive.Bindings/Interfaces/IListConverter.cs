using System.Collections.Generic;

namespace RedSharp.Reactive.Bindings.Interfaces
{
    /// <summary>
    /// Advanced values converter.
    /// Has to convert each value of the input list into a single result.
    /// </summary>
    /// <remarks>
    /// This is not a fundamental interface for the binding system 
    /// (you can make your own converter node that will use whatever you want), 
    /// but it is used by one of the standard nodes, so I put it here.
    /// </remarks>
    public interface IListConverter<TInput, TOutput>
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
        /// Forward converting. Must make a single result 
        /// using all values from the input collection.
        /// </summary>
        /// <remarks>
        /// Has to be implemented.
        /// </remarks>
        TOutput Forward(IList<TInput> collection);

        /// <summary>
        /// Backward converting. Has to mutate collection values by given input value.
        /// </summary>
        /// <remarks>
        /// Can be not implemented.
        /// <br/>Mark <see cref="IsTwoSide"/> as false in this case.
        /// </remarks>
        void Backward(IList<TInput> collection, TOutput value);
    }
}

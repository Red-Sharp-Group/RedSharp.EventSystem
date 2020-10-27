using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// That is object event receiver, surprise.
    /// </summary>
    public interface IWriListener
    {
        /// <summary>
        /// Method event receiver.
        /// </summary>
        /// <typeparam name="TModel">
        /// Event argument type.
        /// </typeparam>
        /// <param name="model">
        /// Event argument model.
        /// In most cases must be not null. But it depends on author implementation.
        /// </typeparam></param>
        void Raise<TModel>(TModel model);
    }
}

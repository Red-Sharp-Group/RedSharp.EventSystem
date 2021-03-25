using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// This is an object main event receiver.
    /// </summary>
    public interface IWriListener<TModel> : IWriListenerGeneral
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
        void ReceiveEvent(TModel model);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// Simple event descriptor, so you can recognize an event even if you set it in a collection f.e.
    /// </summary>
    public interface IWriEventGeneral
    {
        /// <summary>
        /// Events name, strongly recommend to use nameof()
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Object that contain the event, 
        /// because this event is an object and can be got
        /// it would be nice to know who it hold.
        /// </summary>
        Object Owner { get; }
    }
}

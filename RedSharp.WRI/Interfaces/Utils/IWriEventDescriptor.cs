using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.WRI.Interfaces.Utils
{
    /// <summary>
    /// Simple event descriptor, so you can recognize it if you set the event in a collection f.e.
    /// </summary>
    public interface IWriEventDescriptor
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

        /// <summary>
        /// Type of argument.
        /// </summary>
        Type Type { get; }
    }
}

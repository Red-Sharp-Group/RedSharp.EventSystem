using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// Basic class for all "notifiable" collections, 
    /// that fix mistake from the system library, 
    /// where these two interfaces are existed separately.
    /// </summary>
    public interface IReactiveEnumerable<TItem> : IEnumerable<TItem>, INotifyCollectionChanged
    { }
}

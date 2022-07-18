using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.Reactive.Sys.Interfaces.Entities
{
    /// <summary>
    /// Base interface for all observable objects.
    /// <br/>This object unites all needed interfaces in one entity.
    /// </summary>
    public interface IReactiveEntity : INotifyPropertyChanging, INotifyPropertyChanged, INotifyDataErrorInfo
    { }
}

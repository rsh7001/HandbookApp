using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace HandbookApp.Utilities
{
    public static class RoutingStateExtension
    {
        public static IReactiveCommand<object> NavigateModal<T> (this RoutingState This)
            where T : IRoutableViewModel, new()
        {
            var ret = new ReactiveCommand<object>(This.Navigate.CanExecuteObservable, x => {
                Observable.Return(x));
            ret.Select(_ => (IRoutableViewModel)Locator.Current.GetService<T>() ?? new T()).InvokeCommand(This.Navigate);

            return ret;
        }
    }
}

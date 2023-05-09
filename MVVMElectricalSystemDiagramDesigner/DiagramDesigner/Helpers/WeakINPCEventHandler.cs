using System;
using System.ComponentModel;
using System.Reflection;

namespace DiagramDesigner.Helpers
{
    //[DebuggerNonUserCode]
    public sealed class WeakINPCEventHandler
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;

        public WeakINPCEventHandler(PropertyChangedEventHandler callback)
        {
            _method = callback.Method;
            _targetReference = new(callback.Target, true);
        }

        //[DebuggerNonUserCode]
        public void Handler(object sender, PropertyChangedEventArgs e)
        {
            var target = _targetReference.Target;
            if (target != null)
                ((Action<object, PropertyChangedEventArgs>)Delegate.CreateDelegate(typeof(Action<object, PropertyChangedEventArgs>), target, _method, true))?.Invoke(sender, e);
        }
    }
}

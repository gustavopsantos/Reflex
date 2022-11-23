using System;

namespace Reflex.Scripts.Events
{
    public class DisposableEvent
    {
        private event Action _event;

        public void Invoke()
        {
            _event?.Invoke();
        }

        public IDisposable Subscribe(Action action)
        {
            _event += action;
            return new EventSubscription(() => Unsubscribe(action));
        }

        private void Unsubscribe(Action action)
        {
            _event -= action;
        }
    }
}
using System;

namespace Reflex.Scripts.Events
{
    public class EventSubscription : IDisposable
    {
        private readonly Action _onDisposal;

        public EventSubscription(Action onDisposal)
        {
            _onDisposal = onDisposal;
        }
        
        public void Dispose()
        {
            _onDisposal.Invoke();
        }
    }
}
using System;
using FluentAssertions;

namespace Reflex.EditModeTests
{
    public class CallbackAssertion
    {
        private int _calls;

        public void Invoke()
        {
            _calls++;
        }

        public static implicit operator Action(CallbackAssertion callbackAssertion)
        {
            return callbackAssertion.Invoke;
        }

        public void ShouldHaveBeenCalledOnce()
        {
            _calls.Should().Be(1);
        }

        public void ShouldNotHaveBeenCalled()
        {
            _calls.Should().Be(0);
        }

        public void ShouldHaveBeenCalled(int times)
        {
            _calls.Should().Be(times);
        }
    }
}
    using System;
    using FluentAssertions;

    public class CallbackAssertion
    {
        private int _calls;
        private readonly Action _call;

        public CallbackAssertion()
        {
            _call = () => _calls++;
        }

        public static implicit operator Action(CallbackAssertion callbackAssertion)
        {
            return callbackAssertion._call;
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
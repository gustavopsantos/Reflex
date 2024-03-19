using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Reflex.Tests
{
    public class LinkerTests
    {
        [Test]
        public void LinkerFileExist()
        {
            var linker = Resources.Load<TextAsset>("link");
            linker.Should().NotBeNull();
        }
    }
}
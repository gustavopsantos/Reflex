using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Reflex.Microsoft.Tests
{
    public class LinkerTests
    {
        [Test]
        public void LinkerFileExist()
        {
			TextAsset linker = Resources.Load<TextAsset>("link");
            linker.Should().NotBeNull();
        }
    }
}
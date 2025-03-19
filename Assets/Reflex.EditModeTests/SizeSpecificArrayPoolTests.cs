using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Pooling;

namespace Reflex.EditModeTests
{
    internal class SizeSpecificArrayPoolTests
    {
        [Test]
        public void Constructor_InitializesBucketsCorrectly()
        {
            var maxLength = 8;
            var pool = new SizeSpecificArrayPool<int>(maxLength);

            for (var i = 0; i <= maxLength; i++)
            {
                pool.Rent(i).Should().NotBeNull();
            }
        }

        [Test]
        public void Rent_ReturnsArrayWithCorrectLength()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var array = pool.Rent(5);
            array.Should().NotBeNull();
            array.Length.Should().Be(5);
        }

        [Test]
        public void Rent_IncrementsRentalIndex()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var firstArray = pool.Rent(3);
            var secondArray = pool.Rent(3);

            var distinctReferences = new HashSet<int[]>();
            distinctReferences.Add(firstArray);
            distinctReferences.Add(secondArray);
            distinctReferences.Count.Should().Be(2);
        }

        [Test]
        public void Return_ClearsArrayOfValueTypes()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var array = pool.Rent(4);
            array[0] = 42;
            array[2] = 42;

            pool.Return(array);
            array.All(n => n == 0).Should().Be(true);
        }

        [Test]
        public void Return_ClearsArrayOfReferenceTypes()
        {
            var pool = new SizeSpecificArrayPool<object>(8);
            var array = pool.Rent(4);
            array[0] = new object();
            array[2] = new object();

            pool.Return(array);
            array.All(n => n == null).Should().Be(true);
        }

        [Test]
        public void Rent_AfterReturn_ReusesArray()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            var array = pool.Rent(6);
            pool.Return(array);
            var reusedArray = pool.Rent(6);

            var distinctReferences = new HashSet<int[]>();
            distinctReferences.Add(array);
            distinctReferences.Add(reusedArray);
            distinctReferences.Count.Should().Be(1);
        }

        [Test]
        public void Rent_ExpandBucket()
        {
            var pool = new SizeSpecificArrayPool<int>(8);
            Assert.DoesNotThrow(() =>
            {
                for (var i = 0; i < SizeSpecificArrayPool<int>.InitialBucketSize * 2; i++)
                {
                    pool.Rent(1);
                }
            });
        }

        [Test]
        public void Rent_OverMaxLength_DoesNotThrow()
        {
            var pool = new SizeSpecificArrayPool<object>(8);
            Assert.DoesNotThrow(() => { pool.Rent(12); });
        }

        [Test]
        public void Rent_OverMaxLength_DoesReturnUnpooledArray()
        {
            var pool = new SizeSpecificArrayPool<object>(8);
            var array = pool.Rent(12);
            array.Length.Should().Be(12);
            pool.Return(array);
            pool.Rent(12).Should().NotBeSameAs(array);
        }
    }
}
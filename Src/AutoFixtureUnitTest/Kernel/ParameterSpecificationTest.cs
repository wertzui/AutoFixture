﻿using System;
using System.Linq;
using System.Reflection;
using AutoFixture.Kernel;
using TestTypeFoundation;
using Xunit;

namespace AutoFixtureUnitTest.Kernel
{
    public class ParameterSpecificationTest
    {
        [Fact]
        public void SutIsRequestSpecification()
        {
            // Fixture setup
            // Exercise system
            var sut = new ParameterSpecification(typeof(object), "someName");
            // Verify outcome
            Assert.IsAssignableFrom<IRequestSpecification>(sut);
            // Teardown
        }

        [Fact]
        [Obsolete]
        public void InitializeWithParameterNameShouldSetCorrespondingProperty()
        {
            // Fixture setup
            var type = typeof(object);
            var name = "someName";
            // Exercise system
            var sut = new ParameterSpecification(type, name);
            // Verify outcome
#pragma warning disable 618
            Assert.Equal(type, sut.TargetType);
            Assert.Equal(name, sut.TargetName);
#pragma warning restore 618
            // Teardown
        }

        [Fact]
        public void InitializeWithNullParameterTypeShouldThrowArgumentNullException()
        {
            // Fixture setup
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                new ParameterSpecification(null, "someName"));
            // Teardown
        }

        [Fact]
        public void InitializeWithNullParameterNameShouldThrowArgumentNullException()
        {
            // Fixture setup
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                new ParameterSpecification(typeof(object), null));
            // Teardown
        }

        [Fact]
        public void InitializeWithNullTargetThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ParameterSpecification(null));
        }

        [Fact]
        public void IsSatisfiedByWithNullRequestShouldThrowArgumentNullException()
        {
            // Fixture setup
            var sut = new ParameterSpecification(typeof(object), "someName");
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                sut.IsSatisfiedBy(null));
            // Teardown
        }

        [Theory]
        [InlineData("parameter", "parameter", true)]
        [InlineData("someName", "parameter", false)]
        [InlineData("Parameter", "parameter", false)]
        public void IsSatisfiedByWithRequestForParameterShouldReturnCorrectResult(
            string parameterName,
            string requestedName,
            bool expectedResult)
        {
            // Fixture setup
            var sut = new ParameterSpecification(typeof(object), parameterName);
            var request = typeof(SingleParameterType<object>)
                          .GetConstructor(new[] { typeof(object) })
                          .GetParameters()
                          .Single(p => p.Name == requestedName);
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Fact]
        public void IsSatisfiedByWithRequestForParameterWithSameNameAndIncompatibleTypeShouldReturnFalse()
        {
            // Fixture setup
            var parameterName = "parameter";
            var requestedName = "parameter";
            var sut = new ParameterSpecification(typeof(object), parameterName);
            var request = typeof(SingleParameterType<int>)
                          .GetConstructor(new[] { typeof(int) })
                          .GetParameters()
                          .Single(p => p.Name == requestedName);
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.False(result);
            // Teardown
        }

        [Fact]
        public void IsSatisfiedByWithRequestForMemberOtherThanParameterWithSameNameShouldReturnFalse()
        {
            // Fixture setup
            var parameterName = "Parameter";
            var requestedName = "Parameter";
            var sut = new ParameterSpecification(typeof(object), parameterName);
            var request = typeof(SingleParameterType<object>).GetProperty(requestedName);
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.False(result);
            // Teardown
        }

        [Theory]
        [InlineData("aString")]
        [InlineData(9)]
        [InlineData(true)]
        [InlineData(typeof(object))]
        public void IsSatisfiedByWithInvalidRequestShouldReturnFalse(object request)
        {
            // Fixture setup
            var sut = new ParameterSpecification(typeof(object), "someName");
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.False(result);
            // Teardown
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsSatisfiedByReturnsCorrectResultAccordingToTarget(
            bool expected)
        {
            var parameter =
                typeof(string).GetMethod("Contains").GetParameters().First();
            var target = new DelegatingCriterion<ParameterInfo>
            {
                OnEquals = f =>
                {
                    Assert.Equal(parameter, f);
                    return expected;
                }
            };
            var sut = new ParameterSpecification(target);

            var actual = sut.IsSatisfiedBy(parameter);

            Assert.Equal(expected, actual);
        }
    }
}

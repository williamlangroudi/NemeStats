﻿using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

namespace NemeStats.TestingHelpers.NemeStatsTestingExtensions.NemeStatsTestingExtensionsTests
{
    [TestFixture]
    public class AssertFirstCallIsTypeTests
    {
        private IList<object[]> _arguments;

        [Test]
        public void It_Fails_If_Arguments_Is_Null()
        {
            //--act
            _arguments.AssertFirstCallIsType<object>(0);
        }

        [Test]
        public void It_Fails_If_More_Than_One_Call_Was_Made()
        {
            //--assert
            _arguments = new List<object[]> {new object[0], new object[0]};

            //--act
            _arguments.AssertFirstCallIsType<object>(0);
        }

        [Test]
        public void It_Fails_If_First_Call_Is_Not_Of_Specified_Type()
        {
            //--assert
            _arguments = new List<object[]> { new object[0] };

            //--act
            _arguments.AssertFirstCallIsType<string>(0);
        }

        [Test]
        public void It_Returns_The_Parameter_At_The_Specified_Index()
        {
            string firstParameter = "first parameter";
            string secondParameter = "second parameter";
            object[] argumentsForFirstCall = { firstParameter, secondParameter };
            _arguments = new List<object[]> { argumentsForFirstCall };

            //--act
            var result = _arguments.AssertFirstCallIsType<object>(1);

            //--assert
            result.ShouldBeSameAs(secondParameter);
        }
    }
}

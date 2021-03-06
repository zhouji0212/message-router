﻿using System;
using MessageRouter.Helpers;
using NUnit.Framework;

namespace NetmqRouter.Tests.Helpers
{
    [TestFixture]
    public class TypeComparerTests
    {
        #region test classes

        class ClassA
        {

        }

        class ClassB : ClassA
        {

        }

        #endregion

        [TestCase(typeof(int), typeof(object), ExpectedResult = 1)]
        [TestCase(typeof(string), typeof(object), ExpectedResult = 1)]
        [TestCase(typeof(int), typeof(string), ExpectedResult = 0)]
        [TestCase(typeof(ClassA), typeof(ClassB), ExpectedResult = -1)]
        [TestCase(typeof(ClassB), typeof(ClassA), ExpectedResult = 1)]
        [TestCase(typeof(void), typeof(ClassA), ExpectedResult = 0)]
        [TestCase(typeof(ClassA), typeof(void), ExpectedResult = 0)]
        public int Compare(Type typeA, Type typeB)
        {
            return new TypeComparer().Compare(typeA, typeB);
        }
    }
}
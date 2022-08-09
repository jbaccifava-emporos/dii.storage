﻿using System;

namespace dii.storage.cosmos.tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityOrderAttribute : Attribute
    {
        public TestPriorityOrderAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
    }
}
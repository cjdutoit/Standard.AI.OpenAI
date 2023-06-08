// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xunit;
using Xunit.Sdk;

namespace Standard.AI.OpenAI.Tests.Integration
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer(
        typeName: "Standard.AI.OpenAI.Tests.Integration.ReleaseCandidateTestCaseDiscoverer",
        assemblyName: "Standard.AI.OpenAI.Tests.Integration")]
    public class ReleaseCandidateFactAttribute : FactAttribute { }
}

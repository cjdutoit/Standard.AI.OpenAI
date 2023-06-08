// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Standard.AI.OpenAI.Tests.Integration
{
    public class ReleaseCandidateTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink diagnosticMessageSink;
        private readonly IConfiguration configuration;

        public ReleaseCandidateTestCaseDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("STANDARD_AI_OPENAI_");

            configuration = configurationBuilder.Build();
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var isReleaseCandidate = configuration.GetValue<bool>("IS_RELEASE_CANDIDATE");

            if (isReleaseCandidate)
            {
                yield return new XunitTestCase(
                    diagnosticMessageSink,
                    defaultMethodDisplay: discoveryOptions.MethodDisplayOrDefault(),
                    defaultMethodDisplayOptions: discoveryOptions.MethodDisplayOptionsOrDefault(),
                    testMethod);
            }
        }
    }
}

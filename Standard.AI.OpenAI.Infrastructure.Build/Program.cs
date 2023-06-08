// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV1s;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV3s;

namespace Standard.AI.OpenAI.Infrastructure.Build
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var adoNetClient = new ADotNetClient();

            var githubPipeline = new GithubPipeline
            {
                Name = "Standard.AI.OpenAI Build",

                OnEvents = new Events
                {
                    Push = new PushEvent
                    {
                        Branches = new string[] { "main" }
                    },

                    PullRequest = new PullRequestEvent
                    {
                        Branches = new string[] { "main" }
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            EnvironmentVariables = new Dictionary<string, string>
                            {
                                { "ApiKey", "${{ secrets.APIKEY }}" },
                                { "OrgId", "${{ secrets.ORGID }}" },
                                { "STANDARD_AI_OPENAI_IS_RELEASE_CANDIDATE", EnvironmentVariables.IsGitHubReleaseCandidate() }
                            },

                            RunsOn = BuildMachines.WindowsLatest,

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV3
                                {
                                    Name = "Pulling Code"
                                },

                                new SetupDotNetTaskV3
                                {
                                    Name = "Installing .NET",

                                    With = new TargetDotNetVersionV3
                                    {
                                        DotNetVersion = "7.0.201"
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restoring Packages"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Building Solution"
                                },

                                new TestTask
                                {
                                    Name = "Running Tests",
                                },
                            }
                        }
                    },
                    {
                        "add_tag",
                        new TagJob(
                            runsOn: BuildMachines.WindowsLatest, 
                            dependsOn: "build", 
                            projectRelativePath: "Standard.AI.OpenAI\\Standard.AI.OpenAI.csproj",
                            githubToken: "${{ secrets.PAT_FOR_TAGGING }}",
                            versionEnvironmentVariableName: "version_number",
                            packageReleaseNotesEnvironmentVariable: "package_release_notes",
                            branchName: "main")
                    },
                    {
                        "publish",
                        new PublishJob(
                            runsOn: BuildMachines.WindowsLatest,
                            dependsOn: "add_tag",
                            nugetApiKey: "${{ secrets.NUGET_API_KEY }}")
                    }
                }
            };

            string buildScriptPath = "../../../../.github/workflows/dotnet.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adoNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }
    }
}
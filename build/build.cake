var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var basePath = "../";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectory(basePath + $"./src/TinyPubSub/bin/{configuration}");
    CleanDirectory(basePath + $"./src/TinyPubSub.Tests/bin/{configuration}");
    CleanDirectory(basePath + $"./src/TinyPubSub.Forms/bin/{configuration}");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild(basePath + "./src/TinyPubSub", new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest(basePath + "./src/TinyPubSub.Tests", new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    });
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
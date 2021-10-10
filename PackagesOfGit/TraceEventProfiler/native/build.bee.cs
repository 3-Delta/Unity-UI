using System.Collections.Generic;
using Bee.Core;
using Bee.Toolchain.Android;
using Bee.Toolchain.IOS;
using Bee.Toolchain.Windows;
using NiceIO;
using System;
using System.Linq;
using Bee.Toolchain.VisualStudio;
using Bee.Toolchain.GNU;
using Bee.NativeProgramSupport;
using Bee.Tools;
using Bee.Toolchain.MacOS;

class BuildProgram
{
    static string pluginDir = "../com.unity.traceeventprofiler/Plugins";

    static string DllNameForPlatform(Platform platform)
    {
        if (platform.Name == "Android")
            return "libtraceeventprofiler";
        return "traceeventprofiler";
    }

    static void Main()
    {
        List<BuildCommand> commands = HostPlatform.IsOSX ? MacCommands() : WindowsCommands();
        commands.AddRange(GetAndroidBuildCommands());

        NativeProgram plugin = new NativeProgram("traceeventprofiler")
        {
            Sources = { "src" }
        };
        plugin.CompilerSettingsForIosOrTvos().Add(s => s.WithEmbedBitcode(true));
        plugin.DynamicLinkerSettingsForWindows().Add(s => s.WithDefinitionFile("src/ProfilerPlugin.def"));

        plugin.OutputName.Set(c => DllNameForPlatform(c.ToolChain.Platform));

        foreach (var command in commands)
        {
            var toolchain = command.ToolChain;

            var config = new NativeProgramConfiguration(CodeGen.Release, toolchain, false);
            var tempDir = $"artifacts_preprocess/{toolchain.LegacyPlatformIdentifier}";
            var builtProgram = plugin.SetupSpecificConfiguration(config, toolchain.DynamicLibraryFormat).DeployTo(tempDir).Path;
            if (command.PostProcess != null)
                builtProgram = command.PostProcess(builtProgram, toolchain, command.PluginSubFolder);
            Backend.Current.AddAliasDependency(command.Alias, builtProgram);
        }
    }

    class BuildCommand
    {
        public ToolChain ToolChain;
        public string Alias;
        public string PluginSubFolder;
        public Func<NPath, ToolChain, string, NPath> PostProcess = PostProcessDefault;
        public static BuildCommand Create (ToolChain chain, string alias, string pluginSubFolder = "")
        {
            return new BuildCommand() { Alias = alias, ToolChain = chain, PluginSubFolder = pluginSubFolder };
        }
    }

    private static List<BuildCommand> GetAndroidBuildCommands()
    {
        List<BuildCommand> cmds = new List<BuildCommand>();
        if (AndroidNdk.LocatorArmv7.UserDefault == null)
        {
            Console.WriteLine("No Android SDK found");
            return cmds;
        }
        cmds.Add(BuildCommand.Create(new AndroidNdkToolchain(AndroidNdk.LocatorArmv7.UserDefault), "android", "Android/armeabi-v7a"));
        cmds.Add(BuildCommand.Create(new AndroidNdkToolchain(AndroidNdk.LocatorArm64.UserDefault), "android", "Android/armeabi"));
        cmds.Add(BuildCommand.Create(new AndroidNdkToolchain(AndroidNdk.Locatorx86.UserDefault), "android", "Android/x86"));
        return cmds;
    }

    private static List<BuildCommand> WindowsCommands()
    {
        var archs = new List<IntelArchitecture>() { new x64Architecture(), new x86Architecture() };
        return archs.ConvertAll(i => BuildCommand.Create(new WindowsToolchain(WindowsSdk.LocatorFor(i).UserDefaultOrDummy), "windows"));
    }

    private static List<BuildCommand> MacCommands()
    {
        return new List<BuildCommand>()
        {
            new BuildCommand()
            {
                ToolChain = new MacToolchain(MacSdk.Locatorx64.UserDefaultOrDummy),
                PostProcess = (p, t, s) => ChangeExtension(".bundle", p, t, string.Empty),
                Alias = "osx"
            },
            new BuildCommand()
            {
                ToolChain = new IOSToolchain(IOSSdk.LocatorArm64.UserDefaultOrDummy),
                PostProcess = (p, t, s) => ChangeExtension(".bundle", p, t, string.Empty),
                Alias = "ios"
            },
            // new BuildCommand()
            // {
            //     ToolChain = new TvOSToolchain(),
            //     PostProcess = (p, t) => ChangeExtension(".so", p, t)
            // }, 
        };
    }

    private static NPath PostProcessDefault(NPath builtProgram, ToolChain toolchain, string subFolderDir)
    {
        return Copy(builtProgram, builtProgram, toolchain, subFolderDir);
    }

    private static NPath ChangeExtension(string extension, NPath builtProgram, ToolChain toolchain, string subFolderDir)
    {
        return Copy(builtProgram, builtProgram.ChangeExtension(extension), toolchain, subFolderDir);
    }

    private static NPath Copy(NPath from, NPath to, ToolChain toolchain, string subFolderDir)
    {
        if (subFolderDir != string.Empty)
            to = new NPath($"{pluginDir}/{subFolderDir}").Combine(to.FileName);
        else
            to = new NPath($"{pluginDir}/{toolchain.LegacyPlatformIdentifier}").Combine(to.FileName);
        CopyTool.Instance().Setup(to, from);
        return to;
    }

}

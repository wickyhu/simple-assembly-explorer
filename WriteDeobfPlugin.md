# Write a deobfuscator plugin for SAE #

**Steps:**

1. Create a Class Library project, e.g. `SAE.MyDeobfPlugin`

It's recommended to keep assembly file name and name space same.

2. Add a reference to `SimpleAssemblyExplorer.Plugin.dll`.

This assembly can be found in SAE archive file.

3. Add a new class and inherit DefaultDeobfPlugin

SAE will firstly search all exported types for plugin, if error occured, will secondly try to use assembly file name + DeobfPlugin to search plugin (i.e. `SAE.MyDeobfPlugin.DeobfPlugin`).

And, you need to have at least a constructor with a IHost argument.

4. Override PluginInfo property and the methods you wish to handle

5. Build and put the assembly to SAE's Plugins directory

6. Open SAE to try the plugin

**Samples:**

You can get below SAE deobfuscator plugins source code from repository for reference:
  * `SAE.DeobfPluginSample`


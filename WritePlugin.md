# Write a main plugin for SAE #

**Steps:**


1. Create a Class Library project, e.g. `SAE.MyPlugin`

It's recommended to keep assembly file name and name space same.

2. Add a reference to `SimpleAssemblyExplorer.Plugin.dll`.

This assembly can be found in SAE archive file.

3. Add a new class and inherit DefaultMainPlugin

SAE will firstly search all exported types for plugin, if error occured, will secondly try to use assembly file name + Plugin to search plugin (e.g. `SAE.MyPlugin.Plugin`).

And, you need to have at least a constructor with a IHost argument.

4. Override PluginInfo property and Run method

5. Build and put the assembly to SAE's Plugins directory

6. Open SAE to try the plugin

**Samples:**

You can get some SAE plugins source code from repository for reference:
  * `SAE.EditFile`
  * `SAE.FileDisassembler`
  * `SAE.ILMerge`
  * `SAE.MethodSearcher`
  * `SAE.PluginSample`
  * `SAE.Reflector`
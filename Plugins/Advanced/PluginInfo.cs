// ============================================================================
// ARCHIVO 4: Plugins/Advanced/PluginInfo.cs
// ============================================================================
using System.Collections.Generic;

namespace DynamicUI.Plugins.Advanced
{
    public class PluginInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public int ControlCount { get; set; }
        public int SetterCount { get; set; }
        public int ValidatorCount { get; set; }
        public int ConverterCount { get; set; }
        public List<string> Controls { get; set; }
    }
}
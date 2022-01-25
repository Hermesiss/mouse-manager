using CommandLine;

namespace MouseManager {
    public class CommandLineOptions {
        [Option(shortName: 'w', longName: "width", Required = true, HelpText = "Target zone width")]
        public uint Width { get; set; }
        [Option(shortName: 'h', longName: "height", Required = true, HelpText = "Target zone height")]
        public uint Height { get; set; }
        
        [Option(shortName: 't', longName: "top", Required = false, HelpText = "Distance from top", Default = 0U)]
        public uint Top { get; set; }
        [Option(shortName: 'l', longName: "left", Required = false, HelpText = "Distance from left", Default = 0U)]
        public uint Left { get; set; }
        
        [Option(longName: "mv", Required = false, HelpText = "Distance from left", Default = false)]
        public bool MirrorVertical { get; set; }
        
        [Option(longName: "mh", Required = false, HelpText = "Distance from left", Default = false)]
        public bool MirrorHorizontal { get; set; }
    }
}
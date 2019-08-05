using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaldDuckPatcher {
    class Program {

        public static List<Patch> Patches = new List<Patch>()
        {
            new Patch() {
                Name = "Compatibility",
                Offset = 0x48205,
                OriginalBytes = new byte[] { 0x00, 0x10, 0x00, 0x00, 0x50, 0xE8, 0x11, 0xFF, 0xFF, 0xFF, 0x83, 0xC4, 0x0C },
                PatchedBytes  = new byte[] { 0x6E, 0x5F, 0x5C, 0x00, 0xFF, 0x15, 0xC0, 0x54, 0x5C, 0x00, 0x83, 0xC4, 0x04 }
            },

            new Patch() {
                Name = "Remove synchro for 60fps",
                Offset = 0xA13D7,
                OriginalBytes = new byte[] { 0x57, 0x69, 0x74, 0x68, 0x53, 0x79, 0x6E, 0x63, 0x72, 0x6F },
                PatchedBytes  = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Donald Duck Quack Attack / Goin' Quackers PC Patcher for Modern PC's");
            if (args.Length==0) {

                string defaultLocation = Path.Combine(Environment.CurrentDirectory, "Donald.exe");

                if (File.Exists(defaultLocation)) {
                    args = new string[] { defaultLocation };
                } else {

                    Console.WriteLine("Please drag your Donald.exe onto this batch file to patch the executable (or run from the command line with the location to Donald.exe as parameter)");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }

            /*byte[] expectedBytes = new byte[]
            {
                        0x00,0x10,0x00,0x00,0x50,0xE8,0x11,0xFF,0xFF,0xFF,0x83,0xC4,0x0C
            };
            byte[] patchedBytes = new byte[]
            {
                        0x6E,0x5F,0x5C,0x00,0xFF,0x15,0xC0,0x54,0x5C,0x00,0x83,0xC4,0x04
            };
            int offset = 0x48205;*/

            try {

                // Perform first checks
                using (FileStream file = File.Open(args[0], FileMode.Open)) {


                    foreach (Patch patch in Patches)
                    {
                        file.Seek(patch.Offset, SeekOrigin.Begin);

                        byte[] readBytes = new byte[patch.OriginalBytes.Length];
                        file.Read(readBytes, 0, patch.OriginalBytes.Length);
                        if (!readBytes.SequenceEqual(patch.OriginalBytes))
                        {
                            if (readBytes.SequenceEqual(patch.PatchedBytes))
                            {
                                Console.WriteLine("Error, EXE is already patched! (detected patch '"+patch.Name+"')");
                            }
                            else
                            {
                                Console.WriteLine("Error, unexpected byte sequence at "+patch.Offset+" (patch "+patch.Name+")");
                            }
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            return;
                        }
                    }

                    Console.WriteLine("Patching Donald.exe (and creating Donald.exe.bak as a backup)");
                    file.Close();
                }

                // Copy file first
                if (!File.Exists(args[0] + ".bak")) {
                    File.Copy(args[0], args[0] + ".bak");
                }

                using (FileStream file = File.Open(args[0], FileMode.Open)) {

                    int i = 1;
                    foreach (Patch patch in Patches) {
                        Console.WriteLine("Applying Patch #"+(i++)+" - "+patch.Name);
                        file.Seek(patch.Offset, SeekOrigin.Begin);
                        file.Write(patch.PatchedBytes, 0, patch.PatchedBytes.Length);
                    }
                    Console.WriteLine("Successfully patched Donald.exe!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();

                    file.Close();
                }

            } catch (IOException e) {
                Console.WriteLine("IOException, try running this batch file as administrator");
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}

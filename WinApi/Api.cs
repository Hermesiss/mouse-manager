using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WinApi {
    public static class Api {
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SendInput" , CharSet = CharSet.Auto)]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        public enum SystemMetric {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric smIndex);

        public static (int x,int y) GetScreenSize() {
            return (GetSystemMetrics(SystemMetric.SM_CXSCREEN), GetSystemMetrics(SystemMetric.SM_CYSCREEN));
        }

        private static int CalculateAbsoluteCoordinateX(int x) {
            return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        private static int CalculateAbsoluteCoordinateY(int y) {
            return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }

        public static async Task MakeClick(int x, int y) {
            var sw = new Stopwatch();
            sw.Start();
            x = CalculateAbsoluteCoordinateX(x);
            y = CalculateAbsoluteCoordinateY(y);
            await Task.Delay(50);
            var inputs = new[] {
                new Input {
                    type = (int)InputType.Mouse,
                    u = new InputUnion {
                        mi = new MouseInput {
                            dwFlags = (uint)(MouseEventF.LeftDown),
                            //dwExtraInfo = GetMessageExtraInfo(),
                            time = 0
                        }
                    }
                },
                new Input {
                    type = (int)InputType.Mouse,
                    u = new InputUnion {
                        mi = new MouseInput {
                            dwFlags = (uint)MouseEventF.LeftUp,
                            //dwExtraInfo = GetMessageExtraInfo(),
                            time = 0,
                        }
                    }
                }
            };

            //Console.WriteLine("02: Do mouse click {0}:{1} in {2} ms", x, y, sw.ElapsedMilliseconds);
            var sizeOf = Marshal.SizeOf(typeof(Input));
            //Console.WriteLine("Size of {0} ms", sw.ElapsedMilliseconds);
            var inputsLength = (uint)inputs.Length;
            //Console.WriteLine("InputsLength {0} ms", sw.ElapsedMilliseconds);
            var result = SendInput(inputsLength, inputs, sizeOf);
            //Console.WriteLine("Result {1}  {0} ms", sw.ElapsedMilliseconds, result);
            //Thread.Sleep(50);
            /*inputs[0] = new Input {
                type = (int)InputType.Mouse,
                u = new InputUnion {
                    mi = new MouseInput {
                        dwFlags = (uint)MouseEventF.LeftUp,
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };*/
            /*Console.WriteLine("03: Mouse up");
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));*/
            Console.WriteLine("03: Input successfully sent in {0} ms", sw.ElapsedMilliseconds);
            sw.Stop();
        }
    }
}
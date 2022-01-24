using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;

namespace MouseManager
{
    class Program
    {
        private static Control _hookControl;

        private static async Task<int> Main(string[] args) {
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult( opts =>
                    {
                        try
                        {
                            var thread = new Thread(() =>HookThread(opts)) {
                                IsBackground = true
                            };
                            thread.Start();

                            //keyboardHook.KeyUpEvent += KeyUp;
            
                            Console.ReadLine();
                            return Task.FromResult(0);
                        }
                        catch
                        {
                            Console.WriteLine("Error!");
                            return Task.FromResult(-3); // Unhandled error
                        }
                    },
                    errs => Task.FromResult(-1)); // Invalid arguments

           
            //await RunHandling();
        }

        private static void TerminateThread() {
            // _hookControl.BeginInvoke(((Action)(() => Application.ExitThread())));
        }
        
        private static void HookThread(CommandLineOptions opts)
        {
            //_hookControl = new Control();
            
            //var keyboardHook = new Hook("Global Action Hook");
            //keyboardHook.KeyDownEvent += KeyDown;
            
            var handling = new EventHandling(true, opts.Width,opts.Height, opts.Left, opts.Top);
            //var handling = new EventHandling(true, 300,500, 200, 200);
            //var handling = new EventHandling(true, 1120,680, 400, 200);
            
            handling.SubscribeOnMouseDown(true);
            
            /*var handle = _hookControl.Handle;

            _hookProc = new HookProc(HookFunction);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hook = SetWindowsHookEx(HookType.WH_MOUSE_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);// (uint)AppDomain.GetCurrentThreadId());
            }*/

            Application.Run();

            /*UnhookWindowsHookEx(_hook);
            _hook = IntPtr.Zero;*/
        }
        
        private static void KeyDown(KeyboardHookEventArgs e)
        {
            Console.WriteLine(e.Key);
            // handle keydown event here
            // Such as by checking if e (KeyboardHookEventArgs) matches the key you're interested in
    
            if (e.Key == Keys.F12 && e.isCtrlPressed)
            {
                // Do your magic...
            }
        }
        
        

        private static async Task RunHandling() {
            while (true) {
                await Task.Delay(1000);
            }
        }
    }
}

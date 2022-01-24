using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using InputHooks;
using WinApi;

namespace MouseManager {
    public class EventHandling {
        public EventHandling(bool useGlobal, uint width, uint height, uint left = 0, uint top = 0) {
            _useGlobal = useGlobal;
            _left = left;
            _top = top;
            _width = width;
            _height = height;
            _screen = Api.GetScreenSize();
            HookManager.UseGlobal = useGlobal;
        }

        private bool _manualClick;
        private readonly bool _useGlobal;

        private readonly uint _height, _width, _top, _left;
        private readonly (int x, int y) _screen;


        public async Task DoMouseClick(int x, int y) {
            if (_manualClick) return;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _manualClick = true;

            await Api.MakeClick(x, y);
            _manualClick = false;
            stopwatch.Stop();
            Console.WriteLine("Perform click in {0} ms", stopwatch.ElapsedMilliseconds);
        }

        //##################################################################

        #region Check boxes to set or remove particular event handlers.

        public void SubscribeOnMouseMove(bool mode) {
            if (mode) {
                HookManager.MouseMove += MouseMove;
            }
            else {
                HookManager.MouseMove -= MouseMove;
            }
        }

        public void SubscribeOnMouseClick(bool mode) {
            if (mode) {
                HookManager.MouseClick += MouseClick;
            }
            else {
                HookManager.MouseClick -= MouseClick;
            }
        }

        public void SubscribeOnMouseUp(bool mode) {
            if (mode) {
                HookManager.MouseUp += MouseUp;
            }
            else {
                HookManager.MouseUp -= MouseUp;
            }
        }

        public void SubscribeOnMouseDown(bool mode) {
            Console.WriteLine("SubscribeOnMouseDown");
            if (mode) {
                HookManager.MouseDown += MouseDown;
            }
            else {
                HookManager.MouseDown -= MouseDown;
            }
        }

        public void SubscribeMouseDoubleClick(bool mode) {
            if (mode) {
                HookManager.MouseDoubleClick += MouseDoubleClick;
            }
            else {
                HookManager.MouseDoubleClick -= MouseDoubleClick;
            }
        }

        public void SubscribeMouseWheel(bool mode) {
            if (mode) {
                HookManager.MouseWheel += MouseWheel;
            }
            else {
                HookManager.MouseWheel -= MouseWheel;
            }
        }

        public void SubscribeKeyDown(bool mode) {
            if (mode) {
                HookManager.KeyDown += KeyDown;
            }
            else {
                HookManager.KeyDown -= KeyDown;
            }
        }


        public void SubscribeKeyUp(bool mode) {
            if (mode) {
                HookManager.KeyUp += KeyUp;
            }
            else {
                HookManager.KeyUp -= KeyUp;
            }
        }

        public void SubscribeKeyPress(bool mode) {
            if (mode) {
                HookManager.KeyPress += KeyPress;
            }
            else {
                HookManager.KeyPress -= KeyPress;
            }
        }

        #endregion

        //##################################################################

        #region Event handlers of particular events. They will be activated when an appropriate subscribe is checked.

        private void KeyDown(object sender, KeyEventArgs e) {
            /*textBoxLog.AppendText(string.Format("KeyDown - {0}\n", e.KeyCode));
            textBoxLog.ScrollToCaret();*/
        }

        private void KeyUp(object sender, KeyEventArgs e) {
            /*textBoxLog.AppendText(string.Format("KeyUp - {0}\n", e.KeyCode));
            textBoxLog.ScrollToCaret();*/
        }


        private void KeyPress(object sender, KeyPressEventArgs e) {
            /*textBoxLog.AppendText(string.Format("KeyPress - {0}\n", e.KeyChar));
            textBoxLog.ScrollToCaret();*/
        }


        private void MouseMove(object sender, MouseEventArgs e) {
            var x = e.X;
            var y = e.Y;

            /*Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X /100*100, Cursor.Position.Y /100*100);*/

            //var ev = e as MouseEventExtArgs;
            //ev.Handled = true;

            /*if (_manualClick) {
                _manualClick = false;
                return;
            }*/


            //labelMousePosition.Text = string.Format("x={0:0000}; y={1:0000}, {2:0000}, {3:0000}", x, y, Cursor.Position.X, Cursor.Position.Y);
            /*Console.WriteLine(e.Button);
            Console.WriteLine(e.Clicks);
            if (e.Button == MouseButtons.Left) {
                //DoMouseClick((uint)Cursor.Position.X/100*100,(uint)Cursor.Position.Y/100*100); //TODO fix endless loop
                Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new Point(Cursor.Position.X /100*100, Cursor.Position.Y /100*100);
                ev.OverridePoint = new HookManager.Point(Cursor.Position.X/100*100, Cursor.Position.Y/100*100);
            }*/

            //Console.WriteLine(string.Format(@"{0}, {1}", Cursor.Position.X, Cursor.Position.Y));
        }

        private void MouseClick(object sender, MouseEventArgs e) {
            /*if (_manualClick) {
                _manualClick = false;
                return;
            }*/

            //textBoxLog.AppendText(string.Format("MouseClick - {0}\n", e.Button));
            //textBoxLog.ScrollToCaret();

            /*var ev = e as MouseEventExtArgs;
            //ev.Handled = true;
            Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X /100*100, Cursor.Position.Y /100*100);
            ev.OverridePoint = new HookManager.Point(Cursor.Position.X, Cursor.Position.Y);*/

            /*MouseEventExtArgs ev = e as MouseEventExtArgs;
            ev.Handled = true;
            Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X /100*100, Cursor.Position.Y /100*100);*/
            //DoMouseClick();


            //Cursor.Position = new Point(Cursor.Position.X /100*100, Cursor.Position.Y /100*100);

            //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
            //Cursor.Clip = new Rectangle(this.Location, this.Size);
        }

        private void MouseUp(object sender, MouseEventArgs e) {
            /*if (_manualClick) {
                _manualClick = false;
                return;
            }*/

            //textBoxLog.AppendText(string.Format("MouseUp - {0}\n", e.Button));
            //textBoxLog.ScrollToCaret();
        }


        private void MouseDown(object sender, MouseEventArgs e) {
            //Debug.WriteLine(string.Format("Manual click {0}, x {1}, y {2}", _manualClick, e.X, e.Y));
            Console.WriteLine($"Mouse down: Manual click {_manualClick}, x {e.X}, y {e.Y}");
            if (_manualClick) {
                //_manualClick = false;

                //DoMouseClick((uint)Cursor.Position.X/100*100,(uint)Cursor.Position.Y/100*100);
                return;
            }

            Console.WriteLine("Button {0}", e.Button);

            if (e.Button == MouseButtons.Left) {
                //_manualClick = true;
                ((MouseEventExtArgs)e).Handled = true;
                //var cursor = new Cursor(Cursor.Current.Handle);
                var newX = (int)(_left + (Cursor.Position.X / (float)_screen.x) * _width);
                var newY = (int)(_top + (Cursor.Position.Y / (float)_screen.y) * _height);
                Cursor.Position = new Point(newX, newY);
                //DoMouseClick((uint)Cursor.Position.X / 100 * 100, (uint)Cursor.Position.Y / 100 * 100);
                _ = DoMouseClick(newX, newY);
            }

            //textBoxLog.AppendText(string.Format("MouseDown - {0}\n", e.Button));
            //textBoxLog.ScrollToCaret();

            //return;
            //Cursor.Position = new Point(Cursor.Position.X /100*100, Cursor.Position.Y /100*100);
            //((MouseEventExtArgs)e).OverridePoint = new HookManager.Point(Cursor.Position.X /100*100, Cursor.Position.Y /100*100);
        }


        private void MouseDoubleClick(object sender, MouseEventArgs e) {
            /*if (_manualClick) {
                _manualClick = false;
                return;
            }*/

            //textBoxLog.AppendText(string.Format("MouseDoubleClick - {0}\n", e.Button));
            //textBoxLog.ScrollToCaret();
        }


        private void MouseWheel(object sender, MouseEventArgs e) {
            /*if (_manualClick) {
                _manualClick = false;
                return;
            }*/

            //labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
        }

        #endregion
    }
}
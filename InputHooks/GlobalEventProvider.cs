using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace InputHooks {
    /// <summary>
    /// This component monitors all mouse activities globally (also outside of the application) 
    /// and provides appropriate events.
    /// </summary>
    public class GlobalEventProvider : Component {
        /// <summary>
        /// This component raises events. The value is always true.
        /// </summary>
        protected override bool CanRaiseEvents => true;

        //################################################################

        #region Mouse events

        private event MouseEventHandler MouseMoveEvt;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        public event MouseEventHandler MouseMove {
            add {
                if (MouseMoveEvt == null) {
                    HookManager.MouseMove += HookManager_MouseMove;
                }

                MouseMoveEvt += value;
            }

            remove {
                MouseMoveEvt -= value;
                if (MouseMoveEvt == null) {
                    HookManager.MouseMove -= HookManager_MouseMove;
                }
            }
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e) {
            MouseMoveEvt?.Invoke(this, e);
        }

        private event MouseEventHandler MouseClickEvt;

        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        public event MouseEventHandler MouseClick {
            add {
                if (MouseClickEvt == null) {
                    HookManager.MouseClick += HookManager_MouseClick;
                }

                MouseClickEvt += value;
            }

            remove {
                MouseClickEvt -= value;
                if (MouseClickEvt == null) {
                    HookManager.MouseClick -= HookManager_MouseClick;
                }
            }
        }

        private void HookManager_MouseClick(object sender, MouseEventArgs e) {
            MouseClickEvt?.Invoke(this, e);
        }

        private event MouseEventHandler MouseDownEvt;

        /// <summary>
        /// Occurs when the mouse a mouse button is pressed. 
        /// </summary>
        public event MouseEventHandler MouseDown {
            add {
                if (MouseDownEvt == null) {
                    HookManager.MouseDown += HookManager_MouseDown;
                }

                MouseDownEvt += value;
            }

            remove {
                MouseDownEvt -= value;
                if (MouseDownEvt == null) {
                    HookManager.MouseDown -= HookManager_MouseDown;
                }
            }
        }

        private void HookManager_MouseDown(object sender, MouseEventArgs e) {
            MouseDownEvt?.Invoke(this, e);
        }


        private event MouseEventHandler MouseUpEvt;

        /// <summary>
        /// Occurs when a mouse button is released. 
        /// </summary>
        public event MouseEventHandler MouseUp {
            add {
                if (MouseUpEvt == null) {
                    HookManager.MouseUp += HookManager_MouseUp;
                }

                MouseUpEvt += value;
            }

            remove {
                MouseUpEvt -= value;
                if (MouseUpEvt == null) {
                    HookManager.MouseUp -= HookManager_MouseUp;
                }
            }
        }

        private void HookManager_MouseUp(object sender, MouseEventArgs e) {
            MouseUpEvt?.Invoke(this, e);
        }

        private event MouseEventHandler MouseDoubleClickEvt;

        /// <summary>
        /// Occurs when a double clicked was performed by the mouse. 
        /// </summary>
        public event MouseEventHandler MouseDoubleClick {
            add {
                if (MouseDoubleClickEvt == null) {
                    HookManager.MouseDoubleClick += HookManager_MouseDoubleClick;
                }

                MouseDoubleClickEvt += value;
            }

            remove {
                MouseDoubleClickEvt -= value;
                if (MouseDoubleClickEvt == null) {
                    HookManager.MouseDoubleClick -= HookManager_MouseDoubleClick;
                }
            }
        }

        private void HookManager_MouseDoubleClick(object sender, MouseEventArgs e) {
            MouseDoubleClickEvt?.Invoke(this, e);
        }


        private event EventHandler<MouseEventExtArgs> MouseMoveExtEvt;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse movement in other applications.
        /// </remarks>
        public event EventHandler<MouseEventExtArgs> MouseMoveExt {
            add {
                if (MouseMoveExtEvt == null) {
                    HookManager.MouseMoveExt += HookManager_MouseMoveExt;
                }

                MouseMoveExtEvt += value;
            }

            remove {
                MouseMoveExtEvt -= value;
                if (MouseMoveExtEvt == null) {
                    HookManager.MouseMoveExt -= HookManager_MouseMoveExt;
                }
            }
        }

        private void HookManager_MouseMoveExt(object sender, MouseEventExtArgs e) {
            MouseMoveExtEvt?.Invoke(this, e);
        }

        private event EventHandler<MouseEventExtArgs> MouseClickExtEvt;

        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse click in other applications.
        /// </remarks>
        public event EventHandler<MouseEventExtArgs> MouseClickExt {
            add {
                if (MouseClickExtEvt == null) {
                    HookManager.MouseClickExt += HookManager_MouseClickExt;
                }

                MouseClickExtEvt += value;
            }

            remove {
                MouseClickExtEvt -= value;
                if (MouseClickExtEvt == null) {
                    HookManager.MouseClickExt -= HookManager_MouseClickExt;
                }
            }
        }

        private void HookManager_MouseClickExt(object sender, MouseEventExtArgs e) {
            MouseClickExtEvt?.Invoke(this, e);
        }

        #endregion

        //################################################################

        #region Keyboard events

        private event KeyPressEventHandler KeyPressEvt;

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <remarks>
        /// Key events occur in the following order: 
        /// <list type="number">
        /// <item>KeyDown</item>
        /// <item>KeyPress</item>
        /// <item>KeyUp</item>
        /// </list>
        ///The KeyPress event is not raised by noncharacter keys; however, the noncharacter keys do raise the KeyDown and KeyUp events. 
        ///Use the KeyChar property to sample keystrokes at run time and to consume or modify a subset of common keystrokes. 
        ///To handle keyboard events only in your application and not enable other applications to receive keyboard events, 
        /// set the KeyPressEventArgs.Handled property in your form's KeyPress event-handling method to <b>true</b>. 
        /// </remarks>
        public event KeyPressEventHandler KeyPress {
            add {
                if (KeyPressEvt == null) {
                    HookManager.KeyPress += HookManager_KeyPress;
                }

                KeyPressEvt += value;
            }
            remove {
                KeyPressEvt -= value;
                if (KeyPressEvt == null) {
                    HookManager.KeyPress -= HookManager_KeyPress;
                }
            }
        }

        private void HookManager_KeyPress(object sender, KeyPressEventArgs e) {
            KeyPressEvt?.Invoke(this, e);
        }

        private event KeyEventHandler KeyUpEvt;

        /// <summary>
        /// Occurs when a key is released. 
        /// </summary>
        public event KeyEventHandler KeyUp {
            add {
                if (KeyUpEvt == null) {
                    HookManager.KeyUp += HookManager_KeyUp;
                }

                KeyUpEvt += value;
            }
            remove {
                KeyUpEvt -= value;
                if (KeyUpEvt == null) {
                    HookManager.KeyUp -= HookManager_KeyUp;
                }
            }
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e) {
            KeyUpEvt?.Invoke(this, e);
        }

        private event KeyEventHandler KeyDownEvt;

        /// <summary>
        /// Occurs when a key is pressed. 
        /// </summary>
        public event KeyEventHandler KeyDown {
            add {
                if (KeyDownEvt == null) {
                    HookManager.KeyDown += HookManager_KeyDown;
                }

                KeyDownEvt += value;
            }
            remove {
                KeyDownEvt -= value;
                if (KeyDownEvt == null) {
                    HookManager.KeyDown -= HookManager_KeyDown;
                }
            }
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e) {
            KeyDownEvt?.Invoke(this, e);
        }

        #endregion
    }
}
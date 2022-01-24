using System;
using System.Windows.Forms;

namespace InputHooks {

    /// <summary>
    /// This class monitors all mouse activities globally (also outside of the application) 
    /// and provides appropriate events.
    /// </summary>
    public static partial class HookManager
    {
        //################################################################
        #region Mouse events

        private static event MouseEventHandler MouseMoveEvt;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        public static event MouseEventHandler MouseMove
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                MouseMoveEvt += value;
            }

            remove
            {
                MouseMoveEvt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgs> MouseMoveExtEvt;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse movement in other applications.
        /// </remarks>
        public static event EventHandler<MouseEventExtArgs> MouseMoveExt
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                MouseMoveExtEvt += value;
            }

            remove
            {

                MouseMoveExtEvt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseClickEvt;

        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        public static event MouseEventHandler MouseClick
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                MouseClickEvt += value;
            }
            remove
            {
                MouseClickEvt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgs> MouseClickExtEvt;

        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse click in other applications.
        /// </remarks>
        public static event EventHandler<MouseEventExtArgs> MouseClickExt
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                MouseClickExtEvt += value;
            }
            remove
            {
                MouseClickExtEvt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseDownEvt;

        /// <summary>
        /// Occurs when the mouse a mouse button is pressed. 
        /// </summary>
        public static event MouseEventHandler  MouseDown
        {
            add 
            { 
                EnsureSubscribedToGlobalMouseEvents();
                MouseDownEvt += value;
            }
            remove
            {
                MouseDownEvt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseUpEvt;

        /// <summary>
        /// Occurs when a mouse button is released. 
        /// </summary>
        public static event MouseEventHandler MouseUp
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                MouseUpEvt += value;
            }
            remove
            {
                MouseUpEvt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseWheelEvt;

        /// <summary>
        /// Occurs when the mouse wheel moves. 
        /// </summary>
        public static event MouseEventHandler MouseWheel
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                MouseWheelEvt += value;
            }
            remove
            {
                MouseWheelEvt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }


        private static event MouseEventHandler MouseDoubleClickEvt;

        //The double click event will not be provided directly from hook.
        //To fire the double click event wee need to monitor mouse up event and when it occures 
        //Two times during the time interval which is defined in Windows as a doble click time
        //we fire this event.

        /// <summary>
        /// Occurs when a double clicked was performed by the mouse. 
        /// </summary>
        public static event MouseEventHandler MouseDoubleClick
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                if (MouseDoubleClickEvt == null)
                {
                    //We create a timer to monitor interval between two clicks
                    _doubleClickTimer = new Timer
                    {
                        //This interval will be set to the value we retrive from windows. This is a windows setting from contro planel.
                        Interval = GetDoubleClickTime(),
                        //We do not start timer yet. It will be start when the click occures.
                        Enabled = false
                    };
                    //We define the callback function for the timer
                    _doubleClickTimer.Tick += DoubleClickTimeElapsed;
                    //We start to monitor mouse up event.
                    MouseUp += OnMouseUp;
                }
                MouseDoubleClickEvt += value;
            }
            remove
            {
                if (MouseDoubleClickEvt != null)
                {
                    MouseDoubleClickEvt -= value;
                    if (MouseDoubleClickEvt == null)
                    {
                        //Stop monitoring mouse up
                        MouseUp -= OnMouseUp;
                        //Dispose the timer
                        _doubleClickTimer.Tick -= DoubleClickTimeElapsed;
                        _doubleClickTimer = null;
                    }
                }
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        //This field remembers mouse button pressed because in addition to the short interval it must be also the same button.
        private static MouseButtons _prevClickedButton;
        //The timer to monitor time interval between two clicks.
        private static Timer _doubleClickTimer;

        private static void DoubleClickTimeElapsed(object sender, EventArgs e)
        {
            //Timer is elapsed and no second click occured
            _doubleClickTimer.Enabled = false;
            _prevClickedButton = MouseButtons.None;
        }

        /// <summary>
        /// This method is designed to monitor mouse clicks in order to fire a double click event if interval between 
        /// clicks was short enaugh.
        /// </summary>
        /// <param name="sender">Is always null</param>
        /// <param name="e">Some information about click heppened.</param>
        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            //This should not heppen
            if (e.Clicks < 1) { return;}
            //If the secon click heppened on the same button
            if (e.Button.Equals(_prevClickedButton))
            {
                if (MouseDoubleClickEvt!=null)
                {
                    //Fire double click
                    MouseDoubleClickEvt.Invoke(null, e);
                }
                //Stop timer
                _doubleClickTimer.Enabled = false;
                _prevClickedButton = MouseButtons.None;
            }
            else
            {
                //If it was the firts click start the timer
                _doubleClickTimer.Enabled = true;
                _prevClickedButton = e.Button;
            }
        }
        #endregion

        //################################################################
        #region Keyboard events

        private static event KeyPressEventHandler KeyPressEvt;

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
        public static event KeyPressEventHandler KeyPress
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                KeyPressEvt += value;
            }
            remove
            {
                KeyPressEvt -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static event KeyEventHandler KeyUpEvt;

        /// <summary>
        /// Occurs when a key is released. 
        /// </summary>
        public static event KeyEventHandler KeyUp
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                KeyUpEvt += value;
            }
            remove
            {
                KeyUpEvt -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static event KeyEventHandler KeyDownEvt;

        /// <summary>
        /// Occurs when a key is pressed. 
        /// </summary>
        public static event KeyEventHandler KeyDown
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                KeyDownEvt += value;
            }
            remove
            {
                KeyDownEvt -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }


        #endregion
    }
}

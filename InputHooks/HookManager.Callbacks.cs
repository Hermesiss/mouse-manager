using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InputHooks {
    public static partial class HookManager {
        /// <summary>
        /// The CallWndProc hook procedure is an application-defined or library-defined callback 
        /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
        /// to this callback function. CallWndProc is a placeholder for the application-defined 
        /// or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        //##############################################################################

        #region Mouse hook processing

        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private static HookProc _mouseDelegate;

        /// <summary>
        /// Stores the handle to the mouse hook procedure.
        /// </summary>
        private static int _mouseHookHandle;

        private static int _mOldX;
        private static int _mOldY;

        /// <summary>
        /// A callback function which will be called every Time a mouse activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam) {
            if (nCode < 0) return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
            //Marshall the data from callback.
            var mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct))!;

            //detect button clicked
            var button = MouseButtons.None;
            short mouseDelta = 0;
            var clickCount = 0;
            var mouseDown = false;
            var mouseUp = false;

            switch (wParam) {
                case WM_LBUTTONDOWN:
                    mouseDown = true;
                    button = MouseButtons.Left;
                    clickCount = 1;
                    break;
                case WM_LBUTTONUP:
                    mouseUp = true;
                    button = MouseButtons.Left;
                    clickCount = 1;
                    break;
                case WM_LBUTTONDBLCLK:
                    button = MouseButtons.Left;
                    clickCount = 2;
                    break;
                case WM_RBUTTONDOWN:
                    mouseDown = true;
                    button = MouseButtons.Right;
                    clickCount = 1;
                    break;
                case WM_RBUTTONUP:
                    mouseUp = true;
                    button = MouseButtons.Right;
                    clickCount = 1;
                    break;
                case WM_RBUTTONDBLCLK:
                    button = MouseButtons.Right;
                    clickCount = 2;
                    break;
                case WM_MOUSEWHEEL:
                    //If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
                    //One wheel click is defined as WHEEL_DELTA, which is 120. 
                    //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                    mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);

                    //TODO: X BUTTONS (I haven't them so was unable to test)
                    //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                    //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                    //and the low-order word is reserved. This value can be one or more of the following values. 
                    //Otherwise, MouseData is not used. 
                    break;
            }

            //generate event 
            var e = new MouseEventExtArgs(
                button,
                clickCount,
                mouseHookStruct.Point.X,
                mouseHookStruct.Point.Y,
                mouseDelta);

            //If someone listens to move and there was a change in coordinates raise move event
            if ((MouseMoveEvt != null || MouseMoveExtEvt != null) &&
                (_mOldX != mouseHookStruct.Point.X || _mOldY != mouseHookStruct.Point.Y)) {
                _mOldX = mouseHookStruct.Point.X;
                _mOldY = mouseHookStruct.Point.Y;
                MouseMoveEvt?.Invoke(null, e);

                MouseMoveExtEvt?.Invoke(null, e);
            }

            OverrideInputPoint(lParam, e, ref mouseHookStruct);

            //Mouse up
            if (MouseUpEvt != null && mouseUp) {
                MouseUpEvt.Invoke(null, e);
            }

            OverrideInputPoint(lParam, e, ref mouseHookStruct);

            //Mouse down
            if (MouseDownEvt != null && mouseDown) {
                MouseDownEvt.Invoke(null, e);
            }

            OverrideInputPoint(lParam, e, ref mouseHookStruct);
            //If someone listens to click and a click is happened
            if (MouseClickEvt != null && clickCount > 0) {
                MouseClickEvt.Invoke(null, e);
            }

            OverrideInputPoint(lParam, e, ref mouseHookStruct);
            //If someone listens to click and a click is happened
            if (MouseClickExtEvt != null && clickCount > 0) {
                MouseClickExtEvt.Invoke(null, e);
            }

            OverrideInputPoint(lParam, e, ref mouseHookStruct);
            //If someone listens to double click and a click is happened
            if (MouseDoubleClickEvt != null && clickCount == 2) {
                MouseDoubleClickEvt.Invoke(null, e);
            }

            OverrideInputPoint(lParam, e, ref mouseHookStruct);
            //Wheel was moved
            if (MouseWheelEvt != null && mouseDelta != 0) {
                MouseWheelEvt.Invoke(null, e);
            }

            OverrideInputPoint(lParam, e, ref mouseHookStruct);
            if (e.Handled) {
                return -1;
            }

            //call next hook
            return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }

        private static void OverrideInputPoint(IntPtr lParam, MouseEventExtArgs e,
            ref MouseLLHookStruct mouseHookStruct) {
            if (e.OverridePoint == null) return;

            Console.Write("Old lParam {0} ", lParam);
            mouseHookStruct.Point = e.OverridePoint.Value;
            e.OverridePoint = null;
            Marshal.StructureToPtr(mouseHookStruct, lParam, true);
            Console.WriteLine("New lParam {0}", lParam);
        }

        private static void EnsureSubscribedToGlobalMouseEvents() {
            // install Mouse hook only if it is not installed and must be installed
            Console.WriteLine("HOOK ENSURING");
            if (_mouseHookHandle != 0) return;
            //See comment of this field. To avoid GC to clean it up.
            _mouseDelegate = MouseHookProc;
            //install hook
            _mouseHookHandle = SetWindowsHookEx(
                UseGlobal ? WH_MOUSE_LL : WH_MOUSE,
                _mouseDelegate,
                Marshal.GetHINSTANCE(
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                0);
            //If SetWindowsHookEx fails.
            if (_mouseHookHandle != 0) return;
            //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
            var errorCode = Marshal.GetLastWin32Error();
            Console.WriteLine("ERROR {0}", errorCode);
            //do cleanup

            //Initializes and throws a new instance of the Win32Exception class with the specified error. 
            throw new Win32Exception(errorCode);
        }

        private static void TryUnsubscribeFromGlobalMouseEvents() {
            //if no subscribers are registered unsubscribe from hook
            if (MouseClickEvt == null &&
                MouseDownEvt == null &&
                MouseMoveEvt == null &&
                MouseUpEvt == null &&
                MouseClickExtEvt == null &&
                MouseMoveExtEvt == null &&
                MouseWheelEvt == null) {
                ForceUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static void ForceUnsubscribeFromGlobalMouseEvents() {
            if (_mouseHookHandle == 0) return;
            //uninstall hook
            var result = UnhookWindowsHookEx(_mouseHookHandle);
            //reset invalid handle
            _mouseHookHandle = 0;
            //Free up for GC
            _mouseDelegate = null;
            //if failed and exception must be thrown
            if (result == 0) {
                //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                var errorCode = Marshal.GetLastWin32Error();
                //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                throw new Win32Exception(errorCode);
            }
        }

        #endregion

        //##############################################################################

        #region Keyboard hook processing

        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private static HookProc _keyboardDelegate;

        /// <summary>
        /// Stores the handle to the Keyboard hook procedure.
        /// </summary>
        private static int _keyboardHookHandle;

        public static bool UseGlobal { get; set; }

        /// <summary>
        /// A callback function which will be called every Time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private static int KeyboardHookProc(int nCode, int wParam, IntPtr lParam) {
            //indicates if any of underlying events set e.Handled flag

            var handled = false;

            if (nCode >= 0) {
                //read structure KeyboardHookStruct at lParam
                var myKeyboardHookStruct =
                    (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct))!;
                //raise KeyDown
                if (KeyDownEvt != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)) {
                    var keyData = (Keys)myKeyboardHookStruct.VirtualKeyCode;
                    var e = new KeyEventArgs(keyData);
                    KeyDownEvt.Invoke(null, e);
                    handled = e.Handled;
                }

                // raise KeyPress
                if (KeyPressEvt != null && wParam == WM_KEYDOWN) {
                    var isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80);
                    var isDownCapslock = (GetKeyState(VK_CAPITAL) != 0);

                    var keyState = new byte[256];
                    _ = GetKeyboardState(keyState);
                    var inBuffer = new byte[2];
                    if (ToAscii(myKeyboardHookStruct.VirtualKeyCode,
                            myKeyboardHookStruct.ScanCode,
                            keyState,
                            inBuffer,
                            myKeyboardHookStruct.Flags) == 1) {
                        var key = (char)inBuffer[0];
                        if (isDownCapslock ^ isDownShift && char.IsLetter(key)) key = char.ToUpper(key);
                        var e = new KeyPressEventArgs(key);
                        KeyPressEvt.Invoke(null, e);
                        handled = handled || e.Handled;
                    }
                }

                // raise KeyUp
                if (KeyUpEvt != null && wParam is WM_KEYUP or WM_SYSKEYUP) {
                    var keyData = (Keys)myKeyboardHookStruct.VirtualKeyCode;
                    var e = new KeyEventArgs(keyData);
                    KeyUpEvt.Invoke(null, e);
                    handled = handled || e.Handled;
                }
            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                return -1;

            //forward to other application
            return CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
        }

        private static void EnsureSubscribedToGlobalKeyboardEvents() {
            // install Keyboard hook only if it is not installed and must be installed
            if (_keyboardHookHandle != 0) return;

            //See comment of this field. To avoid GC to clean it up.
            _keyboardDelegate = KeyboardHookProc;
            //install hook
            _keyboardHookHandle = SetWindowsHookEx(
                UseGlobal ? WH_KEYBOARD_LL : WH_KEYBOARD,
                //WH_KEYBOARD,
                _keyboardDelegate,
                Marshal.GetHINSTANCE(
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                0);
            //If SetWindowsHookEx fails.
            if (_keyboardHookHandle != 0) return;

            //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
            var errorCode = Marshal.GetLastWin32Error();
            //do cleanup

            //Initializes and throws a new instance of the Win32Exception class with the specified error. 
            throw new Win32Exception(errorCode);
        }

        private static void TryUnsubscribeFromGlobalKeyboardEvents() {
            //if no subscribers are registered unsubscribe from hook
            if (KeyDownEvt == null &&
                KeyUpEvt == null &&
                KeyPressEvt == null) {
                ForceUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static void ForceUnsubscribeFromGlobalKeyboardEvents() {
            if (_keyboardHookHandle == 0) return;

            //uninstall hook
            var result = UnhookWindowsHookEx(_keyboardHookHandle);
            //reset invalid handle
            _keyboardHookHandle = 0;
            //Free up for GC
            _keyboardDelegate = null;
            //if failed and exception must be thrown
            if (result == 0) {
                //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                var errorCode = Marshal.GetLastWin32Error();
                //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                throw new Win32Exception(errorCode);
            }
        }

        #endregion
    }
}
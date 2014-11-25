using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX.DirectInput;
using SlimDX.XInput;


namespace SlimFramework
{
    // This class manages user input functionality.
    public class UserInput : IDisposable
    {

        // MEMBER VARIABLES
        // ======================================================================================================================
        
        bool m_IsDisposed = false;

        // Our DirectInput object is held in this variable.
        DirectInput m_DirectInput;

        // Our keyboard variables.
        Keyboard m_Keyboard;
        KeyboardState m_KeyboardStateCurrent;
        KeyboardState m_KeyboardStateLast;

        // Our mouse variables.
        Mouse m_Mouse;
        MouseState m_MouseStateCurrent;
        MouseState m_MouseStateLast;

        // DirectInput joystick variables
        Joystick m_Joystick1;
        JoystickState m_Joy1StateCurrent;
        JoystickState m_Joy1StateLast;

        // XInput joystick variables
        Controller m_Controller1;
        Gamepad m_Controller1StateCurrent;
        Gamepad m_Controller1StateLast;




        // CONSTRUCTORS
        // ======================================================================================================================

        /// <summary>
        /// The constructor.
        /// </summary>
        public UserInput()
        {
            InitDirectInput();
            InitXInput();

            // We need to intiailize these because otherwise we will get a null reference error
            // if the program tries to access these on the first frame.
            m_KeyboardStateCurrent = new KeyboardState();
            m_KeyboardStateLast = new KeyboardState();

            m_MouseStateCurrent = new MouseState();
            m_MouseStateLast = new MouseState();

            m_Joy1StateCurrent = new JoystickState();
            m_Joy1StateLast = new JoystickState();

            m_Controller1StateCurrent = new Gamepad();
            m_Controller1StateLast = new Gamepad();
        }




        // NON-PUBLIC METHODS
        // ======================================================================================================================

        /// <summary>
        /// This method initializes DirectInput.
        /// </summary>
        private void InitDirectInput()
        {
            m_DirectInput = new DirectInput();
            if (m_DirectInput == null)
                return; // An error has occurred, initialization of DirectInput failed for some reason so simply return from this method.


            // Create our keyboard and mouse devices.
            m_Keyboard = new Keyboard(m_DirectInput);
            if (m_Keyboard == null)
                return;  // An error has occurred, initialization of the keyboard failed for some reason so simply return from this method.
            
            m_Mouse = new Mouse(m_DirectInput);
            if (m_Mouse == null)
                return; // An error has occurred, initialization of the mouse failed for some reason so simply return from this method.


            GetJoysticks();
            
        }


        /// <summary>
        /// This method initializes our XInput stuff.
        /// </summary>
        private void InitXInput()
        {
            m_Controller1 = new Controller(UserIndex.One);
        }




        // PUBLIC METHODS
        // ======================================================================================================================

        /// <summary>
        /// In the first version of this method, it gets the list of available game controllers and outputs it into Visual Studio's Output Pane.
        /// The new version of this method gets the first joystick from the list of available game controllers (if there are any) and prepares it for use.
        /// </summary>
        public void GetJoysticks()
        {
            IList<DeviceInstance> deviceList = m_DirectInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);

            for (int i = 0; i < deviceList.Count; i++)
            {
                if (i == 0)
                {
                    m_Joystick1 = new Joystick(m_DirectInput, deviceList[0].InstanceGuid);
                    if (m_Joystick1 == null)
                        return;  // An error has occurred, initialization of the joystick failed for some reason so simply return from this method.

                    
                    // Set the range to use for all of the axis on our game controller.
                    m_Joystick1.Properties.SetRange(-1000, 1000);
                }
            }



            // Below is the original code that was in this method before we modified it to the above code.
            // -------------------------------------------------------------------------------------------------
            /*
            
            IList<DeviceInstance> controllers = m_DirectInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
            if (controllers.Count < 1)
            {
                System.Diagnostics.Debug.WriteLine("NO GAME CONTROLLERS WERE FOUND!");
            }
            else
            {
                foreach (DeviceInstance device in controllers)
                {
                    System.Diagnostics.Debug.WriteLine("PRODUCT NAME: " + device.ProductName);
                }
            }
            
            */
        }


        /// <summary>
        /// This function updates the state variables.  It should be called from the game's UpdateScene() function before
        /// it does any input processing.  
        /// </summary>
        public void Update()
        {

            // Reacquire the devices in case another application has taken control of them and check for errors.
            if (m_Keyboard.Acquire().IsFailure ||
                m_Mouse.Acquire().IsFailure ||
                m_Joystick1.Acquire().IsFailure)
            {
                // We failed to successfully acquire one of the devices so abort updating the user input stuff by simply returning from this method.
                return;
            }


            // Update our keyboard state variables.
            m_KeyboardStateLast = m_KeyboardStateCurrent;
            m_KeyboardStateCurrent = m_Keyboard.GetCurrentState();




            // NOTE: All of the if statements below are for testing purposes.  In a real program, you would remove them or comment them out
            //       and then recompile before releasing your game.  This is because we don't want debug code slowing down the finished game


            // This is our test code for keyboard input via DirectInput.
            if (IsKeyPressed(Key.Space))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: KEY SPACE IS PRESSED!");
            if (IsKeyHeldDown(Key.Space))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: KEY SPACE IS HELD DOWN!");
            if (IsKeyPressed(Key.Z))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: KEY Z IS PRESSED!");



            // Update our mouse state variables.
            m_MouseStateLast = m_MouseStateCurrent;
            m_MouseStateCurrent = m_Mouse.GetCurrentState();

            // This is our test code for mouse input via DirectInput.
            if (IsMouseButtonPressed(0))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: LEFT MOUSE BUTTON IS PRESSED!");
            if (IsMouseButtonPressed(1))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: RIGHT MOUSE BUTTON IS PRESSED!");
            if (IsMouseButtonPressed(2))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: MIDDLE MOUSE BUTTON IS PRESSED!");




            // Update our DirectInput joystick state variables.
            m_Joy1StateLast = m_Joy1StateCurrent;
            m_Joy1StateCurrent = m_Joystick1.GetCurrentState();

            // This is our test code for using a joystick with DirectInput.

            // These first three lines of test code are commented out since they tend to spam the output pane which can be annoying
            // if you want to see the output from button presses.
            //System.Diagnostics.Debug.WriteLine("DIRECTINPUT: LEFT JOYSTICK POSITION - " + DI_LeftStickPosition().ToString());
            //System.Diagnostics.Debug.WriteLine("DIRECTINPUT: RIGHT JOYSTICK POSITION - " + DI_RightStickPosition().ToString());
            //System.Diagnostics.Debug.WriteLine("DIRECTINPUT: TRIGGERS AXIS - " + DI_TriggersAxis().ToString());

            if (m_Joy1StateCurrent.IsPressed(0))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 0 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(1))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 1 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(2))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 2 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(3))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 3 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(4))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 4 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(5))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 5 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(6))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 6 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(7))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 7 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(8))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 8 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(9))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 9 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(10))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 10 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(11))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 11 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(12))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 12 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(13))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 13 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(14))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 14 IS PRESSED!");
            if (m_Joy1StateCurrent.IsPressed(15))
                System.Diagnostics.Debug.WriteLine("DIRECTINPUT: BUTTON 15 IS PRESSED!");


           
            // Update our XInput controller state variables.
            m_Controller1StateLast = m_Controller1StateCurrent;
            m_Controller1StateCurrent = m_Controller1.GetState().Gamepad;

            // This is our test code for using a joystick with XInput.
            if (XI_IsButtonPressed(GamepadButtonFlags.A))
                System.Diagnostics.Debug.WriteLine("XINPUT: THE A BUTTON IS PRESSED!!");
            if (XI_IsButtonPressed(GamepadButtonFlags.B))
                System.Diagnostics.Debug.WriteLine("XINPUT: THE B BUTTON IS PRESSED!!");
            if (XI_IsButtonPressed(GamepadButtonFlags.X))
                System.Diagnostics.Debug.WriteLine("XINPUT: THE X BUTTON IS PRESSED!!");
            if (XI_IsButtonPressed(GamepadButtonFlags.Y))
                System.Diagnostics.Debug.WriteLine("XINPUT: THE Y BUTTON IS PRESSED!!");

        }




        // KEYBOARD/MOUSE METHODS
        // ======================================================================================================================

        /// <summary>
        /// This method checks if the specified key is pressed.
        /// </summary>
        /// <param name="key">The key to check the state of.</param>
        /// <returns>True if the key is pressed or false otherwise.</returns>
        public bool IsKeyPressed(Key key)
        {
            return m_KeyboardStateCurrent.IsPressed(key);
        }


        /// <summary>
        /// This method checks if the specified key was pressed during the previous frame.
        /// </summary>
        /// <param name="key">The key to check the state of.</param>
        /// <returns>True if the key was pressed during the previous frame, or false otherwise.</returns>
        public bool WasKeyPressed(Key key)
        {
            return m_KeyboardStateLast.IsPressed(key);
        }

        /// <summary>
        /// This method checks if the specified key is released.
        /// </summary>
        /// <param name="key">The key to check the state of.</param>
        /// <returns>True if the key is released or false otherwise.</returns>
        public bool IsKeyReleased(Key key)
        {
            return m_KeyboardStateCurrent.IsReleased(key);
        }

        /// <summary>
        /// This method checks if the specified key was released (not pressed) during the previous frame.
        /// </summary>
        /// <param name="key">The key to check the state of.</param>
        /// <returns>True if the key was not pressed during the previous frame, or false otherwise.</returns>
        public bool WasKeyReleased(Key key)
        {
            return m_KeyboardStateLast.IsReleased(key);
        }

        /// <summary>
        /// This method checks if the specified key is held down (meaning it has been held down for 2 or more consecutive frames).
        /// </summary>
        /// <param name="key">The key to check the state of.</param>
        /// <returns>True if the key is being held down or false otherwise.</returns>
        public bool IsKeyHeldDown(Key key)
        {
            return (m_KeyboardStateCurrent.IsPressed(key) && m_KeyboardStateLast.IsPressed(key));
        }




        /// <summary>
        /// This method checks if the specified mouse button is pressed.
        /// </summary>
        /// <param name="button">The button to check the state of. 0 = left button, 1 = right button, 2 = middle button</param>
        /// <returns>True if the button is pressed or false otherwise.</returns>
        public bool IsMouseButtonPressed(int button)
        {
            return m_MouseStateCurrent.IsPressed(button);
        }

        /// <summary>
        /// This method checks if the specified mouse button was pressed during the previous frame.
        /// </summary>
        /// <param name="button">The button to check the state of. 0 = left button, 1 = right button, 2 = middle button</param>
        /// <returns>True if the button was pressed during the previous frame or false otherwise.</returns>
        public bool WasMouseButtonPressed(int button)
        {
            return m_MouseStateLast.IsPressed(button);
        }

        /// <summary>
        /// This method checks if the specified mouse button is pressed.
        /// </summary>
        /// <param name="button">The button to check the state of. 0 = left button, 1 = right button, 2 = middle button.</param>
        /// <returns>True if the button is released or false otherwise.</returns>
        public bool IsMouseButtonReleased(int button)
        {
            return m_MouseStateCurrent.IsReleased(button);
        }

        /// <summary>
        /// This method checks if the specified mouse button was released (not pressed) during the previous frame.
        /// </summary>
        /// <param name="button">The button to check the state of. 0 = left button, 1 = right button, 2 = middle button</param>
        /// <returns>True if the button was released (not pressed) during the previous frame or false otherwise.</returns>
        public bool WasMouseButtonReleased(int button)
        {
            return m_MouseStateLast.IsReleased(button);
        }

        /// <summary>
        /// This method checks if the specified mouse button is being held down (meaning it has been held down for 2 or more consecutive frames).
        /// </summary>
        /// <param name="button">The button to check the state of. 0 = left button, 1 = right button, 2 = middle button</param>
        /// <returns>True if the button is held down or false otherwise.</returns>
        public bool IsMouseButtonHeldDown(int button)
        {
            return (m_MouseStateCurrent.IsPressed(button) && m_MouseStateLast.IsPressed(button));
        }

        /// <summary>
        /// This method checks if the mouse has moved since the previous frame.
        /// </summary>
        /// <returns>True if the mouse has moved since the previous frame, or false otherwise.</returns>
        public bool MouseHasMoved()
        {
            if ((m_MouseStateCurrent.X != m_MouseStateLast.X) ||
                (m_MouseStateCurrent.Y != m_MouseStateLast.Y))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This method gets the mouse position for the current frame.
        /// </summary>
        /// <returns>A System.Drawing.Point object containing the current mouse position.</returns>
        public System.Drawing.Point MousePosition()
        {
            return new System.Drawing.Point(m_MouseStateCurrent.X, m_MouseStateCurrent.Y);
        }

        /// <summary>
        /// This method gets the mouse position for the previous frame.
        /// </summary>
        /// <returns>A System.Drawing.Point object containing the mouse's position during the previous frame.</returns>
        public System.Drawing.Point LastMousePosition()
        {
            return new System.Drawing.Point(m_MouseStateLast.X, m_MouseStateLast.Y);
        }

        /// <summary>
        /// This method gets the scrollwheel value in most cases.
        /// Note that this value is a delta, or in other words it is the amount the scroll wheel has been moved
        /// since the last frame.
        /// </summary>
        /// <returns>The amount the scroll wheel has moved.  This can be positive or negative depending on which way it has moved.</returns>
        public int MouseWheelMovement()
        {
            return m_MouseStateCurrent.Z;
        }




        // DIRECTINPUT GAMEPAD METHODS
        // ======================================================================================================================

        // NOTE: The methods in this section have names beginning with DI_ to indicate that they use DirectInput.


        /// <summary>
        /// This method checks if the specified button on the game controller is pressed.
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button is pressed or false otherwise.</returns>
        public bool DI_IsButtonPressed(int button)
        {
            return m_Joy1StateCurrent.IsPressed(button);
        }


        /// <summary>
        /// This method checks if the specified button was pressed during the previous frame.
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button was pressed during the previous frame or false otherwise.</returns>
        public bool DI_WasButtonPressed(int button)
        {
            return m_Joy1StateLast.IsPressed(button);
        }


        /// <summary>
        /// This method checks if the specified button is released (not pressed).
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button is released (not pressed) or false otherwise.</returns>
        public bool DI_IsButtonReleased(int button)
        {
            return m_Joy1StateCurrent.IsReleased(button);
        }


        /// <summary>
        /// This method checks if the specified button was released (not pressed) during the previous frame.
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button was released (not pressed) during the previous frame.</returns>
        public bool DI_WasButtonReleased(int button)
        {
            return m_Joy1StateLast.IsReleased(button);
        }


        /// <summary>
        /// This method gets the position of the left joystick for most gamepad style game controllers.
        /// </summary>
        /// <returns>A System.Drawing.Point object containing the current position of the left stick in most cases.</returns>
        public System.Drawing.Point DI_LeftStickPosition()
        {
            return new System.Drawing.Point(m_Joy1StateCurrent.X, m_Joy1StateCurrent.Y);
        }


        /// <summary>
        /// This method gets the position of the right joystick for most gamepad style game controllers.
        /// </summary>
        /// <returns>A System.Drawing.Point object containing the current position of the right stick in most cases.</returns>
        public System.Drawing.Point DI_RightStickPosition()
        {
            return new System.Drawing.Point(m_Joy1StateCurrent.RotationX, m_Joy1StateCurrent.RotationY);
        }

        /// <summary>
        /// This method gets the current value of the axis that is used under DirectInput to represent the trigger keys if your
        /// gamepad style controller has them.
        /// </summary>
        /// <returns></returns>
        public int DI_TriggersAxis()
        {
            return m_Joy1StateCurrent.Z;
        }




        // XINPUT GAMEPAD METHODS
        // ======================================================================================================================

        /// <summary>
        /// This method checks if the specified button is pressed.
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button is pressed or false otherwise.</returns>
        public bool XI_IsButtonPressed(GamepadButtonFlags button)
        {
            return m_Controller1StateCurrent.Buttons.HasFlag(button);
        }

        /// <summary>
        /// This method checks if the specified button was pressed during the previous frame.
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button was pressed during the previous frame or false otherwise.</returns>
        public bool XI_WasButtonPressed(GamepadButtonFlags button)
        {
            return m_Controller1StateLast.Buttons.HasFlag(button);
        }

        /// <summary>
        /// This method checks if the specified button is released (not pressed).
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button is released (not pressed) or false otherwise.</returns>
        public bool XI_IsButtonReleased(GamepadButtonFlags button)
        {
            return !(m_Controller1StateCurrent.Buttons.HasFlag(button));
        }

        /// <summary>
        /// This method checks if the specified button was released (not pressed) during the previous frame.
        /// </summary>
        /// <param name="button">The button to check the state of.</param>
        /// <returns>True if the button was released (not pressed) during the previous frame or false otherwise.</returns>
        public bool XI_WasButtonReleased(GamepadButtonFlags button)
        {
            return !(m_Controller1StateLast.Buttons.HasFlag(button));
        }

        /// <summary>
        /// This method gets the current position for the left stick.
        /// </summary>
        /// <returns>A System.Drawing.Point object containing the position of the left stick.</returns>
        public System.Drawing.Point XI_LeftStickPosition()
        {
            return new System.Drawing.Point(m_Controller1StateCurrent.LeftThumbX, m_Controller1StateCurrent.LeftThumbY);
        }

        /// <summary>
        /// This method gets the current position for the right stick.
        /// </summary>
        /// <returns>A System.Drawing.Point object containing the position of the right stick.</returns>
        public System.Drawing.Point XI_RightStickPosition()
        {
            return new System.Drawing.Point(m_Controller1StateCurrent.RightThumbX, m_Controller1StateCurrent.RightThumbY);
        }        

        /// <summary>
        /// This method gets the value of the left trigger.
        /// </summary>
        /// <returns>The value of the left trigger.  It ranges from 0-255.</returns>
        public int XI_LeftTrigger()
        {
            return m_Controller1StateCurrent.LeftTrigger;
        }


        /// <summary>
        /// This method gets the value of the right trigger.
        /// </summary>
        /// <returns>The value of the right trigger.  It ranges from 0-255.</returns>
        public int XI_RightTrigger()
        {
            return m_Controller1StateCurrent.RightTrigger;
        }





        // INTERFACE METHODS
        // ======================================================================================================================

        // This section is for methods that are part of the interfaces that the class implements.


        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);

            // Since this Dispose() method already cleaned up the resources used by this object, there's no need for the
            // Garbage Collector to call this class's Finalizer, so we tell it not to.
            // We did not implement a Finalizer for this class as in our case we don't need to implement it.
            // The Finalize() method is used to give the object a chance to clean up its unmanaged resources before it
            // is destroyed by the Garbage Collector.  Since we are only using managed code, we do not need to
            // implement the Finalize() method.
            GC.SuppressFinalize(this);

        }

        protected void Dispose(bool disposing)
        {
            if (!this.m_IsDisposed)
            {
                /*
                * The following text is from MSDN  (http://msdn.microsoft.com/en-us/library/fs2xkftw%28VS.80%29.aspx)
                * 
                * 
                * Dispose(bool disposing) executes in two distinct scenarios:
                * 
                * If disposing equals true, the method has been called directly or indirectly by a user's code and managed and unmanaged resources can be disposed.
                * If disposing equals false, the method has been called by the runtime from inside the finalizer and only unmanaged resources can be disposed. 
                * 
                * When an object is executing its finalization code, it should not reference other objects, because finalizers do not execute in any particular order. 
                * If an executing finalizer references another object that has already been finalized, the executing finalizer will fail.
                */
                if (disposing)
                {
                    // Unregister events


                    // get rid of managed resources
                    if (m_DirectInput != null)
                        m_DirectInput.Dispose();

                    if (m_Keyboard != null)
                        m_Keyboard.Dispose();

                    if (m_Mouse != null)
                        m_Mouse.Dispose();

                    if (m_Joystick1 != null)
                        m_Joystick1.Dispose();


                }

                // get rid of unmanaged resources

            }

        }




        // PROPERTIES
        // ======================================================================================================================

        /// <summary>
        /// Returns a boolean value indicating whether or not this object has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_IsDisposed;
            }
        }

        /// <summary>
        /// Gets the keyboard object.
        /// </summary>
        public Keyboard Keyboard
        {
            get
            {
                return m_Keyboard;
            }
        }

        /// <summary>
        /// Gets the keyboard state for the current frame.
        /// </summary>
        public KeyboardState KeyboardState_Current
        {
            get
            {
                return m_KeyboardStateCurrent;
            }
        }

        /// <summary>
        /// Gets the keyboard state from the previous frame.
        /// </summary>
        public KeyboardState KeyboardState_Previous
        {
            get
            {
                return m_KeyboardStateLast;
            }
        }

        /// <summary>
        /// Gets the DirectInput joystick object.
        /// </summary>
        public Joystick Joystick1
        {
            get
            {
                return m_Joystick1;
            }
        }

        /// <summary>
        /// Gets the Joystick1 state for the current frame.
        /// </summary>
        public JoystickState Joy1State_Current
        {
            get
            {
                return m_Joy1StateCurrent;
            }
        }

        /// <summary>
        /// Gets the Joystick1 state from the previous frame.
        /// </summary>
        public JoystickState Joy1State_Last
        {
            get
            {
                return m_Joy1StateLast;
            }
        }

        /// <summary>
        ///  Gets the mouse object.
        /// </summary>
        public Mouse Mouse
        {
            get
            {
                return m_Mouse;
            }
        }

        /// <summary>
        /// Gets the mouse state for the current frame.
        /// </summary>
        public MouseState MouseState_Current
        {
            get
            {
                return m_MouseStateCurrent;
            }
        }

        /// <summary>
        /// Gets the mouse state from the previous frame.
        /// </summary>
        public MouseState MouseState_Previous
        {
            get
            {
                return m_MouseStateLast;
            }
        }

        /// <summary>
        /// Gets the XInput controller1 object.
        /// </summary>
        public Controller XInputController1
        {
            get
            {
                return m_Controller1;
            }
        }

        /// <summary>
        /// Gets the XInput Controller1 state for the current frame.
        /// </summary>
        public Gamepad XInput_Controller1State_Curr
        {
            get
            {
                return m_Controller1StateCurrent;
            }
        }

        /// <summary>
        /// Gets the XInput Controller1 state from the previous frame.
        /// </summary>
        public Gamepad XInput_Controller1State_Last
        {
            get
            {
                return m_Controller1StateLast;
            }
        }


    }
}

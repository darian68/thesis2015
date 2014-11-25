using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Diagnostics;
using SlimDX;
using SlimDX.Windows;


namespace SlimFramework
{
    public class GameWindow : IDisposable
    {

        // MEMBER VARIABLES
        // ======================================================================================================================

        private bool m_IsDisposed = false; // Indicates whether or not the game window has been disposed.
        private bool m_IsInitialized = false; // Indicates whether or not the game window has been initialized yet.

        private bool m_IsFullScreen = false; // Indicates whether or not the game window is running in fullscreen mode.        
        
        private bool m_IsPaused = false;     // Indicates whether the game is paused.

        private RenderForm m_Form; // The SlimDX form that will be our game window.
        private Color4 m_ClearColor; // The color to use when clearing the screen.

        // These are protected in case a subclass wants to override the GameLoop() method to create a different type
        // of game loop.
        private long m_CurrFrameTime;    // Stores the time for the current frame.
        private long m_LastFrameTime;    // Stores the time of the last frame.
        private int m_FrameCount;    // Stores the number of frames completed so far during the current second.
        private int m_FPS;           // Stores the number of frames we rendered during the previous second.




        // CONSTRUCTORS
        // ======================================================================================================================

        /// <summary>
        /// This is the constructor.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="fullscreen">Whether the window should be fullscreen or not.</param>
        public GameWindow(string title, int width, int height, bool fullscreen)
        {
            // Store parameters in member variables.
            m_IsFullScreen = fullscreen;
            m_ClearColor = new Color4(1.0f, 0.0f, 0.0f, 0.0f);


            // Create the game window that will display the game.
            m_Form = new RenderForm(title);
            m_Form.ClientSize = new System.Drawing.Size(width, height);

            // Hook up event handlers so we can recieve events from the RenderForm object.
            m_Form.FormClosed += FormClosed;

        }




        // PUBLIC METHODS
        // ======================================================================================================================

        /// <summary>
        /// This function is the main game loop.  It gets called repeatedly throughout the life of the game.
        /// </summary>
        public virtual void GameLoop()
        {
            m_LastFrameTime = m_CurrFrameTime;
            m_CurrFrameTime = Stopwatch.GetTimestamp();

            UpdateScene((double) (m_CurrFrameTime - m_LastFrameTime) / Stopwatch.Frequency);

            RenderScene();


            // This code tracks our frame rate.
            m_FPS = (int)(Stopwatch.Frequency / ( (float) (m_CurrFrameTime - m_LastFrameTime)));


            // DEBUG CODE
            // Uncomment the three lines below and you will see output in the Visual Studio output pane for each iteration of the game loop.
            // ===============================================================================================================================
            //Debug.WriteLine("FPS: " + m_FPS.ToString());
            //m_FrameCount++;
            //Debug.WriteLine("GAME LOOP ITERATION #" + m_FrameCount.ToString());

        }


        /// <summary>
        /// This function activates the main game loop, starting up the game.
        /// You should call this function at the end of the constructors of your most derived GameWindow classes.
        /// For example, GameWindow_D3D11 doesn't call this function in its constructor because if it did,
        /// the constructor of any class derived from GameWindow_D3D11 would never execute the body of its
        /// constructor because this thread gets stuck in the main game loop then instead.
        /// </summary>
        public void StartGameLoop()
        {
            // If initialization is already finished, then simply return.
            if (m_IsInitialized)
                return;

            m_IsInitialized = true;

            // Start the message pump.
            MessagePump.Run(m_Form, GameLoop);
        }


        /// <summary>
        /// This method is called once per frame and is where we update objects in our scene.
        /// <param name="frameTime">The amount of time (in seconds) that has elapsed since the previous update.</param>
        /// </summary>
        public virtual void UpdateScene(double frameTime)
        {

        }


        /// <summary>
        /// This function is called each time we need to render the next frame.
        /// </summary>
        public virtual void RenderScene()
        {
            if ((!this.IsInitialized) ||
                this.IsDisposed)
            {
                return;
            }

        }


        /// <summary>
        /// This function toggles fullscreen mode.  It does nothing here (except for updating the m_bFullscreen member variable) because thats all it needs to do in this base class.
        /// This function is overridden by derived classes that need to add code to toggle fullscreen mode.
        /// </summary>
        public virtual void ToggleFullscreen()
        {
            m_IsFullScreen = !m_IsFullScreen;
        }



        // INTERFACE METHODS
        // ======================================================================================================================

        // This section is for methods that are part of the interfaces that the class implements.

        /// <summary>
        /// This method implements the Dispose() method required by the IDisposable interface.
        /// Do not make this method virtual as a derived class should not be able to override this method.
        /// </summary>
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

        /// <summary>
        /// This is the GameWindow class's internal Dispose() method that actually disposes of the game window class.
        /// </summary>
        /// <param name="disposing">If this method is being called by this class's own code, then the value true should be passed into this parameter to indicate that this method was not called by the Garbage Collector.</param>
        protected virtual void Dispose(bool disposing)
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
                    m_Form.FormClosed -= this.FormClosed;


                    // get rid of managed resources
                }

                // get rid of unmanaged resources

            }


            m_IsDisposed = true;
        }




        // PROPERTIES
        // ======================================================================================================================

        /// <summary>
        /// Gets/Sets the color to clear the screen to before drawing the next frame.
        /// </summary>
        public Color4 ClearColor
        {
            get
            {
                return m_ClearColor;
            }
            protected set
            {
                m_ClearColor = value;
            }
        }

        /// <summary>
        /// Returns the time in ticks when we started processing the current frame.
        /// </summary>
        public long CurrentFrameTime
        {
            get
            {
                return m_CurrFrameTime;
            }
            protected set
            {
                m_CurrFrameTime = value;
            }
        }

        /// <summary>
        /// Returns the underlying RenderForm object that actually represents the window itself.
        /// </summary>
        public RenderForm FormObject
        {
            get
            {
                return m_Form;
            }
        }

        /// <summary>
        /// Returns the number of frames we are rendering per second.
        /// </summary>
        public int FramesPerSecond
        {
            get
            {
                return m_FPS;
            }
            protected set
            {
                m_FPS = value;
            }
        }

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
        /// Gets/Sets whether the game window is fullscreen or not.
        /// </summary>
        public bool IsFullscreen
        {
            get
            {
                return m_IsFullScreen;
            }
            protected set
            {
                m_IsFullScreen = value;
            }
        }

        /// <summary>
        /// Returns a boolean value indicating whether the GameWindow has finished initializing itself yet or not.
        /// </summary>
        virtual public bool IsInitialized
        {
            get
            {
                return m_IsInitialized;
            }
        }


        /// <summary>
        /// Returns a boolean value indicating whether or not the game is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return m_IsPaused;
            }
            protected set
            {
                m_IsPaused = value;
            }
        }

        /// <summary>
        /// Returns the time in ticks when the we started processing the previous frame.
        /// </summary>
        public long LastFrameTime
        {
            get
            {
                return m_LastFrameTime;
            }
            protected set
            {
                m_LastFrameTime = value;
            }
        }



        // EVENT HANDLERS
        // ======================================================================================================================

        /// <summary>
        /// This is the event handler for the form's Closed event.
        /// </summary>
        /// <param name="o">The object that fired this event.</param>
        /// <param name="e">The event arguments for this event.</param>
        public virtual void FormClosed(object o, FormClosedEventArgs e)
        {
            if (!m_IsDisposed)
                Dispose();
        }

    }
}

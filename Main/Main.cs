using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NVYVE.MVC
{
    /// <summary>
    /// A mimic of the main of regular application
    /// Kicks off the setup for any controllers that are needed by the application 
    /// </summary>
    public class Main : MonoBehaviour
    {
        /// <summary>
        /// Elements that needed the main not to allow the application to proceed out of the title viewer until they are finished
        /// </summary>
        static public List<UnityEngine.Object> startupLock = new List<Object>();

        /// <summary>
        /// The locked state of the main
        /// </summary>
        static public bool locked
        {
            get
            {
                return startupLock.Count > 0;
            }
        } // static public bool locked

        /// <summary>
        /// Lock the main with a given element
        /// </summary>
        /// <param name="ueObject">The object to lock the main with</param>
        static public void Lock(UnityEngine.Object ueObject)
        {
            startupLock.Add(ueObject);
        } // static public void Lock(UnityEngine.Object ueObject)

        /// <summary>
        /// Release a lock on the main from a given object
        /// </summary>
        /// <param name="ueObject">The object releaseing it's lock</param>
        static public void Unlock (UnityEngine.Object ueObject)
        {
            startupLock.Remove(ueObject);
        } // static public void Unlock (UnityEngine.Object ueObject)

        /// <summary>
        /// Flush all locks on the main out of the system
        /// </summary>
        static public void ClearAllLocks()
        {
            startupLock.Clear();
        } // static public void ClearAllLocks()
        
        public virtual void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// The start of the main and the entire application
        /// 
        /// *Note
        /// in this model of the MVC pattern nothing inside the system should have a start, rather by permission of the main and usually not directly, usually by the hand of a control, elements inside this system are allowed to start up
        /// </summary>
        public virtual void Start()
        {
            Main.ClearAllLocks();

            ViewerController.instance.Setup();
            ViewerController.instance.Populate();
        } // public virtual void Start()

        /// <summary>
        /// The update permitting all the ticks in the application
        /// </summary>
        public virtual void Update ()
        {
            ViewerController.instance.Tick(Time.deltaTime);
        } // public virtual void Update ()
    } // public class Main : MonoBehaviour
} // namespace NVYVE.MVC
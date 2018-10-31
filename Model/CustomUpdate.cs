using UnityEngine;
using System.Collections;
using System;

namespace NVYVE.MVC
{
    //---------------------------------------------------------------------------------------

    class CustomUpdate : IEnumerator 
    {
        float delay;
        Action action;
        //bool waitForEndOfFrame;
        bool pause = false;
		public bool stop = false;

        //private bool initialDelay;
        
        public CustomUpdate(float _delay, Action _action, bool _waitForEndOfFrame) 
        {
            this.delay = _delay;
            this.action = _action;
            //this.waitForEndOfFrame = _waitForEndOfFrame;
        }

        public CustomUpdate(float _delay, Action _action) 
        {
            this.delay = _delay;
            this.action = _action;
           // this.waitForEndOfFrame = true;
        }

		public void Stop()
		{
			stop = true;
		}

        public void Reset() 
        {
            //initialDelay = false;
        }

        public void Pause()
        {
            pause = true;
        }

        public void UnPause()
        {
            pause = false;
        }

        public bool MoveNext()
        {
			if (stop) 
			{
				return false;
			}

            if (!pause)
            {
                action();
            }
            return true;
        }

        /// <summary>
        /// Grabs the current event.
        /// </summary>
        /// <value>The current.</value>
        public object Current
        {
            get
            {
                try
                {                   
                    // Return WaitForSeconds, Unity will handle the delay for us.
                    return new WaitForSeconds(delay);
//                    if(waitForEndOfFrame)
//                    {
//                       return new WaitForEndOfFrame();
//                    }
                    // If we have already delayed do nothing.
//                    return null;
                }

                catch(IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}

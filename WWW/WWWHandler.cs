using NVYVE.MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NVYVE.WWW
{
    public class WWWHandler : Controller<WWWHandler>
    {
        #region /- Variables -----------------------------------------------------------------------------------------------
        public bool m_debug = false;
        public static new bool DEBUG
        {
            get { return (instance.m_debug); }
            set { instance.m_debug = value; }
        }

        public string m_authUser = "";
        public string m_authPassword = "";
        public string m_authAPIKey = "";

        public int m_retryCount = 3;
        public float m_retryDelay = 1.0f;

        public int m_maxThreads = 10;
        public float m_timeStamp = 0f;
        public float m_frequency = 1f;

        public List<WWWItem> m_items = new List<WWWItem>();
        #endregion

        #region /- Delegates -----------------------------------------------------------------------------------------------
        public delegate void WWWCallback(WWWItem www);
        #endregion

        #region /- Help ----------------------------------------------------------------------------------------------------
        [ContextMenu("Help")]
        void Context_Help() { HelpSystem.OpenURL(this.GetType().Name); }
        #endregion

        #region /- Initialize ----------------------------------------------------------------------------------------------
        void Start()
        {

            // Clear out the queue (should never be full at the start anyway)
            m_items.Clear();

        } // Start()
        #endregion

        #region /- Update --------------------------------------------------------------------------------------------------
        void LateUpdate()
        {
            if (m_items.Count > 0) // Has Items to process
            {
                if (Time.time > (m_timeStamp + m_frequency)) // Has time elapsed for a tick?
                {
                    ProcessItems();

                    // Update the timestamp for the next cycle
                    m_timeStamp = Time.time;
                } // time elapsed
            } // has items
        } // LateUpdate()
        #endregion

        #region /- Create WWW ----------------------------------------------------------------------------------------------
        public static WWWItem GetWWW(string url, WWWForm form, WWWCallback callback, string name = "")
        {
            return (GetWWW(url, form, callback, null, name));
        }

        //public static WWWItem GetWWWAvailability(string url, WWWForm form, WWWCallback callback, ModelRef model, string name = "")
        //{
        //    return (GetWWW(url, form, callback, model, null, name));
        //}

        //public static WWWItem GetWWW(string url, WWWForm form, WWWCallback callback, ModelRef model, WWWCallback callbackProgress = null, string name = "")
        //{
        //    if (Application.isPlaying)
        //    {
        //        // Add for htpasswd protected sites
        //        if (Instance.m_authUser != "") form.headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(Instance.m_authUser + ":" + Instance.m_authPassword)));
        //        //if (Instance.m_authAPIKey != "") form.AddField("apikey", Instance.m_authAPIKey); // API Key ?

        //        // Create a new WWWItem object and add it to the list
        //        WWWItem newItem = new WWWItem(url, form, callback, callbackProgress, model, name);
        //        newItem.retryCount = Instance.m_retryCount;
        //        newItem.retryDelay = Instance.m_retryDelay;

        //        Instance.m_items.Add(newItem);

        //        // Return to the calling function in case that wants to track the WWW call.
        //        return (newItem);
        //    }
        //    else
        //    {
        //        string debuginfo = string.Format("Application is not running!");
        //        Debug.LogWarningFormat("[ {0} ] {1}():\n{2}", Instance.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, debuginfo);
        //        return (null);
        //    }
        //} // GetWWW()

        public static WWWItem GetWWW(string url, WWWForm form, WWWCallback callback, WWWCallback callbackProgress = null, string name = "")
        {
            if (Application.isPlaying)
            {
                // Add for htpasswd protected sites
                if (instance.m_authUser != "") form.headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(instance.m_authUser + ":" + instance.m_authPassword)));
                //if (Instance.m_authAPIKey != "") form.AddField("apikey", Instance.m_authAPIKey); // API Key ?

                // Create a new WWWItem object and add it to the list
                WWWItem newItem = new WWWItem(url, form, callback, callbackProgress, name);
                newItem.retryCount = instance.m_retryCount;
                newItem.retryDelay = instance.m_retryDelay;

                instance.m_items.Add(newItem);

                // Return to the calling function in case that wants to track the WWW call.
                return (newItem);
            }
            else
            {
                string debuginfo = string.Format("Application is not running!");
                Debug.LogWarningFormat("[ {0} ] {1}():\n{2}", instance.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, debuginfo);
                return (null);
            }
        } // GetWWW()
        #endregion

        #region /- WWW - With Callbacks and Progress -----------------------------------------------------------------------
        public void ProcessItems()
        {
            string debuginfo = "";
            int max = m_items.Count;
            if (max > m_maxThreads) max = m_maxThreads; // More items than allowed max items, only process the first #

            List<WWWItem> usedItems = new List<WWWItem>();

            for (int i = 0; i < max; i++)
            {
                WWWItem item = m_items[i];
                if (item == null)
                {
                    continue;
                }
                else if (item.url == "") // Remove this WWWItem
                {
                    item.used = true;
                }
                else if (item.www == null) // Check if this WWWItem is already running or not
                {
                    debuginfo += string.Format("    - {0}\n", item.url);
                    debuginfo += string.Format("    - Starting WWW\n");
                    StartCoroutine(_GetWWW(item));
                }
                else // already running
                {
                    debuginfo += string.Format("    - {0}: Already running: {1:0.0}%\n", item.name, item.www.progress * 100f);
                }

            } // loop items

            // Gather the used WWWItems
            for (int i = 0; i < m_items.Count; i++) if (m_items[i].used) usedItems.Add(m_items[i]);

            // Remove them from the queue
            for (int i = 0; i < usedItems.Count; i++) m_items.Remove(usedItems[i]);

            if (DEBUG) Debug.LogFormat("[ {0} ] {1}():\n{2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, debuginfo);
        } // ProcessItems()

        /// <summary>
        /// Coroutine to download content from a remote server via HTTP.
        /// </summary>
        /// <param name="item">The WWWItem that holds information of this download.</param>
        IEnumerator _GetWWW(WWWItem item)
        {
            // Download
            int loopCounter = 0;

            item.success = false;
            item.startTime = System.DateTime.Now;

            while (!item.success && (item.retryCount > 0))
            {
                // Create the WWW object to do the web call
                item.www = new UnityEngine.WWW(item.url, item.form != null ? item.form.data : null, item.form.headers);

                while (!item.www.isDone)
                {
                    // Update the progress of the download
                    item.progress = item.www.progress;

                    // Do the progress update callback
                    if (item.callbackProgress != null)
                    {
                        item.callbackProgress(item);
                    }
                    yield return null;
                }

                // Done downloading - was it successful?
                item.content = item.www.text;
                if (string.IsNullOrEmpty(item.www.error))
                {
                    item.success = true; // No error, guess it was successful!
                }
                else
                {
                    item.error = item.www.error + "\n" + item.www.text;

                    // Wait before retrying
                    if (DEBUG) Debug.LogWarningFormat("Waiting for {0} seconds...", item.retryDelay);
                    yield return new WaitForSeconds(item.retryDelay);
                    item.retryCount--;
                    if (DEBUG) Debug.LogWarningFormat("Retrying: {0}/{1}...", m_retryCount - item.retryCount, m_retryCount);
                }

                yield return null;
                if (loopCounter++ > 10000) break;
            } // loop

            item.endTime = System.DateTime.Now;
            item.duration = new System.TimeSpan((item.endTime - item.startTime).Ticks);

            // Do the callback on download complete
            if (item.callback != null)
            {
                item.callback(item);
            }

        } // _GetWWW()
        #endregion

    } // WWWHandler()

} // NVYVE.Architecture2
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace NVYVE.WWW
{
    [System.Serializable, XmlRoot("wwwitem")]
    public class WWWItem
    {
        #region /- Properties ------------------------------------------------------------------------------------------
        /// <summary>
        /// Name of this WWW call for easier identifying, or to store some other value to be passed to the on complete callback. Not used for anything internally.
        /// </summary>
        [XmlElement]
        public string name = "";
        /// <summary>
        /// Can be used to store some other value to be passed to the on complete callback. Not used for anything internally.
        /// </summary>
        [XmlElement]
        public int id = -1;
        [XmlElement]
        public int deleteId = -1;

        // WWW
        [XmlElement]
        public string url = "";
        [XmlElement]
        public string urlForm = "";
        [XmlIgnore]
        public UnityEngine.WWW www;
        [XmlIgnore]
        public WWWForm form;
        //[XmlIgnore] Dictionary<string, string> headers          = new Dictionary<string, string>();
        [XmlElement]
        public string content = "";
        [XmlElement]
        public string error = null;

        [XmlElement]
        public float delay = 0f;
        [XmlElement]
        public float progress = 0f;
        [XmlElement]
        public bool success = false;
        [XmlElement]
        public int retryCount = 0;
        [XmlElement]
        public float retryDelay = 0f;

        [XmlElement]
        public DateTime startTime;
        [XmlElement]
        public DateTime endTime;
        [XmlElement]
        public TimeSpan duration;

        /// <summary>
        /// Marks this WWWItem has having been processed by the associated callback function, marked for deletion during the clean up phase.
        /// </summary>
        [XmlElement]
        public bool used = false;

        // Callbacks
        /// <summary>
        /// Callback Function to be called on URL completion (success or fail after # retries)
        /// </summary>
        [XmlIgnore]
        public WWWHandler.WWWCallback callback;
        [XmlIgnore]
        public WWWHandler.WWWCallback callbackProgress;
        //[XmlIgnore]
        //public ModelRef model;

        #endregion

        #region /- Constructor -----------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        public WWWItem()
        {
        } // WWWItem()

        /// <summary>
        /// Constructor for a new WWWItem.
        /// </summary>
        /// <param name="url">URL to access.</param>
        /// <param name="form">Form to pass to the server.</param>
        /// <param name="callback">Function to call when the web call has finished.</param>
        /// <param name="callbackProgress">Function to call while the web call is in progress, updates with the download progress.</param>
        /// <param name="callbackFailure">Function to call when the web call has failed (repeated x times, or returned with a specific error message).</param>
        public WWWItem(string url, WWWForm form, WWWHandler.WWWCallback callback, WWWHandler.WWWCallback callbackProgress, string name = "")
        {
            this.name = name;
            this.url = url;
            this.form = form;
            this.callback = callback;
            this.callbackProgress = callbackProgress;
            this.urlForm = string.Format("{0}?{1}", this.url, this.form.data.convertToString());
            //Debug.Log(this.urlForm);
        } // WWWItem()

        //public WWWItem(string url, WWWForm form, WWWHandler.WWWCallback callback, WWWHandler.WWWCallback callbackProgress, ModelRef model, string name = "")
        //{
        //    this.name = name;
        //    this.url = url;
        //    this.form = form;
        //    this.callback = callback;
        //    this.callbackProgress = callbackProgress;
        //    this.model = model;
        //    this.urlForm = string.Format("{0}?{1}", this.url, this.form.data.convertToString());
        //    //Debug.Log(this.urlForm);
        //} // WWWItem()

        public WWWItem(WWWItem source)
        {
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(this, field.GetValue(source));
            } // loop fields
        } // WWWItem()
        #endregion


    } // WWWItem()
} // NVYVE.Architecture2
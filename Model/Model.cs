using UnityEngine;
using System.Collections;

namespace NVYVE.MVC
{
    /// <summary>
    /// The base class for all elements of data that exist within the project
    /// </summary>
    public class Model : MonoBehaviour, System.IComparable<Model>
    {
        /// <summary>
        /// Unique ID for a particular model
        /// </summary>
        public string id;

        /// <summary>
        /// Unique description for a particular model
        /// </summary>
        public string description;

        /// <summary>
        /// The delegate of data changes for model
        /// </summary>
        public delegate void ModelUpdate();
        /// <summary>
        /// The indicating a need to relook at the data inside a model
        /// </summary>
        public event ModelUpdate ModelUpdated;

        /// <summary>
        /// A flag indicate the necessity of push a ModelUpdate event
        /// </summary>
        public bool dirty;
        
        /// <summary>
        /// Special note, you may want the model tick to run after your classes 
        /// </summary>
        /// <param name="timeTick"></param>
        public virtual void Tick(float timeTick)
        {
            if (dirty)
            {
                try
                {
                    ModelUpdated();
                }
                catch
                {

                }
            }
            dirty = false;
        }

        /// <summary>
        /// The set class for a model
        /// </summary>
        public virtual void Setup() { }

        /// <summary>
        /// A reset class for a model
        /// </summary>
        public virtual void Reset() { }

        public int CompareTo(Model other)
        {
            return id.CompareTo(other.id);
        } // public int CompareTo(Data other)
    } // public class Data : MonoBehaviour
} // namespace NVYVE.MVC

/*
honestly? 

because coding is viewed as something "any can pick up", and people only care about what they see come out on the other side, a blind eye is turned to what happens inside
*/

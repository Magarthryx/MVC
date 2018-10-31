using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NVYVE;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// http://wiki.unity3d.com/index.php/Singleton
/// </summary>
/// 
namespace NVYVE.MVC
{
    public class Controller<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region /- Variables -----------------------------------------------------------------------------------------------
        private static T _instance;
        private static GameObject _container;

        private static object _lock = new object();
        private static bool _application_is_quitting = false;

        public static bool DEBUG = false;
        private static int birthSceneIndex = -1;
        
        [Header("[ Unity Singleton ]")]
        public bool debug = false;

        public bool isPersistent = false;

        [Space(15)]
        protected bool isValidSingleton = true;

        //IEnumerator LockingTest()
        //{
        //    Main.Lock(this);
        //    Debug.Log("start");
        //    yield return new WaitForSeconds(5f);
        //    Debug.Log("end");
        //    Main.Unlock(this);
        //
        //} // IEnumerator LockingTest()
        #endregion

        #region /- Singleton - Get/Set -------------------------------------------------------------------------------------
        public static T instance
        {
            get
            {
                if (_application_is_quitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[" + typeof(T).Name + "] Something went really wrong - there should never be more than 1 singleton.");
                            return (_instance);
                        }

                        if (_instance == null) // Not found, create new GameObject and component
                        {
                            _container = new GameObject();
                            _instance = _container.AddComponent<T>();
                            _container.name = typeof(T).ToString() + "(singleton)";

                            //						DontDestroyOnLoad(_container);

                            if (DEBUG) Debug.Log("[" + typeof(T).Name + "]: Needed in the scene, '" + _container.name + "' created.", _container);
                        }
                        else
                        {
                            _container = _instance.gameObject;

                            if (DEBUG) Debug.Log("[" + typeof(T).Name + "] Using instance already created: " + _container.name, _container);
                        }
                    }

                    return _instance;
                }
            }
        }
        #endregion

        #region /- Singleton - Accessors -----------------------------------------------------------------------------------
        public static GameObject Container
        {
            get
            {
                return (_container);
            }
        }
        #endregion

        #region /- Singleton - Methods -------------------------------------------------------------------------------------
        private void SetupSingleton()
        {
            //Debug.Log("[" + typeof(T).Name + "] SetupSingleton: " + gameObject.name, gameObject);

            if (_instance == null)
            {
                _instance = this as T;
                _container = this.gameObject;
                birthSceneIndex = SceneManager.GetActiveScene().buildIndex;
                if (isPersistent)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }
            else if (_instance != this) // not null, and this is not the assigned instance
            {
                if (_container.GetComponent<Controller<T>>().isPersistent != isPersistent)
                    Debug.LogError("[" + typeof(T).Name + "] Multiple instances of singletons have inconsistent persistency:" + gameObject.name + " and " + _container.name, gameObject);

                if (isPersistent)
                {
                    if (SceneManager.GetActiveScene().buildIndex != birthSceneIndex)
                    {
                        //make sure there's only one placement of a persistent singleton throughout all scenes in the project
                        Debug.Log("[" + typeof(T).Name + "] Only one instance of a persistent singleton is allowed across all scenes in the game: " + gameObject.name + " and " + _container.name, gameObject);
                    }
                    isValidSingleton = false;
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("[" + typeof(T).Name + "] Destroying old version, using new one: " + gameObject.name, gameObject);
                    Destroy(_container);
                    _instance = this as T;
                    _container = this.gameObject;
                }
            }

            if (isPersistent)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        } // SetupSingleton()
        #endregion

        #region /- Singleton - Events --------------------------------------------------------------------------------------
        public virtual void Setup() { }

        public virtual void Populate() { }

        public virtual void Tick(float timeTick) { }

        public static T1 ResourceLoad<T1>(string directory, string id) where T1 : UnityEngine.Object
        {
            foreach (T1 t in new List<T1>(Resources.LoadAll<T1>(directory)))
            {
                if (t.name.ToLower().CompareTo(id.ToString().ToLower()) == 0)
                {
                    return t;
                }
            }

            return null;
        } // public static T ResourceLoad<T>(string directory, string id) where T : UnityEngine.Object

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnDestroy()
        {
            _application_is_quitting = true;

            _instance = null;
            _container = null;

            StopAllCoroutines();
        }

        public virtual void Awake()
        {
            SetupSingleton();
        }
        #endregion

        public virtual List<T1> Filter<T1>(Filter filter) where T1 : Model
        {
            return new List<T1>();
        } // public virtual List<T1> Filter(Filter filter) where T1 : Model
    } // public class Controller<T> : MonoBehaviour where T : MonoBehaviour
} // namespace NVYVE.MVC
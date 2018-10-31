using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NVYVE
{
    /// <summary>
    /// Used primarily for updating slider values 
    /// </summary>
    [Serializable]
    public class Bandwidth
    {
        public static Bandwidth operator +(Bandwidth bw1, Bandwidth bw2)
        {
            return new Bandwidth(Mathf.Min(bw1.lower, bw2.lower), Mathf.Max(bw1.upper, bw2.upper));
        } // public static Bandwidth operator +(Bandwidth bw1, Bandwidth bw2)
        public static Bandwidth operator -(Bandwidth bw1, Bandwidth bw2)
        {
            return new Bandwidth(Mathf.Max(bw1.lower, bw2.lower), Mathf.Min(bw1.upper, bw2.upper));
        } // public static Bandwidth operator +(Bandwidth bw1, Bandwidth bw2)

        public float lower;
        public float upper;

        public bool filterZero { get; set; }

        /// <summary>
        /// Bandwidth constructor
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public Bandwidth(float lower, float upper)
        {
            this.lower = lower;
            this.upper = upper;
        } // public BandWidth (float lower, float upper)

        /// <summary>
        /// Check if a value is within bandwidth's range
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool WithinBandwidth(float amount)
        {
            //TODO : Uncomment if the client wants to show zero value homes
            if (filterZero && amount == 0) return false;
            //if (amount == 0) return true;
            return amount <= upper && amount >= lower;
        } // public bool WithinBandwidth(float amount)

        /// <summary>
        /// Returns Bandwidth as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return lower + " <-> " + upper;
        } // public override string ToString()

        /// <summary>
        /// Returns bandwith's range
        /// </summary>
        public float range
        {
            get
            {
                return upper - lower;
            }
        } // public float bandwidth

        public string ToString(string v)
        {
            switch (v.ToLower())
            {
                case "dollar":
                    return "$ " + lower.ToString("#.00") + " - $ " + upper.ToString("#.00");

                case "int":
                    return lower.ToInt() + " - " + upper.ToInt();

                default:
                    return ToString();
            }
        }
    } // public class BandWidth

    /// <summary>
    /// What the fuck is this even?
    /// </summary>
    public interface IFilterable
    {

    }

    /// <summary>
    /// Holds a list of KeyValuePairs that are used to filter unit data. Primarily used for UnitSearch, LotSelection, and HomeSelection
    /// </summary>
    [Serializable]
    public class Filter
    {
        /// <summary>
        /// Merges two filters together. Primarily used for merging, Bandwiths and Toggles.
        /// </summary>
        /// <param name="filterA"></param>
        /// <param name="filterB"></param>
        /// <returns></returns>
        public static Filter MergeFilters(Filter filterA, Filter filterB)
        {
            Filter returnFilter = new Filter();

            foreach (KeyValuePair<string, Bandwidth> bandwidth in filterA.bandwidths)
            {
                returnFilter.bandwidths.Add(bandwidth);
            }
            foreach (KeyValuePair<string, Bandwidth> bandwidth in filterB.bandwidths)
            {
                returnFilter.bandwidths.Add(bandwidth);
            }
            foreach (KeyValuePair<string, string> property in filterA.properties)
            {
                returnFilter.properties.Add(property);
            }
            foreach (KeyValuePair<string, string> property in filterB.properties)
            {
                returnFilter.properties.Add(property);
            }
            foreach (KeyValuePair<string, int> property in filterA.propertiesInt)
            {
                returnFilter.propertiesInt.Add(property);
            }
            foreach (KeyValuePair<string, int> property in filterB.propertiesInt)
            {
                returnFilter.propertiesInt.Add(property);
            }

            return returnFilter;
        } // public static Filter MergeFilters (Filter filterA, Filter filterB)

        public List<KeyValuePair<string, Bandwidth>> bandwidths;

        /// <summary>
        /// Returns a list of Bandwidths within this filter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Bandwidth> GetBandwiths(string key)
        {
            List<Bandwidth> returnList = new List<Bandwidth>();

            foreach (KeyValuePair<string, Bandwidth> bandwidth in bandwidths)
            {
                if (bandwidth.Key.CompareTo(key) == 0)
                {
                    returnList.Add(bandwidth.Value);
                }
            }
            return returnList;
        } // public Bandwidth GetBandwith(string key)

        /// <summary>
        /// Adds Bandwidth to this filter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="upper"></param>
        /// <param name="lower"></param>
        public void AddBandwidth(string key, float upper, float lower)
        {
            AddBandwidth(key, new Bandwidth(lower, upper));
        } // public void AddBandwidth (string key, float upper, float lower)

        /// <summary>
        /// Adds Bandwidth to this filter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bandwidth"></param>
        public void AddBandwidth(string key, Bandwidth bandwidth)
        {
            bandwidths.Add(new KeyValuePair<string, Bandwidth>(key, bandwidth));
        } // public void AddBandwidth(string key, Bandwidth bandwidth)

        public List<KeyValuePair<string, string>> properties;
        public List<KeyValuePair<string, int>> propertiesInt;

        /// <summary>
        /// Returns a list of string properties
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> GetProperties(string key)
        {
            List<string> returnList = new List<string>();

            foreach (KeyValuePair<string, string> property in properties)
            {
                if (property.Key.CompareTo(key) == 0)
                {
                    returnList.Add(property.Value);
                }
            }

            return returnList;
        } // public Bandwidth GetBandwith(string key)

        /// <summary>
        /// Returns a list of int properties
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<int> GetIntProperties(string key)
        {
            List<int> returnList = new List<int>();

            foreach (KeyValuePair<string, int> property in propertiesInt)
            {
                if (property.Key.CompareTo(key) == 0)
                {
                    returnList.Add(property.Value);
                }
            }

            return returnList;
        } // public List<int> GetIntProperties(string key)

        /// <summary>
        /// Adds string property to this filter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddProperty(string key, string value)
        {
            properties.Add(new KeyValuePair<string, string>(key, value));
        } // public void AddProperty(string key, string value)

        /// <summary>
        /// Adds int property to this filter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddProperty(string key, int value)
        {
            propertiesInt.Add(new KeyValuePair<string, int>(key, value));
        } // public void AddProperty(string key, int value)

        /// <summary>
        /// Basic constructor
        /// </summary>
        public Filter()
        {
            bandwidths = new List<KeyValuePair<string, Bandwidth>>();
            properties = new List<KeyValuePair<string, string>>();
            propertiesInt = new List<KeyValuePair<string, int>>();
        } // public Filter()

        /// <summary>
        /// Check if a float value is within a bandwidth
        /// </summary>
        /// <param name="withinAmount"></param>
        /// <param name="amountTypes"></param>
        /// <returns></returns>
        public bool WithinBandwidth(string bandwidthKey, float bandwidthValue)
        {
            foreach (Bandwidth bandwidth in GetBandwiths(bandwidthKey))
            {
                if (bandwidth.WithinBandwidth(bandwidthValue))
                {
                    return true;
                }
            }

            return false;
        } // bool WithinBandwidths(float withinAmount, string amountTypes)


        public bool HasBandwidth(string v)
        {
            return GetBandwiths(v).Count > 0;
        }

        #region Property

        public bool HasProperty(string v)
        {
            return GetProperties(v).Count > 0;
        }

        /// <summary>
        /// Returns true if string property finds match in this filter
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool HasProperty(string propertyKey, string propertyValue)
        {
            return GetProperties(propertyKey).Contains(propertyValue);
        } // bool HasProperty(string hasProperty, string propertyType)

        /// <summary>
        /// Returns true if int property finds match in this filter
        /// </summary>
        /// <param name="hasProperty"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        //public bool HasIntProperty(int hasProperty, string propertyType)
        //{
        //    return GetIntProperties(propertyType).Contains(hasProperty);
        //} // public bool HasIntProperty(int hasProperty, string propertyType)

        #endregion Property

        /// <summary>
        /// Returns a string containing all Bandwidths and Properties within this Filter
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string returnString = "";
            returnString += "Filter\n";
            returnString += "---------------------\n";
            returnString += "Bandwidths\n";
            foreach (KeyValuePair<string, Bandwidth> bandwidth in bandwidths)
                returnString += bandwidth.Key + ":" + bandwidth.Value + "\n";
            returnString += "---------------------\n";
            returnString += "Properties\n";
            foreach (KeyValuePair<string, string> property in properties)
                returnString += property.Key + ":" + property.Value + "\n";
            returnString += "---------------------\n";

            return returnString;
        } // public override string ToString()

    } // public class Filter
} // namespace NVYVE.Architecture
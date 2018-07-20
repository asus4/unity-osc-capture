using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace UniOSC
{
 
    /// <summary>
    /// The OscMessage class is a data structure that represents
    /// an OSC address and an arbitrary number of values to be sent to that address.
    /// </summary>
    public class OscMessage
    {
        /// <summary>
        /// The OSC address of the message as a string.
        /// </summary>
        public string address;
        /// <summary>
        /// The list of values to be delivered to the Address.
        /// </summary>
        public ArrayList values;

        public OscMessage()
        {
            values = new ArrayList();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(address);
            foreach (object o in values)
            {
                s.Append(" ");
                s.Append(o.ToString());
            }
            return s.ToString();

        }


        public int GetInt(int index)
        {

            if (values[index].GetType() == typeof(int))
            {
                int data = (int)values[index];
                if (Double.IsNaN(data)) return 0;
                return data;
            }
            else if (values[index].GetType() == typeof(float))
            {
                int data = (int)((float)values[index]);
                if (Double.IsNaN(data)) return 0;
                return data;
            }
            else
            {
                Debug.Log("Wrong type");
                return 0;
            }
        }

        public float GetFloat(int index)
        {

            if (values[index].GetType() == typeof(int))
            {
                float data = (int)values[index];
                if (Double.IsNaN(data)) return 0f;
                return data;
            }
            else if (values[index].GetType() == typeof(float))
            {
                float data = (float)values[index];
                if (Double.IsNaN(data)) return 0f;
                return data;
            }
            else
            {
                Debug.Log("Wrong type");
                return 0f;
            }
        }

    }   
}
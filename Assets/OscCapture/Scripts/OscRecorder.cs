using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;

namespace OscCapture
{
    [RequireComponent(typeof(OSC))]
    public class OscRecorder : MonoBehaviour
    {
        OSC osc;
        float startTime;
        Sequence sequence;

        void OnEnable()
        {
            osc = GetComponent<OSC>();
            osc.SetAllMessageHandler(OnOSCMessage);
            sequence = ScriptableObject.CreateInstance<Sequence>();
        }

        void OnDisable()
        {
            osc.SetAllMessageHandler(null);
        }

        void OnOSCMessage(OscMessage msg)
        {
            if (IsRecording)
            {
                sequence.AddMessage(Time.realtimeSinceStartup - startTime, msg);
            }
        }

        public void StartRecord()
        {
            Debug.Log("StartRecord");
            startTime = Time.realtimeSinceStartup;
            IsRecording = true;
        }

        public void StopRecord()
        {
            Debug.Log("StopRecord");
            startTime = float.NaN;
            IsRecording = false;
        }

        public Sequence Sequence { get { return sequence; } }

        public bool IsRecording { get; private set; }
    }
}
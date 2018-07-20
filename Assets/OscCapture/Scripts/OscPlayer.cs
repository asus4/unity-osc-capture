using UnityEngine;
using UniOSC;

namespace OscCapture
{
    [RequireComponent(typeof(OSC))]
    public class OscPlayer : MonoBehaviour
    {
        [SerializeField]
        TextAsset csv;

        OSC osc;
        Sequence sequence;
        float startTime;

        void Start()
        {
            osc = GetComponent<OSC>();
            sequence = ScriptableObject.CreateInstance<Sequence>();
            sequence.FromCsv(csv.text);
        }

        void Update()
        {
            if (!IsPlaying) { return; }

            float time = Time.realtimeSinceStartup - startTime;
            var messages = sequence.GetFrames(time);
            if (messages.Length > 0)
            {
                osc.Send(messages);
            }
        }

        public void Play()
        {
            startTime = Time.realtimeSinceStartup;
            IsPlaying = true;
        }

        public void Stop()
        {
            startTime = -1;
            IsPlaying = false;
        }

        public bool IsPlaying { get; private set; }
    }
}
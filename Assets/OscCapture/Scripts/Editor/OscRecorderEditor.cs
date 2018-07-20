using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace OscCapture
{
    [CustomEditor(typeof(OscRecorder))]
    public class OscRecorderEditor : Editor
    {
        OscRecorder recorder;

        void OnEnable()
        {
            recorder = (OscRecorder)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying)
            {
                return;
            }
            // Show on playing only

            if (recorder.IsRecording)
            {
                if (GUILayout.Button("Stop Record"))
                {
                    recorder.StopRecord();
                    SaveTextFile(recorder.Sequence.ToCsv());
                }
            }
            else
            {
                if (GUILayout.Button("Start Record"))
                {
                    recorder.StartRecord();
                }
            }
        }

        void SaveTextFile(string text)
        {
            var path = EditorUtility.SaveFilePanel("Save CSV", "", "record.csv", "csv");
            File.WriteAllText(path, text, Encoding.UTF8);
        }
    }
}
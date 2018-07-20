using UnityEngine;
using UnityEditor;


namespace OscCapture
{
    [CustomEditor(typeof(OscPlayer))]
    public class OscPlayerEditor : Editor
    {
        OscPlayer player;

        void OnEnable()
        {
            player = (OscPlayer)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying)
            {
                return;
            }
            // Show on playing only

            if (player.IsPlaying)
            {
                if (GUILayout.Button("Stop"))
                {
                    player.Stop();
                }
            }
            else
            {
                if (GUILayout.Button("Play"))
                {
                    player.Play();
                }
            }
        }
    }
}
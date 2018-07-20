using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UniOSC;

namespace OscCapture
{
    public class Sequence : ScriptableObject
    {
        public struct OscFrame
        {
            public float time;
            public string address;
            public object[] values;

            public OscMessage ToOscMessage()
            {
                var msg = new OscMessage();
                msg.address = address;
                msg.values.AddRange(values);
                return msg;
            }
        }

        List<OscFrame> frames;
        int currentIndex;

        void OnEnable()
        {
            frames = new List<OscFrame>();
            currentIndex = 0;
        }

        void OnDisable()
        {
            frames.Clear();
        }

        public void AddMessage(float time, OscMessage msg)
        {
            // Debug.LogFormat("{0}:{1}", time, msg.ToString());
            frames.Add(new OscFrame()
            {
                time = time,
                address = msg.address,
                values = msg.values.ToArray()
            });
        }

        public OscMessage[] GetFrames(float time)
        {
            var msgs = new List<OscMessage>();

            int i = currentIndex;
            while (i < frames.Count)
            {
                if (frames[i].time > time)
                {
                    break;
                }
                msgs.Add(frames[i].ToOscMessage());
                i++;
            }
            currentIndex = i;

            return msgs.ToArray();
        }

        public string ToCsv()
        {
            var sb = new StringBuilder();
            foreach (var frame in frames)
            {
                sb.Append(frame.time);
                sb.Append(',');
                sb.Append(frame.address);
                foreach (var value in frame.values)
                {
                    sb.Append(',');
                    if (value.GetType() == typeof(float))
                    {
                        sb.AppendFormat("{0:f6}", value);
                    }
                    else
                    {
                        sb.Append(value);
                    }

                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void FromCsv(string csv)
        {
            frames.Clear();
            frames = null;
            frames = CsvToFrames(csv);
        }

        public static List<OscFrame> CsvToFrames(string csv)
        {
            var frames = new List<OscFrame>();

            var lines = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                var cells = line.Split(',');
                if (cells.Length < 2)
                {
                    throw new Exception(string.Format("Could not parse the line: {0}", line));
                }

                // Add new frame
                frames.Add(new OscFrame()
                {
                    time = float.Parse(cells[0]),
                    address = cells[1],
                    values = cells.Skip(2).Select(StringToObject).ToArray()
                });
            }

            return frames;
        }


        static readonly Regex IntRegex = new Regex(@"^[-+]?[0-9]+$");
        static object StringToObject(string str)
        {
            // Int
            if (IntRegex.IsMatch(str))
            {
                return int.Parse(str);
            }
            // Float
            float n;
            if (float.TryParse(str, out n))
            {
                return n;
            }
            // String
            return str as object;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
class ProfileEvent
{
    public string cat;
    public int id;
    public string name;
    public string ph;
    public int pid;
    public int tid;
    public int ts;
    public string Identify {  get { return string.Format("{0}(ts={1})", name, ts); } }
}

class EventParser
{

    [Serializable]
    class Wrapper
    {
        public List<ProfileEvent> array;
    }

    public EventParser(string filename)
    {
        string fileText = File.ReadAllText(filename);
        string jsonText = "{ \"array\": " + fileText + "}";
        Wrapper w = JsonUtility.FromJson<Wrapper>(jsonText);
        Events = w.array;
    }

    public List<ProfileEvent> FindWithName(string name)
    {
        return new List<ProfileEvent>(Events.Where(i => i.name == name));
    }

    public List<string> Validate()
    {
        List<string> errors = new List<string>();
        // make sure everything pops nicely on and off the stack
        Stack<int> stack = new Stack<int>();
        for (int i = 0; i < Events.Count; i++)
        {
            ProfileEvent e = Events[i];
            if (e.ph == "B")
            {
                stack.Push(i);
            }
            if (e.ph == "E")
            {
                if (stack.Count == 0)
                {
                    errors.Add(string.Format("Ending event {0}, but no events are active", e.Identify));
                }
                else
                {
                    int startIndex = stack.Peek();
                    ProfileEvent startEvent = Events[startIndex];
                    if (startEvent.name != e.name)
                    {
                        errors.Add(string.Format("Ending event {0}, but event {1} was on the stack", e.Identify, startEvent.Identify));
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
            }
        }

        if(stack.Count != 0)
        {
            string incompleteEvents = string.Join(", ", new List<int>(stack).ConvertAll(i => Events[i].Identify).ToArray());
            errors.Add(string.Format("The follow events where not ended {0}: ", incompleteEvents));
        }

        return errors;
    }

    public static List<string> Validate(string filename)
    {
        return new EventParser(filename).Validate();
    }

    List<ProfileEvent> Events;

#if UNITY_EDITOR
    [MenuItem("Debug/ValidateTrace")]
    public static void ValidateMenuItem()
    {
        string filename = EditorUtility.OpenFilePanel("Select trace file", string.Empty, string.Empty);
        List<string> errors = Validate(filename);
        if(errors.Count == 0)
        {
            Debug.LogFormat("No Errors found in file {0}", filename);
        }
        else
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} Errors where found in file {1}", errors.Count, filename);
            foreach (string s in errors)
                builder.AppendFormat("\n{0}", s);
            Debug.LogError(builder.ToString());
        }
    }
#endif

}

public class ProfileParser {

    public static 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

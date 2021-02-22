using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;

namespace Helpers
{
    public class Validation : MonoBehaviour
    {
        private static string Name = "Helpers.Validation";
        private static string classFromTypeRegex = ".*\\.(.*)";


        public static void VerifyComponents(GameObject gameObject, params Component[] components)
        {
            Caller caller = GetCaller();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    string componentName = GetNameFromRegex(component);
                    string message = string.Format(Name + ": {0} of GameObject '{1}' is NULL at {2}.{3}", componentName, gameObject.name, caller.callingClass, caller.callingMethod);
                    UnityEngine.Debug.LogError(message);
                }
            }
        }

        public static void VerifyReferences(GameObject gameObject, params Object[] references)
        {
            Caller caller = GetCaller();

            foreach(Object reference in references)
            {
                if (!reference)
                {
                    string referenceName = GetNameFromRegex(reference);
                    string message = string.Format(Name + ": {0} referenced in GameObject '{1}' is NULL at {2}.{3}", referenceName, gameObject.name, caller.callingClass, caller.callingMethod);
                    UnityEngine.Debug.LogError(message);
                }
            }
        }

        private static Caller GetCaller()
        {
            var trace = new StackTrace(2);
            var frame = trace.GetFrame(0);
            var method = frame.GetMethod();

            Caller caller = new Caller(method.DeclaringType.Name, method.Name);

            return caller;
        }

        private static string GetNameFromRegex(object o)
        {
            return Regex.Replace(o.GetType().ToString(), classFromTypeRegex, "$1");
        }
    }

    public class Caller
    {
        public Caller(string callingClass, string callingMethod)
        {
            this.callingClass = callingClass;
            this.callingMethod = callingMethod;
        }
        public string callingClass;
        public string callingMethod;
    }
}
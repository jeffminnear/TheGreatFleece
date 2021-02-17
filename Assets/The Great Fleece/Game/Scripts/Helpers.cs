using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Helpers
{
    public class Validation : MonoBehaviour
    {
        public static void VerifyComponents(GameObject gameObject, params Component[] components)
        {
            var trace = new StackTrace(1);
            var frame = trace.GetFrame(0);
            var caller = frame.GetMethod();
            var callingClass = caller.DeclaringType.Name;
            var callingMethod = caller.Name;

            string classFromTypeRegex = ".*\\.(.*)";

            foreach (Component c in components)
            {
                if (c == null)
                {
                    string componentName = Regex.Replace(c.GetType().ToString(), classFromTypeRegex, "$1");
                    string message = string.Format("{0} of GameObject '{1}' is NULL at {2}.{3}", componentName, gameObject.name, callingClass, callingMethod);
                    UnityEngine.Debug.LogError(message);
                }
            }
        }
    }

}
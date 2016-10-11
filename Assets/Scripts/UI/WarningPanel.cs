using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public static class WarningPanel
    {
        /// <summary>
        /// Played after the warning
        /// </summary>
        public delegate void WarningAction(string option);

        /// <summary>
        /// <para>Warning the specified message, with a return </para>
        /// <para>To call this function for example, if you are warning about overriding a save it must be like this</para>
        /// <para>Warning("Save is not saved, Do you wish to continue", new string[] {"Yes", "No"}, new Action[] {{</para>
        /// <para>	() => { yourFunctionHere(); },</para>
        /// <para>	() => { functionObtionTwo(); }</para>
        /// <para>)</para>
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="options">Options.</param>
        /// <param name="a">The void function that is called when the correction option is chosen</param>
        public static void Warning(string message, string[] options, UnityEngine.Events.UnityAction[] a)
        {
            UIManager.ToggleScreen("Confirmation Screen");
            var cScreen = UIScreen.screenList.Find(g => g.getScreenName() == "Confirmation Screen").getScreen();
            cScreen.transform.FindChild("Text").GetComponent<Text>().text = message;

            Debug.Log(options.Length);
            Debug.Log(a.Length);

            if (options.Length == 2)
            {
                // Change the text
                cScreen.transform.FindChild("Yes").GetComponentInChildren<Text>().text = options[0];
                cScreen.transform.FindChild("No").GetComponentInChildren<Text>().text = options[1];

                // Change the listeners
                var y = cScreen.transform.FindChild("Yes").GetComponentInChildren<Button>().onClick;
                var n = cScreen.transform.FindChild("No").GetComponentInChildren<Button>().onClick;
                y.RemoveAllListeners();
                y.AddListener(() => { UIManager.ToggleScreen("Confirmation Screen"); });
                y.AddListener(a[0]);

                n.RemoveAllListeners();
                n.AddListener(() => { UIManager.ToggleScreen("Confirmation Screen"); });
                n.AddListener(a[1]);

            }
            else
            {
                Debug.LogError("Parameter List != 2, length: " + options.Length);
                throw new Exception("Hello World");
            }

        }

        /// <summary>
        /// Warning the specified message and options with no options to return
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="options">Options</param>
        public static void Warning(string message, string[] options)
        {
            //TODO Complete the warning label
        }
        
    }
}

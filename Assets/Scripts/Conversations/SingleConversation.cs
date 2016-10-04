using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Conversations
{

    /// <summary>
    /// A single conversation, a simple statement said by a single person
    /// </summary>
    public struct singleConversation
    {

        /// <param name="char1_"> The name of the person who is speaking </param>
        /// <param name="char2_"> The name of the person who is spoken to </param>
        /// <param name="conversation_"> The actual conversation</param>
        public singleConversation(string speaker_, string spoken_, string conversation_, bool options_, string[] response_)
        {
            speaker = speaker_;
            spoken = spoken_;
            conversation = conversation_;
            options = options_;
            response = response_;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="singleConversation"/> struct. Do not use the default contructor for the struct
        /// </summary>
        /// <param name="reset">If set to <c>true</c> reset.</param>
        public singleConversation(bool reset)
        {
            speaker = "";
            spoken = "";
            conversation = "";
            options = false;
            response = new string[4];
        }

        public string speaker, spoken;
        public string conversation;
        public bool options;
        public string[] response;

        public void print()
        {
            if (!Settings.debugCONVO)
                return;

            if (speaker == "")
                Debug.Log("CONVERSATION EMPTY");
            else Debug.Log("CONVERSATION| " + speaker + " - " + spoken + ": " + conversation);
        }
    }
}
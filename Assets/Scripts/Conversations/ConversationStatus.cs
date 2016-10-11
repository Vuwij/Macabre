using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conversations
{
    public enum ConversationStatus
    {
        Start = 'S',
        CharacterResponse = 'R',
        PlayerClosedResponse = 'C',
        PlayerOpenResponse = 'O',
        EndConversation = 'E'
    };
}

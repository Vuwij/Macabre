using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Objects.Movable.Characters
{
    // What the current conversation status is, what you should see on the screen
    public enum ConversationViewStatus
    {
        // Nothing appears on the screen, the conversation has ended or hasn't began yet
        Empty = 'E',

        // The character makes a response to the player or another character's conversation
        CharacterResponse = 'C',

        // The player makes a response to the character, with an automatic option to continue
        PlayerResponse = 'P',

        // The player has 1-4 options to choose from
        PlayerMultipleReponse = 'M',
    };
}

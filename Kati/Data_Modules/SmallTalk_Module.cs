using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Models{
    /*
         Conversation tones based on stats: both speaker and responder
       Type          |   required high       |      optional/mid      |      required lower
       ------------------------------------------------------------------------------------------
       -strangers    |    -none              |       -none            |       -all
       -neutral      |    -*                 |       -*               |       -*
       -friends      |    -friendship        |       -prof/respect    |       -all others
       -Lovers       |    -Romance           |       -all positive    |       -all negative       
       -Professional |    -prof/respect      |       -friends/rivalry |       -disgust/hatred
       -crush        |    -admiration        |       -all pos         |       -all neg
       -annoying     |    -disgust           |       -all neg         |       -Rom,Admiration,Friend
       -enemy        |    -hatred            |       -all neg         |       -all pos
       -rival        |    -rivalry           |       -all             |       -none
       --------------------------------------------------------------------------------------------
       -Neutral happens when none of the others are true.
       -other tones may use the neutral category
       --------------------------------------------------------------------------------------------
    */
    class SmallTalk_Module
    {
        //can talk to itself
        //topics questions or statements (invoke a response or not)
        /**question, statement, response
         * Weather: (relies on game input (what is the weather?))
         * Current Events: (relies on game input (what is currently going on?))
         * Social Events (relies on game input (where is the character?))
         * Observation about person: (relies on game input (who are they talking to and what do they have with them))
         * greetings and How are you questions 
         * **/

        /** SmallTalk Requirement Keys 
         *  weather.[nice_day, rain, hot, windy, cold, humid]
         *  
         * **/

        /** SmallTalk Leads To Keys format
         *  Module_name.section_in_module --> [small_talk.weather_response]
         *  
         * **/

        //Tone Type: conversation phrase : [tags about responses or leads to] 
        public readonly Dictionary<string, Dictionary<string, List<string>>> weatherQuestion;
        public readonly Dictionary<string, Dictionary<string, List<string>>> weatherStatement;
        public readonly Dictionary<string, Dictionary<string, List<string>>> weatherResponse;
        public readonly Dictionary<string, Dictionary<string, List<string>>> eventQuestion;
        public readonly Dictionary<string, Dictionary<string, List<string>>> eventStatement;
        public readonly Dictionary<string, Dictionary<string, List<string>>> eventResponse;
        public readonly Dictionary<string, Dictionary<string, List<string>>> greetingStatement;
        public readonly Dictionary<string, Dictionary<string, List<string>>> greetingQuestion;
        public readonly Dictionary<string, Dictionary<string, List<string>>> greetingResponse;

    }
}

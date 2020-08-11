using Kati.Data_Modules.GlobalClasses;
using Kati.SourceFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Modules.DayDreamWonder {

    public class DayDreamWonder_Controller: Controller {

        public DayDreamWonder_Controller()
            :base(Constants.DayDreamWonder) {
            
        }

        //Inherit from Controller
        //ref to ModuleLib --> super
        //talk to game  --> super
        //decide which topic to discuss --> abstract out with polymorphism
        //special rules and overridden logic
        //decide on which topic type --> abstract out with polymorphism
        //special rules and overriden logiv
        //talk to parser

    }

    public class DayDreamWonder_Parser {

        public DayDreamWonder_Parser() { 
            //Inherit from Parser
            //talk with controller
            //decide topic branch --> abstract out with polymorphism
            //parse req -->abstract out with polymorphism
            //decide on best dialogue for statements and questions

        }
    }

}

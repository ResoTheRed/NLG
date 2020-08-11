using Kati.Data_Models;
using Kati;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kati.SourceFiles;

namespace KatiUnitTest.Module_Tests{

    [TestClass]
    public class SmallTalkModuleTester {
        private SmallTalk_Module module;
        private SmallTalk_Loader loader;

        [TestInitialize]
        public void Start() {
            module = new SmallTalk_Module("C:/Users/User/Documents/NLG/KatiUnitTest/Module_Tests/SmallTalk/smallTalk.json");
            loader = SmallTalk_Loader.LoadFromFile(module);
        }

        [TestMethod]
        public void TestSmallTalkModuleConstructor() {
            module = new SmallTalk_Module(Constants.smallTalk);
            Assert.IsNotNull(module);
        }

        [TestMethod]
        public void TestSmallTalkLoaderConstructor() {
            loader = SmallTalk_Loader.LoadFromFile(module);
            Assert.IsNotNull(loader);
        }

        [TestMethod]
        public void TestWeatherStatements() {
            //"neutral": {"message"  {"reg" : [], "leads to": [] }}
            Assert.IsNotNull(module.weatherStatement["neutral"]);
            Assert.IsNotNull(module.weatherStatement["neutral"]
                ["It is a wonderful day. The sky is so clear."]);
            Assert.IsNotNull(module.weatherStatement["neutral"]
                ["It is a wonderful day. The sky is so clear."]
                ["req"]);
            Assert.IsTrue(module.weatherStatement["neutral"]
                ["It is a wonderful day. The sky is so clear."]
                ["req"][0].Equals("weather.nice_day"));
            Assert.IsNotNull(module.weatherStatement["disgust"]
                ["It's hot."]["leads to"]);
            Assert.IsTrue(module.weatherStatement["disgust"]
                ["It's hot."]["req"][0].Equals("weather.hot"));
        }

        [TestMethod]
        public void TestWeatherQuestions() {
            Assert.IsNotNull(module.weatherQuestion["neutral"]
                ["So, how about this weather?"]["leads to"]);
            Assert.IsTrue(module.weatherQuestion["neutral"]
                ["So, how about this weather?"]["leads to"]
                [0].Equals("small_talk.weather_response"));
            Assert.IsNotNull(module.weatherQuestion["neutral"]
                ["It's so nice today.  What do you think?"]["req"]);
            Assert.IsTrue(module.weatherQuestion["neutral"]
                ["It's so nice today.  What do you think?"]["req"]
                [0].Equals("weather.nice_day"));
        }

        [TestMethod]
        public void TestWeatherResponse() { 
            Assert.IsNotNull(module.weatherResponse["neutral"]
                ["It sure is a nice day."]["req"]);
            Assert.IsTrue(module.weatherResponse["neutral"]
                ["It sure is a nice day."]["req"]
                [0].Equals("weather.nice_day"));
            Assert.IsTrue(module.weatherResponse["neutral"]
                ["There is a chill in the air."]["req"]
                [0].Equals("weather.cold"));
        }

        [TestMethod]
        public void TestEventStatement() {
            Assert.IsTrue(module.eventStatement["neutral"]
                ["I'm so excited about #current_event#. There is so much to do and see."]
                ["req"][1].Equals("npc.#loves_current_event#"));
            Assert.IsTrue(module.eventStatement["neutral"]
                ["#current_event# will be here soon.  I find it to be a bit boring myself."]
                ["req"].Count==2);
        }

        [TestMethod]
        public void TestEventQuestion() {
            Assert.IsTrue(module.eventQuestion["stranger"]
                ["Do you like #current_event#?"]["req"].Count==1);
            Assert.IsTrue(module.eventQuestion["neutral"]
                ["I always hate going to the #current_event#, what do you think about it?"]
                ["req"][0].Equals("npc.#hates_current_event#"));
        }

        [TestMethod]
        public void TestEventResponse() {
           Assert.IsTrue(module.eventResponse["neutral"]
                ["The #current_event# is okay. I enjoy it well enough."]
                ["leads to"].Count==0);
        }

        [TestMethod]
        public void TestGreetingStatement() {
            Assert.IsTrue(module.greetingStatement["stranger"]
                ["Good morning"]["req"][0]
                .Equals("time.day.morning"));
            Assert.IsTrue(module.greetingStatement["neutral"]
                ["Hello #player.name#."]
                ["req"].Count==0);
        }

        [TestMethod]
        public void TestGreetingQuestion() {
            Assert.IsTrue(module.greetingQuestion["neutral"]
                ["How are things?"]["leads to"][0]
                .Equals("greeting_response"));
            Assert.IsTrue(module.greetingQuestion["friend"]
                ["How goes it?"]["req"].Count==0);
        }

    }

   
}

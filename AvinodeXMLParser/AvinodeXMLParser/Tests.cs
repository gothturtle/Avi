using System;

namespace AvinodeXMLParser
{
    public class Tests
    {
        public Tests()
        {

        }

        public string[] GetArgs(int testNumber = 3)
        {
            string[] args = new string[2];

            switch(testNumber)
            {
                case 1:
                    args[0] = @"..\..\XMLMenuTestFiles\SchedAero Menu.xml";
                    //args[1] = "/default.aspx";
                    args[1] = "/Requests/OpenQuotes.aspx";
                    break;

                case 2:
                    args[0] = @"..\..\XMLMenuTestFiles\Wyvern Menu.xml";
                    args[1] = "/TWR/Directory.aspx";
                    break;

                case 3:
                    args[0] = @"..\..\XMLMenuTestFiles\SchedAero MenuNested.xml";
                    //args[1] = "/Requests/Trips/ScheduledTrips1.aspx";
                    args[1] = "/Requests/Trips/ScheduledTrips25.aspx";
                    break;
            }

            return args;
        }
    }
}
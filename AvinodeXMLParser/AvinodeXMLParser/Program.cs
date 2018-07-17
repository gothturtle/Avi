using System;
using System.IO;
using System.Xml;
using static System.Console;

namespace AvinodeXMLParser
{
    /// <summary>
    ///     Accept two arguments: 
    ///         first, a path to a menu .xml file (e.g. “c:\schedaeromenu.xml”); 
    ///         second an active path to match (e.g. “/default.aspx”) 
    ///     
    ///     Parse the XML document, ignoring any XML content not required for this application 
    /// 
    ///     Identify currently-active menu items — a menu item is active if it or one of its children has a path matching the second argumen
    /// 
    /// Write the parsed menu to the console 
    /// 1. Show display name and path structure for each menu item 
    /// 2. Indent submenu items 
    /// 3. Print the word “ACTIVE” next to any active menu items. 
    /// 
    /// When we run your application from the command line, we should see something like this: 
    /// 
    ///c:> MenuSample.exe “c:\schedAeroMenu.xml” “/Requests/OpenQuotes.aspx” 
    ///Home,    /Default.aspx 
    ///Trips,   /Requests/Quotes/CreateQuote.aspx ACTIVE         
    ///         Create Quote/Trip, /Requests/Quotes/CreateQuote.aspx 
    ///         Open Quotes, /Requests/OpenQuotes.aspx ACTIVE         
    ///         Scheduled Trips, /Requests/Trips/ScheduledTrips.aspx 
    ///Company, /mvc/company/view 
    ///         Customers, /customers/customers.aspx 
    ///         Pilots, /pilots/pilots.aspx 
    ///         Aircraft, /aircraft/Aircraft.aspx
    ///         
    /// Please supply a Visual Studio project and associated files containing your solution.
    /// 
    /// </summary>
    class Program
    {
        private static string userMessage = string.Empty;
        private static int currentIndent = 0;
        private static bool doTests = false;

        /// <summary>
        /// My approach here was to use XPath instead of de-serializing xml.
        /// Since we could be running xml files from different sources there may be extra tags or attributes we need to ignore.   
        /// While we could set [Ignore] attributes in our defined C# objects, this would likely be error prone and a maintanence headache.
        /// This approach allows for infinite nesting of items 
        /// and happily ignores extraneous tags ...
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            #region Testing

            if(doTests)
            {
                Tests t = new Tests();
                args = t.GetArgs(3);
            }

            #endregion

            if (args.Length != 2)
            {              
                userMessage = @"This application requires two arguments:
                first, a path to a menu .xml file (e.g. “c:\schedaeromenu.xml”)
                second an active path to match (e.g. “/default.aspx”) 
                Example:
                c:> AvinodeXMLParser.exe “c:\schedAeroMenu.xml” “/Requests/OpenQuotes.aspx”
                ";
            }
            else if(ValidateFilePath(args[0]) == false)
            {
                userMessage = @"The application failed to find the xml file with the given path.";
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(args[0]);
                    XmlNodeList itemNodes = xmlDoc.SelectNodes("/menu/item");// get top level nodes
                    foreach (XmlNode itemNode in itemNodes)
                    {
                        currentIndent = 0;// reset indent
                        ShowNode(itemNode, args[1]);
                    }
                }
                catch(Exception ex)
                {
                    userMessage = @"The application failed to load the xml file.";
                    userMessage += ex.Message;
                }
            }

            WriteLine(userMessage);
            WriteLine("[Hit any key to exit]");
            ReadKey();
        }

        private static bool HasActiveChild(XmlNode itemNode, string activePath)
        {
            bool active = false;

            //  XPath Examples
            //  https://msdn.microsoft.com/en-us/library/ms256086(v=vs.110).aspx

            XmlNodeList nodeList = itemNode.SelectNodes("descendant::item[path]");
            foreach (XmlNode item in nodeList)
            {
                XmlNode pathNode = item.SelectSingleNode("path");
                string path = pathNode.Attributes["value"].InnerText;
                if(path == activePath)
                {
                    active = true;
                    break;
                }
            }
            return active;
        }

        private static void ShowNode(XmlNode itemNode, string activePath)
        {
            XmlNode displayNameNode = itemNode.SelectSingleNode("displayName");
            XmlNode pathNode = itemNode.SelectSingleNode("path");
            XmlNode submenuNode = itemNode.SelectSingleNode("subMenu");

            string path = pathNode.Attributes["value"].InnerText;
            string name = displayNameNode.InnerText;
            bool active = (path == activePath) ? true : false;

            // ===== if has children and is not active, check if any children are active =====
            if (submenuNode != null && active == false)
                if (HasActiveChild(itemNode, activePath)) active = true;

            PrintLine(name, path, active ? "ACTIVE" : "");

            if(submenuNode != null)
            {
                currentIndent += 5;
                foreach (XmlNode item in submenuNode.ChildNodes)
                    ShowNode(item, activePath);
                currentIndent -= 5;
            }
        }

        private static void PrintLine(string name, string path, string active)
        {
            WriteLine(new string(' ', currentIndent) + name + ",     " + path + "     " + active);
        }

        private static bool ValidateFilePath(string path)
        {
            try { return new FileInfo(path).Exists; }
            catch { }
            return false;
        }
    }
}
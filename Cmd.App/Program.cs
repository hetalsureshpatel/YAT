using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Cmd.App
{
    public static class Extenions
    {
        /// <summary>
        /// Returns a sequence of <see cref="XElement">XElements</see> corresponding to the currently
        /// positioned element and all following sibling elements which match the specified name.
        /// </summary>
        /// <param name="reader">The xml reader positioned at the desired hierarchy level.</param>
        /// <param name="elementName">An <see cref="XName"/> representing the name of the desired element.</param>
        /// <returns>A sequence of <see cref="XElement">XElements</see>.</returns>
        /// <remarks>At the end of the sequence, the reader will be positioned on the end tag of the parent element.</remarks>
        public static IEnumerable<XElement> ReadElements(this XmlReader reader, XName elementName)
        {
            if (reader.Name == elementName.LocalName && reader.NamespaceURI == elementName.NamespaceName)
                yield return (XElement)XNode.ReadFrom(reader);

            while (reader.ReadToNextSibling(elementName.LocalName, elementName.NamespaceName))
            {
                yield return (XElement)XNode.ReadFrom(reader);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //LargeXmlReadingTest();

            XmlDemandewareTest(@"D:\Development\Virtual.Rail\Demandware\master-catalog_2013-10-27 19-00-01.xml");
            //XmlDressipiTest(@"D:\Development\UAT\Console\m_and_s-201402210911.xml");

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.ReadKey();
        }

        private static void XmlDemandewareTest(string path)
        {
            XNamespace xms = "http://www.demandware.com/xml/impex/catalog/2006-10-31";

            using (XmlReader reader = XmlReader.Create(path, new XmlReaderSettings { ProhibitDtd = false }))
            {
                // move the reader to the start of the content and read the root element's start tag
                //   that is, the reader is positioned at the first child of the root element
                reader.MoveToContent();
                reader.ReadStartElement("catalog");

                foreach (XElement etext in reader.ReadElements(xms + "product"))
                {
                    string id = (string)etext.Attribute("product-id");
                    string title = (string)etext.Element(xms + "display-name");

                    //Console.WriteLine("{0}: {1}", id, title);
                }
            }
        }
    }
}

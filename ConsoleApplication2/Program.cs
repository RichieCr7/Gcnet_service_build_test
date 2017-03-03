using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {

            //string base64String = "UEsDBBQAAAAIAK1maUkVEKRlaQEAAGECAAAeAAAAdmVybWFzXzAwNjgwOV8wOTExMTYxMjUzMjcu ZWRpdZLNbqwwDIVfhV0Wlio7/MY7QzIwGhIogWnV93+Qa5jb3s1tpCziyMfnO/KRejjSImwBbQd7 krQ/yAI1ROiYbF1CjohNh84caQLUQ/AMW5TMnqkRPhLn6EdC048R2soBOiJqvvvAGb9HoLJliy/h U1cfpUni4WOVa/gSwyhFlO2eQjFOkqSYdw/rW7G8Ff3yWQyhIEtdDXuIAgDjZMK7hyHBHIcDO9W2 Dip7kH7WZrvdoE/cy2wddkhXId95UATbNZWtPs28DOBUSBUhrpl3BbsnmTOX2JgcZrhRWVnrKsiT iUFAJMBTOR9jZHJdgxdc67oLDh22eqsLbvc7WARQO8ysyW7y+AdW/KXIWj5yvtw9l8RYtrXx6itP K+uk0wkINQ/VtR3WrnFXbHkdtDudElcUZ02iGXaBbQVO8hKJ9C3yEzb8FvXl6GXrP6nrjFO8T8Dx PkwS5kLW4BfdC+Wk12ro40uBf3bmD1BLAQIUABQAAAAIAK1maUkVEKRlaQEAAGECAAAeAAAAAAAA AAAAAAAAAAAAAAB2ZXJtYXNfMDA2ODA5XzA5MTExNjEyNTMyNy5lZGlQSwUGAAAAAAEAAQBMAAAA pQEAAAAA";
            //byte[] zipBytes = Convert.FromBase64String(base64String);
            //using (var zipStream = new MemoryStream(zipBytes))
            //using (var zipArchive = new ZipArchive(zipStream))
            //{
            //    zipArchive.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory));
            //    var entry = zipArchive.Entries.Single();
            //    string mimeType = MimeMapping.GetMimeMapping(entry.Name);
            //    using (var decompressedStream = entry.Open())
            //    {

            //    }

            //}

       //<wsdl:retrieveManifest soapenv:encodingStyle = "http://schemas.xmlsoap.org/soap/encoding/">
       //<string xsi:type="xsd:string" xs:type="type:string" xmlns:xs="http://www.w3.org/2000/XMLSchema-instance" ><![CDATA[<retrieveManifest>




            var trans = DateTime.Now.ToString("yyyyMMddhhmmss");
            var to_Date = DateTime.Now.ToString("yyyyMMddhhmmss");

            var path = Build_Manifest_Request_Body("gphaws" , "5810ad58dd3229bfacbfcd9823b03e4438dc6a71" , "20161005000001" , trans , to_Date);
            var response = call(path);
            //var xml_path = Build_EDO_Request_Body("gphaws" , "5810ad58dd3229bfacbfcd9823b03e4438dc6a71" , "20161005000001" , trans , to_Date);
            //var response = call(xml_path);



            //var trans = DateTime.Now.ToString("yyyyMMddhhmmss");
            //var to_Date = DateTime.Now.ToString("yyyyMMddhhmmss");
            //var bBash_xml = Build_HASH_XML_Request_Body("gcnttest" , "password00" , trans);
            //var hash = SERVICE_HASH_REQUEST(bBash_xml);
            //var xml_path = Build_EDO_Request_Body("gcnttest" , hash , trans , trans , to_Date);
            //var response = call(xml_path);
            Console.ReadLine();

        }

        private static bool AcceptAllCertifications(object sender , X509Certificate certificate , X509Chain chain , SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static HttpWebRequest CreateSOAPWebRequest()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://trsvr.gcnetghana.com/giccswebservice/giccsws/wsdl?WSDL"); 
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
            httpWebRequest.Headers.Add("SOAP:Action");
            httpWebRequest.ContentType = "text/xml;charset=\"utf-8\"";
            httpWebRequest.Accept = "text/xml";
            httpWebRequest.Method = "POST";
            return httpWebRequest;
        }

        public static string SERVICE_HASH_REQUEST(string hash_xml_doc_path)
        {
            string result;
            try
            {
                HttpWebRequest httpWebRequest = CreateSOAPWebRequest();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(hash_xml_doc_path);
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    xmlDocument.Save(requestStream);
                }
                using (WebResponse response = httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string xml = streamReader.ReadToEnd();
                        XmlDocument xmlDocument2 = new XmlDocument();
                        xmlDocument2.LoadXml(xml);
                        XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument2.NameTable);
                        xmlNamespaceManager.AddNamespace("m" , "http://giccs.gcnet.com/wsdl");
                        XmlNodeList xmlNodeList = xmlDocument2.SelectNodes("//m:retrieveHashingDataResponse" , xmlNamespaceManager);
                        int count = xmlNodeList.Count;
                        string s = null;
                        foreach (XmlNode xmlNode in xmlNodeList)
                        {
                            s = xmlNode["m:return"].InnerText;
                        }
                        List<string> list = new List<string>();
                        using (System.IO.StringReader stringReader = new System.IO.StringReader(s))
                        {
                            string item;
                            while ((item = stringReader.ReadLine()) != null)
                            {
                                bool flag = list.Count != 4;
                                if (!flag)
                                {
                                    break;
                                }
                                list.Add(item);
                            }
                        }
                        string text = list.Last();
                        string[] array = text.Split(new char[]
                        {
                            ':'
                        });
                        result = array[1].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public static string Build_HASH_XML_Request_Body(string username , string password , string transactionDateTime)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace ns2 = "http://giccs.gcnet.com/wsdl";
            XNamespace xNamespace = "http://www.w3.org/2001/XMLSchema-instance";
            XElement xElement = new XElement(ns + "Envelope" , new object[]
            {
                new XAttribute(XNamespace.Xmlns + "wsdl", "http://giccs.gcnet.com/wsdl"),
                new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XElement(ns + "Header"),
                new XElement(ns + "Body", new XElement(ns2 + "retrieveHashingData", new XElement(ns2 + "xmlStr", string.Concat(new string[]
                {
                    "userId:",
                    username,
                    ";password:",
                    password,
                    ";transactionDateTime:",
                    transactionDateTime,
                    ";"
                }))))
            });
            Directory.CreateDirectory("SOAP_HASH_REQUEST");
            DirectoryInfo directoryInfo = new DirectoryInfo("SOAP_HASH_REQUEST");
            string fileName = Path.Combine(directoryInfo.FullName , "hash_request.xml");
            xElement.Save(fileName);
            return Path.Combine(Environment.CurrentDirectory , "SOAP_HASH_REQUEST\\hash_request.xml");
        }

        public static string Build_EDO_Request_Body(string userId , string hash , string transactionDateTime , string frmDate , string toDate)
        {

            XmlDocument xmlDocument = new XmlDocument();
            XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace ns2 = "http://giccs.gcnet.com/wsdl";
            XNamespace xNamespace = "http://www.w3.org/2001/XMLSchema-instance";
            XElement xElement = new XElement(ns + "Envelope" , new object[]
            {
                new XAttribute(XNamespace.Xmlns + "wsdl", "http://giccs.gcnet.com/wsdl"),
                new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XElement(ns + "Header"),
                new XElement(ns + "Body", new XElement(ns2 + "retrieveDeliveryOrder", new object[]
                {
                    new XElement(ns2 + "xmlStr", new XCData(Convert.ToString(new XElement("retrieveDeliveryOrder", new object[]
                    {
                        new XElement("userId", userId),
                        new XElement("password", hash),
                        new XElement("transactionDateTime", transactionDateTime),
                        new XElement("fromDate", frmDate),
                         new XElement("toDate", toDate)
                    }))))
                }))
            });
            Directory.CreateDirectory("SOAP_EDO_REQUEST");
            DirectoryInfo directoryInfo = new DirectoryInfo("SOAP_EDO_REQUEST");
            string fileName2 = Path.Combine(directoryInfo.FullName , "EDO_request.xml");
            xElement.Save(fileName2);
            return Path.Combine(Environment.CurrentDirectory , "SOAP_EDO_REQUEST\\EDO_request.xml");
        }

        public static string Build_Manifest_Request_Body(string userId , string hash , string transactionDateTime , string frmDate , string toDate)
        {

           
            XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/"; 
            XNamespace ns2 = "http://giccs.gcnet.com/wsdl";
            XNamespace xNamespace = "http://www.w3.org/2000/XMLSchema-instance";
            XNamespace sop1 = "soapenv";
            XNamespace sop2 = "xsi";
            XNamespace sop3 = "xs";

            XElement xElement = new XElement(ns + "Envelope" , new object[]
            {
                new XAttribute(XNamespace.Xmlns + "wsdl", "http://giccs.gcnet.com/wsdl"),
                 new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                 new XAttribute(XNamespace.Xmlns + "xsd","http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"), 
                new XElement(ns + "Header"),
                new XElement(ns + "Body", new XElement(ns2 + "retrieveManifest", new XAttribute(sop1
                + "encodingStyle", "http://schemas.xmlsoap.org/soap/encoding/"), new object[]
                {
                    new XElement("string",new XAttribute(XNamespace.Xmlns + "xs", xNamespace),new XAttribute(sop3 + "type","type:string"),new XAttribute(sop2 + "type","xsd:string"),new XCData(new XElement("retrieveManifest", new object[]
                    {
                        new XElement("userId", userId),
                        new XElement("password", hash),
                        new XElement("transactionDateTime", transactionDateTime),
                        new XElement("rotationNo",""),
                        new XElement("customsOffice","PL01"),
                        new XElement("fromDate", frmDate),
                         new XElement("toDate", toDate)
                    }).ToString()))
                }))
            });

           
            Directory.CreateDirectory("SOAP_MANIFEST_REQUEST");
            DirectoryInfo directoryInfo = new DirectoryInfo("SOAP_MANIFEST_REQUEST");
            string fileName2 = Path.Combine(directoryInfo.FullName , "MANIFEST_request.xml");
            xElement.Save(fileName2);
            return Path.Combine(Environment.CurrentDirectory , "SOAP_MANIFEST_REQUEST\\MANIFEST_request.xml");
        }

        public static bool call(string Path_)
        {
            try
            {
                HttpWebRequest httpWebRequest = CreateSOAPWebRequest();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Path_);
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    xmlDocument.Save(requestStream);
                }
                using (WebResponse response = httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        try
                        {
                            string text = streamReader.ReadToEnd();
                            Directory.CreateDirectory("SOAP_EDO_RESPONSE");
                            DirectoryInfo directoryInfo = new DirectoryInfo("SOAP_EDO_RESPONSE");
                            var xml_response_file_path = Path.Combine(directoryInfo.FullName , "soap_eDO_response" + ".xml");
                            System.IO.File.WriteAllText(xml_response_file_path , text);
                            Console.WriteLine(text);
                            //XDocument xDocument = null;
                            //using (System.IO.StringReader stringReader = new System.IO.StringReader(text))
                            //{
                            //    var xmlReader = XmlReader.Create(stringReader);

                            //    xmlReader.ReadToDescendant("m:return");
                            //    xmlReader.Read();
                            //    xDocument = XDocument.Parse(xmlReader.Value);

                            //    string value = xDocument.Descendants("transactionMessage").First<XElement>().Value;
                              
                            //}
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

    }
           
}

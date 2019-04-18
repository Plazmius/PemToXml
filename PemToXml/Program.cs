using System;
using System.IO;
using System.Security.Cryptography;
using NDesk.Options;

namespace PemToXml
{
    class Program
    {
        static void Main(string[] args)
        {
            var argParser = new ArgumentParser();

            ConvertOptions options;
            try
            {
                options = argParser.GetConvertOptionsFromArgs(args);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Usage:");
                Console.WriteLine(argParser.GetHelpString());
                return;
            }

            var xmlString = new RsaConverter().PemToXmlString(options.PemFilePath);

            using (var xmlFile = File.CreateText(options.XmlSavePath))
            {
                xmlFile.WriteAsync(xmlString);
            }
        }
    }
}

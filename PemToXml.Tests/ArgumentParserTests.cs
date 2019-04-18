using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PemToXml.Tests
{
    [TestClass]
    public class ArgumentParsesTests
    {
        private ArgumentParser argParser;

        [TestInitialize]
        public void Initialize()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"D:\key.pem", new MockFileData("key")},
                {@"D:\key.xml", new MockFileData("<node>key<node>")}
            });
            argParser = new ArgumentParser(fs);
        }

        [TestMethod]
        public void GetConvertOptionsFromArgs_WhenArgsIsEmptyArray_ShouldThrowArgumentException()
        {
            var args = new string[]{};

            var convertAction = new Action(() =>
            {
                var options = argParser.GetConvertOptionsFromArgs(args);
            });

            Assert.ThrowsException<ArgumentException>(convertAction);
        }

        [TestMethod]
        public void GetConvertOptionsFromArgs_WhenPemPathNotProvided_ShouldThrowArgumentException()
        {
            var args = new[] { "-some-incorrent-arg1=someValue","-some-incorrent-arg2=someValue2" };

            var convertAction = new Action(() =>
            {
                var options = argParser.GetConvertOptionsFromArgs(args);
            });

            Assert.ThrowsException<ArgumentException>(convertAction);
        }

        [TestMethod]
        public void GetConvertOptionsFromArgs_WhenIncorrectPathsPassed_ShouldThrowArgumentException()
        {
            var args = new[] { @"-p=someValue|<>", @"-x=someValue2|<>" };

            var convertAction = new Action(() =>
            {
                var options = argParser.GetConvertOptionsFromArgs(args);
            });

            Assert.ThrowsException<ArgumentException>(convertAction);
        }



        [TestMethod]
        public void GetConvertOptionsFromArgs_WhenNotExistingPemFilePassed_ShouldThrowArgumentException()
        {
            var args = new[] { @"-p=D:\not-exist.pem", @"-x=D:\xml.xml" };

            var convertAction = new Action(() =>
            {
                var options = argParser.GetConvertOptionsFromArgs(args);
            });

            Assert.ThrowsException<ArgumentException>(convertAction);
        }

        [TestMethod]
        public void GetConvertOptionsFromArgs_WhenXmlPathNotProvided_ShouldReturnSamePathAsPemWithXmlExtension()
        {
            var args = new[] { @"-p=D:\key.pem" };

            var options = argParser.GetConvertOptionsFromArgs(args);

            Assert.AreEqual(options.PemFilePath, @"D:\key.pem");
            Assert.AreEqual(options.XmlSavePath, @"D:\key.xml");
        }

        [TestMethod]
        public void GetConvertOptionsFromArgs_WhenBothPathsProvided_ShouldReturnOptionsObjectWithThesePaths()
        {
            var args = new[] { @"-p=D:\key.pem", @"-x=D:\key.xml" };

            var options = argParser.GetConvertOptionsFromArgs(args);

            Assert.AreEqual(@"D:\key.pem",options.PemFilePath);
            Assert.AreEqual(@"D:\key.xml", options.XmlSavePath);
        }

        [TestMethod]
        public void GetHelpString_WhenCalled_ReturnNotEmptyString()
        {
            var help = argParser.GetHelpString();

            Assert.IsFalse(string.IsNullOrEmpty(help));
        }
    }
}

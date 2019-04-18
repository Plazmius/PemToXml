using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using NDesk.Options;

namespace PemToXml
{
    public class ArgumentParser
    {
        private readonly IFileSystem _fileSystem;
        public static string[] PemFileArgKeys = {"p", "pem-file"};
        public static string[] XmlOutFileArgKeys = {"x", "xml-out-file"};
        

        private ConvertOptions _options = new ConvertOptions();
        private readonly OptionSet _parser;

        public ArgumentParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _parser = new OptionSet
            {
                {string.Concat(string.Join('|',PemFileArgKeys), "="), value =>
                {
                    ValidatePath(value);
                    ValidateFileExist(value);
                    _options.PemFilePath = value;
                }},
                {string.Concat(string.Join('|', XmlOutFileArgKeys), "="), value =>
                {
                    ValidatePath(value);
                    _options.XmlSavePath = value;
                }}
            };
        }

        private void ValidateFileExist(string path)
        {
            if (!_fileSystem.File.Exists(path))
            {
                throw new ArgumentException($"File with path {path} do not exist");
            }
        }

        private void ValidatePath(string path)
        {
            //throws argument exception if path contains invalid chars
            if (_fileSystem.Path.GetInvalidPathChars().Any(path.Contains))
            {
                throw new ArgumentException($"Invalid path specified: {path}");
            }
        }

        public ArgumentParser() : this(new FileSystem()) { }

        public ConvertOptions GetConvertOptionsFromArgs(string[] args)
        {
            _options = new ConvertOptions(){PemFilePath = null, XmlSavePath = null};
            ValidateArgsProvided(args);
            _parser.Parse(args);
            ValidatePemPathProvided();
            ValidateXmlPathSet();

            return _options;
        }

        private static void ValidateArgsProvided(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if (args.Length == 0)
            {
                throw new ArgumentException("No arguments provided", nameof(args));
            }
        }

        private void ValidateXmlPathSet()
        {
            if (string.IsNullOrEmpty(_options.XmlSavePath))
            {
                _options.XmlSavePath = _fileSystem.Path.GetDirectoryName(_options.PemFilePath)
                                       + _fileSystem.Path.GetFileNameWithoutExtension(_options.PemFilePath) 
                                       + ".xml";
            }
        }

        private void ValidatePemPathProvided()
        {
            if (string.IsNullOrEmpty(_options.PemFilePath))
            {
                throw new ArgumentException("No path provided for pem file");
            }
        }

        public string GetHelpString()
        {
            using (var writer = new StringWriter())
            {
                _parser.WriteOptionDescriptions(writer);
                return writer.ToString();
            }
        }
    }
}
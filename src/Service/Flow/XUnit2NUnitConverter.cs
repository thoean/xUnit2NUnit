using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Service.Contracts;
using Service.Data;

namespace Service.Flow
{
    public class XUnit2NUnitConverter : IConversionScheduler
    {
        private readonly ITargetBlock<Tuple<Guid, XUnitData>> targetBlock;
        
        public XUnit2NUnitConverter(IConversionRepository repository)
        {
            // cache the xslt to transform from xUnit to nUnit
            var transform = new XslCompiledTransform();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Startup), "Resources.NUnitXml.xslt"))
            {
                using (var xmlReader = new XmlTextReader(stream))
                {
                    transform.Load(xmlReader);
                }
            }

            // save the request in the database
            var saveRequest = new TransformBlock<Tuple<Guid, XUnitData>, Tuple<Guid, XUnitData>>(input =>
            {
                try
                {
                    repository.AddRequest(input.Item1, input.Item2);
                    return input;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return input;
                }
            });

            // convert the input string to an XPathDocument
            var readXml = new TransformBlock<Tuple<Guid, XUnitData>, Tuple<Guid, XPathDocument>>(input =>
            {
                try
                {
                    using (var reader = new StringReader(input.Item2.XUnitResult))
                    {
                        var doc = new XPathDocument(reader);
                        return new Tuple<Guid, XPathDocument>(input.Item1, doc);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return new Tuple<Guid, XPathDocument>(input.Item1, null);
                }
            });

            // perform the actual conversion
            var convertXml = new TransformBlock<Tuple<Guid, XPathDocument>, Tuple<Guid, string>>(input =>
            {
                try
                {
                    using (var stringWriter = new StringWriter())
                    {
                        using (var writer = new XmlTextWriter(stringWriter))
                        {
                            transform.Transform(input.Item2, null, writer);
                        }
                        return new Tuple<Guid, string>(input.Item1, stringWriter.GetStringBuilder().ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return new Tuple<Guid, string>(input.Item1, null);
                }
            });

            // convert to output
            var createOutput = new TransformBlock<Tuple<Guid, string>, Tuple<Guid, NUnitData>>(input => new Tuple<Guid, NUnitData>(input.Item1, new NUnitData {NUnitResult = input.Item2}));
            var saveResult = new ActionBlock<Tuple<Guid, NUnitData>>(input => repository.AddResult(input.Item1, input.Item2));

            saveRequest.LinkTo(readXml);
            readXml.LinkTo(convertXml);
            convertXml.LinkTo(createOutput);
            createOutput.LinkTo(saveResult);
            targetBlock = saveRequest;
        }

        public Guid ScheduleConversion(XUnitData xUnitData)
        {
            var id = Guid.NewGuid();
            targetBlock.Post(new Tuple<Guid, XUnitData>(id, xUnitData));
            return id;
        }
    }
}
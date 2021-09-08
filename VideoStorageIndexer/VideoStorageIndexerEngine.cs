using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Adam.Core;
using Adam.Core.Classifications;
using Adam.Core.Fields;
using Adam.Core.Indexer;
using Adam.Tools.LogHandler;


namespace VideoStorageIndexer
{
    public class VideoStorageIndexerEngine : IndexMaintenanceJob
    {
        public VideoStorageIndexerEngine(Application app)
            : base(app)
        {
        }

        protected override void OnPreCatalog(PreCatalogEventArgs e)
        {
            base.OnPreCatalog(e);
        }

        protected override void OnCatalog(CatalogEventArgs e)
        {
            base.OnCatalog(e);
            FileInfo fi = new FileInfo(e.Path);
            e.Record.Fields.GetField<TextField>("Filesize-videostorage").SetValue(fi.Length.ToString());
            String fileName = Path.GetFileNameWithoutExtension(e.Path);
            e.Record.Fields.GetField<TextField>("Filename-videostorage").SetValue(fileName);
            String pathToXml = Path.GetDirectoryName(e.Path) + @"\" +
                 fileName  + ".xml";
            
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.IgnoreComments = true;
            xrs.IgnoreWhitespace = true;
            XmlReader reader = XmlReader.Create(pathToXml, xrs);
            reader.MoveToContent();
            String rootTagName = reader.Name;
            reader.Read();
            while (reader.Name != rootTagName)
            {
                String name = reader.Name;
                reader.ReadStartElement();
                String value = (String)reader.Value.Clone();
                reader.Read();
                if (e.Record.Fields.Count > 0 && e.Record.Fields.Contains(name))
                    e.Record.Fields.GetField<TextField>(name).SetValue(value);
                else
                    App.Log(LogSeverity.Error, "VIDEO STORAGE INDEXER: NO FIELDS WITH THIS NAMES ASSIGNED");
                reader.ReadEndElement();
            }
        }
    }
}
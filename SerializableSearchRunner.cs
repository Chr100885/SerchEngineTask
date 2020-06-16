using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceEngine.SearchRunners
{
   public class SerializableSearchRunner
    {
        public bool Disabled { get; internal set; }

        [XmlInclude(typeof(WebClientSearchRunner))]
        public abstract class serializableSearchRunner : ISearchRunner
        {
            [XmlAttribute]
            public string Name { get; set; }

            [XmlAttribute]

            [System.ComponentModel.DefaultValue(false)]
            public bool Disabled { get; set; }

            public abstract Task<long> Run(string query);
        }
    }
}


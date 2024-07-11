using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebService
{
    public class Request
    {
        public Guid Id { get; set; }
        public Dictionary<string, NameValueCollection> Sections { get; private set; }


        public Request()
        {
            this.Id = Guid.NewGuid();
            this.Sections = new Dictionary<string, NameValueCollection>();
        }


        public NameValueCollection this[string section]
        {
            get 
            {
                NameValueCollection result;

                if (Sections.TryGetValue(section, out result) == false)
                {
                    result = new NameValueCollection();
                    Sections.Add(section, result);  
                }

                return result;
            }
        }
    }
}

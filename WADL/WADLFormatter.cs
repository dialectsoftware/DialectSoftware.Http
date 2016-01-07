using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Xml.Schema;
using System.Collections;

//optional params (evaluate)
//doc
//add parse to get params already tested & return bool
//updaet href and path for faults/representations check for dulicate representations ...
//add GetWADLSchema
namespace WADL
{
    public class WADLFormatter
    {
        Uri baseUri;
        Type type;
     

        public WADLFormatter(Type type,Uri baseUri)
        {
            this.baseUri = baseUri;
            this.type = type;
        }

        public Stream Serialize(Encoding encoding)
        {
            Stream stream;
            WADLApplication application = IntroSpect();
            WADLUtility.Serialize<WADLApplication>(application, out stream);
            return stream;
        }

        public WADLApplication IntroSpect()
        {
            Dictionary<Type, XmlSchemaSet> grammars  = new Dictionary<Type, XmlSchemaSet>();;
            WADLApplication application = WADLApplication.GetWADLApplication(type);
            WADLResource resource = WADLResource.GetWADLResource(type, baseUri);
            WADLMethod[] methods = WADLMethod.GetWADLMethods(type, delegate(WADLParam wadlParam, XmlSchemaSet schemas)
            {
                AddSchema(grammars, wadlParam.ParameterInfo.ParameterType, schemas);
            },
            delegate(WADLRepresentation wadlParam, XmlSchemaSet schemas)
            {
                if (!String.IsNullOrEmpty(wadlParam.Href) && !application.Includes.Contains(wadlParam.Href.ToLower()))
                {
                    application.Includes.Add(wadlParam.Href.ToLower());
                }
                AddSchema(grammars, wadlParam.Type, schemas);
            });

            foreach (WADLMethod wadlMethod in methods)
            {
                    //add method
                    //TODO:allow dups if params and template are different
                    if (resource.Methods.Exists(delegate(WADLMethod method)
                    { 
                        bool exists = (method.MethodName == wadlMethod.MethodName);
                        if (exists)
                        {
                            if (method.Parameters.Count == wadlMethod.Parameters.Count)
                            {
                                int index = 0;
                                method.Parameters.ForEach(delegate(WADLParam param)
                                {
                                    exists = exists && (param.Style == wadlMethod.Parameters[0].Style);
                                    index++;
                                });
                            }
                            else
                            {
                                exists = false;
                            }
                        }
                        return exists;
                    }))
                    {
                        throw new NotSupportedException(String.Format("Duplicate WADLMethod '{0}'", wadlMethod.MethodName));
                    }
                    else
                    {
                        resource.Methods.Add(wadlMethod);
                    }

            }

            application.Resources.Add(resource);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            foreach (Type key in grammars.Keys)
            {
                foreach (XmlSchema schema in grammars[key].Schemas())
                {
                    string name = WADLUtility.GetSchemaName(schema);
                    if (!application.Grammars.ContainsKey(name))
                    {
                        application.Grammars.Add(name, schema);
                    }
                }
            }

            return application;
        }

        private void AddSchema(Dictionary<Type, XmlSchemaSet> grammars, Type type, XmlSchemaSet schemas)
        {
            if(schemas != null && schemas.Count > 0)
            {
                if (!grammars.ContainsKey(type))
                 {
                     grammars.Add(type, schemas);
                 }
            }
        
        }
    
    }
}



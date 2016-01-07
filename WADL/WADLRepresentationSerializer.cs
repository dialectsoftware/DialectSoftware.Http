using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;

namespace WADL
{

    public class WADLRepresentationSerializer : IFormatter
    {
        protected System.Xml.XmlDocument AccuDataObjectFormatter = null;
        protected StreamingContext context = new StreamingContext(System.Runtime.Serialization.StreamingContextStates.Persistence);

        public WADLRepresentationSerializer()
        {
            //
            // TODO: Add constructor logic here
            //

        }

        public Object Deserialize(Stream serializationStream)
        {
            return null;
        }

        /// <include file='doc\IFormatter.uex' path='docs/doc[@for="IFormatter.Serialize"]/*' />
        public void Serialize(Stream serializationStream, Object graph)
        {

            AccuDataObjectFormatter = new System.Xml.XmlDocument();
            System.Xml.XmlDeclaration Declaration = AccuDataObjectFormatter.CreateXmlDeclaration("1.0", "", "");
            System.Xml.XmlElement Element = AccuDataObjectFormatter.CreateElement("NewDataSet");

            if (graph != null)
            {
                if (graph.GetType().Equals(typeof(System.String)))
                {
                    Element.InnerText = graph.ToString();
                }
                else if (graph.GetType().GetInterface("IEnumerable") != null)
                {
                    Serialize(Element, graph);
                }
                else if (graph.GetType().IsArray)
                {
                    Serialize(Element, graph);
                }
                else if (graph.GetType().IsClass)
                {
                    Serialize(Element, graph);
                }
                else if (graph.GetType().IsPrimitive)
                {
                    Element.InnerText = graph.ToString();
                }
                else if (graph.GetType().IsValueType)
                {
                    System.Xml.XmlElement ValueType = AccuDataObjectFormatter.CreateElement(graph.GetType().Name);
                    Element.AppendChild(ValueType);
                }

            }

            AccuDataObjectFormatter.AppendChild(Declaration);
            AccuDataObjectFormatter.AppendChild(Element);
            System.IO.StreamWriter writer = new StreamWriter(serializationStream);
            writer.Write(AccuDataObjectFormatter.OuterXml);
            writer.Flush();
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
        }

        private void Serialize(System.Xml.XmlNode Root, Object graph)
        {
            System.Xml.XmlElement Element = null;

            if (graph.GetType().Equals(typeof(System.String)))
            {
                Serialize((System.Xml.XmlElement)Root, graph);

            }
            else if (graph.GetType().GetInterface("IEnumerable") != null)
            {
                Element = AccuDataObjectFormatter.CreateElement(graph.GetType().Name.Replace("[]", ""));
                Serialize(Element, graph);
                Root.AppendChild(Element);
            }
            else if (graph.GetType().IsArray)
            {
                Element = AccuDataObjectFormatter.CreateElement(graph.GetType().BaseType.Name);
                Serialize(Element, graph);
                Root.AppendChild(Element);
            }
            else if (graph.GetType().IsClass)
            {
                Element = AccuDataObjectFormatter.CreateElement(graph.GetType().Name);
                Serialize(Element, graph);
                Root.AppendChild(Element);
            }
            else if (graph.GetType().IsPrimitive)
            {
                Serialize((System.Xml.XmlElement)Root, graph);
            }
            else if (graph.GetType().IsValueType)
            {
                Serialize((System.Xml.XmlElement)Root, graph);
            }

        }

        private void Serialize(System.Xml.XmlElement Root, Object graph)
        {
            if (graph.GetType().Equals(typeof(System.String)))
            {
                System.Xml.XmlElement Element = AccuDataObjectFormatter.CreateElement(graph.GetType().Name);
                Element.InnerText = graph.ToString();
                Root.AppendChild(Element);
            }
            else if (graph.GetType().GetInterface("IEnumerable") != null)
            {
                foreach (object item in (System.Collections.IEnumerable)graph)
                {
                    if (item != null)
                        Serialize((System.Xml.XmlNode)Root, item);
                }
            }
            else if (graph.GetType().IsArray)
            {
                foreach (object item in ((Array)graph))
                {
                    if (item != null)
                        Serialize((System.Xml.XmlNode)Root, item);
                }
            }
            else if (graph.GetType().IsClass)
            {
                System.Reflection.PropertyInfo[] properties = graph.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    object[] attributes = property.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), true);
                    if (attributes.Length == 0)
                    {
                        if (property.PropertyType.Equals(typeof(System.String)))
                        {
                            Serialize(Root, graph, property);
                        }
                        else if (property.PropertyType.Equals(typeof(System.Array)))
                        {
                            System.Xml.XmlElement Array = AccuDataObjectFormatter.CreateElement(property.Name);
                            System.Reflection.MethodInfo _get = property.GetGetMethod(false);
                            object _value = _get.Invoke(graph, new object[] { });
                            if (_value != null)
                                Serialize(Array, _value);
                            Root.AppendChild(Array);
                        }
                        else if (property.PropertyType.IsClass)
                        {
                            System.Reflection.MethodInfo _get = property.GetGetMethod(false);
                            object _value = _get.Invoke(graph, new object[] { });
                            if (_value != null)
                            {
                                if (_value.GetType().GetInterface("IEnumerable") != null)
                                {
                                    System.Xml.XmlElement Collection = AccuDataObjectFormatter.CreateElement(property.Name);
                                    Serialize(Collection, _value);
                                    Root.AppendChild(Collection);
                                }
                                else
                                {
                                    System.Xml.XmlElement Element = AccuDataObjectFormatter.CreateElement(property.Name);
                                    Serialize(Element, _value);
                                    Root.AppendChild(Element);
                                }
                            }
                        }
                        else if (property.PropertyType.IsPrimitive)
                        {
                            Serialize(Root, graph, property);
                        }
                        else if (property.PropertyType.IsValueType)
                        {
                            Serialize(Root, graph, property);
                        }
                        else
                            Serialize(Root, graph, property);
                    }
                }
            }
            else if (graph.GetType().IsPrimitive)
            {
                System.Xml.XmlElement Element = AccuDataObjectFormatter.CreateElement(graph.GetType().Name);
                Element.InnerText = graph.ToString();
                Root.AppendChild(Element);
            }
            else if (graph.GetType().IsValueType)
            {
                System.Xml.XmlElement Element = AccuDataObjectFormatter.CreateElement(graph.GetType().Name);
                Root.AppendChild(Element);
            }

        }

        private void Serialize(System.Xml.XmlElement Root, Object graph, PropertyInfo property)
        {
            System.Xml.XmlElement Element = AccuDataObjectFormatter.CreateElement(property.Name);
            if (property.PropertyType.IsValueType && !property.PropertyType.IsPrimitive)
            {
                Root.AppendChild(Element);
            }
            else
            {
                System.Reflection.MethodInfo _get = property.GetGetMethod(false);
                if (graph != null)
                {
                    object _value = _get.Invoke(graph, new object[] { });
                    if (_value != null)
                        Element.InnerText = _value.ToString();
                }
                Root.AppendChild(Element);
            }
        }

        private void Serialize(PropertyInfo property)
        {
        }


        /// <include file='doc\IFormatter.uex' path='docs/doc[@for="IFormatter.SurrogateSelector"]/*' />
        public ISurrogateSelector SurrogateSelector
        {
            get { return null; }
            set { ;}
        }

        /// <include file='doc\IFormatter.uex' path='docs/doc[@for="IFormatter.Binder"]/*' />
        public SerializationBinder Binder
        {
            get { return null; }
            set { ;}
        }

        /// <include file='doc\IFormatter.uex' path='docs/doc[@for="IFormatter.Context"]/*' />
        public StreamingContext Context
        {
            get { return context; }
            set { ;}
        }

        //internal static object Parse(Type type, String m_value)
        //{
        //    //switch (Type.GetTypeCode(value.GetType()))
        //    switch (Type.GetTypeCode(type))
        //    {
        //        case TypeCode.Empty:
        //            throw new SystemException("Invalid data type");

        //        case TypeCode.Object:
        //            if (type.BaseType.Name == typeof(Array).Name)
        //            {
        //                Type t = Type.GetType(type.AssemblyQualifiedName.Replace("[]", ""));
        //                string[] values = m_value.Split(',');
        //                Array array = Array.CreateInstance(t, values.Length);
        //                for (int i = 0; i < values.Length; i++)
        //                {
        //                    array.SetValue(WADLSerializer.Parse(t, values[i]), i);
        //                }

        //                return (object)array;
        //            }
        //            else
        //            {
        //                return (object)m_value;
        //            }

        //        case TypeCode.DBNull:
        //        case TypeCode.Char:
        //        case TypeCode.SByte:
        //        case TypeCode.UInt16:
        //        case TypeCode.UInt32:
        //        case TypeCode.UInt64:
        //            // Throw a SystemException for unsupported data types.
        //            throw new SystemException("Invalid data type");

        //        case TypeCode.Boolean:
        //            return Boolean.Parse(m_value);

        //        case TypeCode.Byte:
        //            return Byte.Parse(m_value);

        //        case TypeCode.Int16:
        //            return Int16.Parse(m_value);

        //        case TypeCode.Int32:
        //            if (type.Name == typeof(Int32).Name)
        //            {
        //                return Int32.Parse(m_value);
        //            }
        //            else
        //            {
        //                return Enum.Parse(type, m_value);
        //            }

        //        case TypeCode.Int64:
        //            return Int64.Parse(m_value);

        //        case TypeCode.Single:
        //            return Single.Parse(m_value);

        //        case TypeCode.Double:
        //            return double.Parse(m_value);

        //        case TypeCode.Decimal:
        //            return Decimal.Parse(m_value);

        //        case TypeCode.DateTime:
        //            return DateTime.Parse(m_value);

        //        case TypeCode.String:
        //            return (String)m_value;

        //        default:
        //            throw new SystemException("Value is of an unsupported data type");
        //    }
        //}
    }

}

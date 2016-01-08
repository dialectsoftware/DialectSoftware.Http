using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DialectSoftware.Http.Services;
using System.IO;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services.Encoders
{
    public class StringEncoder:INetHttpEncoder
    {
        public object Decode(WADL.WADLParam param, object value)
        {
            switch (System.Type.GetTypeCode(param.ParameterInfo.ParameterType))
            {
                case TypeCode.Char:
                    return System.Convert.ToChar(value);
                case TypeCode.SByte:
                    return System.Convert.ToSByte(value);
                case TypeCode.UInt16:
                    return System.Convert.ToUInt16(value);
                case TypeCode.UInt32:
                    return System.Convert.ToUInt32(value);
                case TypeCode.UInt64:
                    return System.Convert.ToUInt64(value); ;
                case TypeCode.Boolean:
                    return System.Convert.ToBoolean(value);
                case TypeCode.Byte:
                    return System.Convert.ToByte(value); //?
                case TypeCode.Int16:
                    return System.Convert.ToInt16(value);
                case TypeCode.Int32:
                    return System.Convert.ToInt32(value);
                case TypeCode.Int64:
                    return System.Convert.ToInt64(value);
                case TypeCode.Single:
                    return System.Convert.ToSingle(value);
                case TypeCode.Double:
                    return System.Convert.ToDouble(value);
                case TypeCode.Decimal:
                    return System.Convert.ToDecimal(value);
                case TypeCode.DateTime:
                    return System.Convert.ToDateTime(value);
                case TypeCode.String:
                    return System.Convert.ToString(value); //can be any type in the event of a mismatch the app must catch it 
                case TypeCode.Object:
                    var cast = param.ParameterInfo.ParameterType.GetMethods(System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public).Where(m => m.ReturnType == param.ParameterInfo.ParameterType).Single();
                    return cast.Invoke(null, new[]{ value });
                case TypeCode.Empty:
                case TypeCode.DBNull:
                default:
                    throw new NotSupportedException(String.Format("datatype {0}", param.ParameterInfo.ParameterType.Name));
            }
        }

        public void Encode(System.IO.Stream stream, WADL.WADLRepresentation param, object value)
        {
            switch (System.Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                    stream.WriteByte((byte)value);
                    break;
                case TypeCode.Object:
                    if (value is Byte[])
                    {
                        byte[] bytes = (byte[])value;
                        stream.Write(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        StreamWriter objWriter = new StreamWriter(stream);
                        objWriter.AutoFlush = true;
                        objWriter.Write(value.ToString());
                    }
                    break;
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    break;
                case TypeCode.Char:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Boolean:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                default:
                    StreamWriter defaultWriter = new StreamWriter(stream);
                    defaultWriter.AutoFlush = true;
                    defaultWriter.Write(value.ToString());

                    break;
            }

        }
    }
}

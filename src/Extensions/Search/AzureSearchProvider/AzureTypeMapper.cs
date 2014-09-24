﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Search.Providers.Azure
{
    using RedDog.Search.Model;

    public static class AzureTypeMapper
    {

        public static string GetAzureSearchType(Type fieldType)
        {
            if (fieldType == typeof(String))
                return FieldType.String;

            if (fieldType == typeof(String[]))
                return FieldType.StringCollection;

            if (fieldType == typeof(Boolean))
                return FieldType.Boolean;

            if (fieldType == typeof(DateTime))
                return FieldType.DateTimeOffset;

            return GetAzureSearchNumericType(fieldType) ?? FieldType.String;
        }


        public static string GetAzureSearchNumericType(Type fieldType)
        {
            if (fieldType.IsEnum)
                return FieldType.Integer;

            // Map numeric types.
            var typeCode = Type.GetTypeCode(fieldType);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Boolean:
                    return FieldType.String;

                case TypeCode.UInt16:
                case TypeCode.Int16:
                    return FieldType.Integer;

                case TypeCode.UInt32:
                case TypeCode.Int32:
                    return FieldType.Integer;

                case TypeCode.UInt64:
                case TypeCode.Int64:
                    return FieldType.Integer;

                case TypeCode.Single:
                    return FieldType.Double;

                case TypeCode.Decimal:
                case TypeCode.Double:
                    return FieldType.Double;
            }
            return null;
        }

    }
}

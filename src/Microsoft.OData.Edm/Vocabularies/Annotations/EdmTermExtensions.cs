//---------------------------------------------------------------------
// <copyright file="EdmTermExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Vocabularies
{
    internal static class EdmTermExtensions
    {
        /// <summary>
        /// Parses a <paramref name="defaultValue"/> into an IEdmExpression value of the correct EDM type.
        /// </summary>
        /// <param name=“typeReference”>The type of value.</param>
        /// <param name=“defaultValue”>Original default value.</param>
        /// <returns>An IEdmExpression of type <paramref name="typeReference".</paramref></returns>
        public static IEdmExpression BuildDefaultEdmExpression(IEdmTypeReference typeReference, string defaultValue)
        {
            EdmTypeKind termTypeKind = typeReference.TypeKind();

            // Create expressions/constants for the corresponding types
            switch (termTypeKind) // TODO: Create test cases for all of these
            {
                case EdmTypeKind.Primitive:
                    return BuildEdmPrimitiveValueExp(typeReference.AsPrimitive(), defaultValue);
                case EdmTypeKind.Enum:
                    // return BuildEdmEnumExp(typeReference.AsEnum(), defaultValue)
                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                    // return BuildEdmStructuredTypeExp(typeReference.AsStructured(), defaultValue);
                case EdmTypeKind.Collection:
                    // return BuildEdmCollectionExp(typeReference, defaultValue);
                case EdmTypeKind.Path:
                    // return BuildEdmPathExp(typeReference.AsPath(), defaultValue);
                case EdmTypeKind.TypeDefinition:
                    // return BuildEdmTypeDefinitionExp(typeReference.AsTypeDefinition(), defaultValue);
                    throw Error.NotImplemented();
                case EdmTypeKind.EntityReference:
                case EdmTypeKind.Untyped:
                default:
                    throw Error.NotSupported();
            }
        }

        /// <summary>Returns an IEdmExpression for an EDM enum member.</summary>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <returns>Enum member expression for the default value.</returns>
        private static EdmEnumMemberExpression BuildEdmEnumExp(IEdmEnumTypeReference typeReference, string defaultValue)
        {
            IEdmEnumType enumType = typeReference.EnumDefinition();
            CsdlLocation location = new CsdlLocation(0, 0);
            EdmEnumValueParser.TryParseJsonEnumMember(defaultValue, enumType, location, out IEnumerable<IEdmEnumMember> results);
            return new EdmEnumMemberExpression(results.ToArray());
        }

        /// <summary>Returns an IEdmExpression for an EDM structured type.</summary>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <returns>Structured expression for the default value.</returns>
        // Example:
        // {
        //   "City":"Redmond",
        //   "Street": "156TH, AVE..."
        // }
        private static IEdmRecordExpression BuildEdmStructuredTypeExp(IEdmStructuredTypeReference typeReference, string defaultValue)
        {
            IDictionary<string, string> entityItems = ParseObject(defaultValue);
            List<IEdmPropertyConstructor> properties = new List<IEdmPropertyConstructor>();
            foreach (var entityItem in entityItems)
            {
                string propertyName = entityItem.Key; // propertyName => "City"
                var propertyType = typeReference.FindProperty(propertyName);
                string valueString = entityItem.Value;

                // TODO: get the actual value type for an IEdmExpression

                // IEdmExpression propertyExpression = BuildDefaultExpression(propertyType, valueString);
                // IEdmPropertyConstructor propertyConstruct = new EdmPropertyConstructor(propertyName, propertyExpression);
                // properties.Add(propertyConstruct);
            }

            return new EdmRecordExpression(properties);
        }

        /// <summary>Returns an IEdmExpression for an EDM collection type.</summary>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <returns>Collection expression for the default value.</returns>
        private static IEdmCollectionExpression BuildEdmCollectionExp(IEdmTypeReference typeReference, string defaultValue)
        {
            IEdmCollectionTypeReference collectionType = (IEdmCollectionTypeReference)typeReference;
            IEdmTypeReference elementType = collectionType.ElementType();
            var items = ParseCollection(defaultValue);
            IList<IEdmExpression> itemExpressions = new List<IEdmExpression>();
            foreach (var item in items)
            {
                IEdmExpression itemExpr = BuildDefaultEdmExpression(elementType, item);
                itemExpressions.Add(itemExpr);
            }

            return new EdmCollectionExpression(elementType, itemExpressions);
        }

        /// <summary>Returns an enumerable structure for a collection.</summary>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <returns>Enumerable structure of all of the elements within the collection.</returns>
        // "[5, 6, 8]"  => return "5", "6", "8"
        // "[{"city":["red}mond","seattle"]},{}]" -> return "{"city":["red}mond", "seattle"]}", "{}"
        internal static IEnumerable<string> ParseCollection(string defaultValue)
        {
            List<string> parsedCollection = new List<string>();
            int itemLevel = 0;
            bool inQuote = false;
            string currItem = "";
            for (int i = 1; i < defaultValue.Length - 1; i++)
            {
                char currChar = defaultValue[i];
                if (currChar == '\"')
                {
                    inQuote = !inQuote;
                }
                else if (!inQuote)
                {
                    if (currChar == '{' || currChar == '[')
                    {
                        itemLevel++;
                    }
                    else if (currChar == '}' || currChar == ']')
                    {
                        itemLevel--;
                    }
                }
                if (currChar == ',' && itemLevel == 0)
                {
                    parsedCollection.Add(currItem);
                    currItem = "";
                }
                else
                {
                    currItem += currChar;
                }
            }
            parsedCollection.Add(currItem);

            return parsedCollection;
        }

        /// <summary>Returns an IEdmExpression for an EDM path type.</summary>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <returns>Expression for the default value.</returns>
        private static IEdmExpression BuildEdmPathExp(IEdmPathTypeReference typeReference, string defaultValue)
        {
            IEdmPathType pathType = (IEdmPathType)typeReference.Definition;
            switch (pathType.PathKind)
            {
                case EdmPathTypeKind.AnnotationPath:
                    return new EdmAnnotationPathExpression(defaultValue);
                case EdmPathTypeKind.PropertyPath:
                    return new EdmPropertyPathExpression(defaultValue);
                case EdmPathTypeKind.NavigationPropertyPath:
                    return new EdmNavigationPropertyPathExpression(defaultValue);
                default:
                    return new EdmPathExpression(defaultValue);
            }
        }

        /// <summary>Returns an IEdmExpression for an EDM type definition type.</summary>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <returns>Expression for the default value.</returns>
        private static IEdmExpression BuildEdmTypeDefinitionExp(IEdmTypeDefinitionReference typeReference, string defaultValue)
        {
            IEdmPrimitiveTypeReference underType = new EdmPrimitiveTypeReference(typeReference.UnderlyingType(), true);
            return BuildDefaultEdmExpression(underType, defaultValue);
        }

        /// <summary>Returns a dictionary for an object.</summary>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <returns>Dictionary where the keys of the object correspond with its values.</returns>
        // Example:
        // {
        //   "City":"Redmond",
        //   "Street": "156TH, AVE..."
        // }
        // TODO: Add support for complicated objects, keep newline formatting into acount
        internal static IDictionary<string, string> ParseObject(string defaultValue)
        {
            IDictionary<string, string> parsedResult = new Dictionary<string, string>();
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(defaultValue));
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (!(line.StartsWith("{", System.StringComparison.Ordinal) ||
                          line.StartsWith("}", System.StringComparison.Ordinal)))
                    {
                        string[] terms = line.Split('\"');
                        string key = terms[1];
                        string value = terms[3];
                        parsedResult.Add(key, value);
                    }
                }
            }
            return parsedResult;
        }

        /// <summary>Returns an IEdmExpression for an EDM primitive value.</summary>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <returns>Primitive expression for the default value of the according type.</returns>
        private static IEdmExpression BuildEdmPrimitiveValueExp(IEdmPrimitiveTypeReference typeReference, string defaultValue)
        {
            switch (typeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    if (EdmValueParser.TryParseBinary(defaultValue, out byte[] binary))
                    {
                        return new EdmBinaryConstant(binary);
                    } else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Decimal:
                    if (EdmValueParser.TryParseDecimal(defaultValue, out decimal? dec))
                    {
                        return new EdmDecimalConstant(dec.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.String:
                    return new EdmStringConstant(defaultValue);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    if (EdmValueParser.TryParseDateTimeOffset(defaultValue, out System.DateTimeOffset? dto))
                    {
                        return new EdmDateTimeOffsetConstant(dto.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Duration:
                    if (EdmValueParser.TryParseDuration(defaultValue, out System.TimeSpan? ts))
                    {
                        return new EdmDurationConstant(ts.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.TimeOfDay:
                    if (EdmValueParser.TryParseTimeOfDay(defaultValue, out TimeOfDay? tod))
                    {
                        return new EdmTimeOfDayConstant(tod.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Boolean:
                    if (EdmValueParser.TryParseBool(defaultValue, out bool? bl))
                    {
                        return new EdmBooleanConstant(bl.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Double:
                    if (EdmValueParser.TryParseFloat(defaultValue, out double? dbl))
                    {
                        return new EdmFloatingConstant(dbl.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Guid:
                    if (EdmValueParser.TryParseGuid(defaultValue, out System.Guid? gd))
                    {
                        return new EdmGuidConstant(gd.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                    if (EdmValueParser.TryParseInt(defaultValue, out int? intNum))
                    {
                        return new EdmIntegerConstant(intNum.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Date:
                    if (EdmValueParser.TryParseDate(defaultValue, out Date? dt))
                    {
                        return new EdmDateConstant(dt.Value);
                    }
                    else
                    {
                        break;
                    }
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.PrimitiveType:
                case EdmPrimitiveTypeKind.None:
                default:
                    break;
            }
            throw Error.NotSupported();
        }

        /// <summary>
        /// Parses a <paramref name="defaultValue"/> into an IEdmExpression value of the correct CSDL type.
        /// </summary>
        /// <param name=“typeReference”>The type of value.</param>
        /// <param name=“defaultValue”>Original default value.</param>
        /// <returns>An IEdmExpression of type <paramref name="typeReference".</paramref></returns>
        public static CsdlExpressionBase BuildDefaultCsdlExpression(IEdmTypeReference typeReference, string defaultValue, CsdlLocation location)
        {
            EdmTypeKind typeKind = typeReference.TypeKind();
            switch (typeKind)
            {
                case EdmTypeKind.Primitive:
                    return BuildCsdlPrimitiveValueExp((IEdmPrimitiveTypeReference)typeReference, defaultValue, location);
                case EdmTypeKind.Collection:
                case EdmTypeKind.Entity:
                case EdmTypeKind.Complex:
                case EdmTypeKind.Enum:
                case EdmTypeKind.TypeDefinition:
                    throw Error.NotImplemented();
                default:
                    throw Error.NotSupported();
            }
        }

        /// <summary>Returns an IEdmExpression for a CSDL primitive value.</summary>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <param name="defaultValue">String representation of the term's default value.</param>
        /// <param name="location">Location for the new expression.</param>
        /// <returns>Primitive expression for the default value of the according type.</returns>
        private static CsdlExpressionBase BuildCsdlPrimitiveValueExp(IEdmPrimitiveTypeReference typeReference, string defaultValue, CsdlLocation location)
        {
            switch (typeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    return new CsdlConstantExpression(EdmValueKind.Binary, defaultValue, location);
                case EdmPrimitiveTypeKind.Decimal:
                    return new CsdlConstantExpression(EdmValueKind.Decimal, defaultValue, location);
                case EdmPrimitiveTypeKind.String:
                    return new CsdlConstantExpression(EdmValueKind.String, defaultValue, location);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return new CsdlConstantExpression(EdmValueKind.DateTimeOffset, defaultValue, location);
                case EdmPrimitiveTypeKind.Duration:
                    return new CsdlConstantExpression(EdmValueKind.Duration, defaultValue, location);
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new CsdlConstantExpression(EdmValueKind.TimeOfDay, defaultValue, location);
                case EdmPrimitiveTypeKind.Boolean:
                    return new CsdlConstantExpression(EdmValueKind.Boolean, defaultValue, location);
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Double:
                    return new CsdlConstantExpression(EdmValueKind.Floating, defaultValue, location);
                case EdmPrimitiveTypeKind.Guid:
                    return new CsdlConstantExpression(EdmValueKind.Guid, defaultValue, location);
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                    return new CsdlConstantExpression(EdmValueKind.Integer, defaultValue, location);
                case EdmPrimitiveTypeKind.Date:
                    return new CsdlConstantExpression(EdmValueKind.Date, defaultValue, location);
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.PrimitiveType:
                case EdmPrimitiveTypeKind.None:
                default:
                    break;
            }
            throw Error.NotSupported();
        }

#if false // backup
        public static IEdmExpression BuildDefaultExpression(IEdmTerm term)
        {
            if (term is EdmTerm edmTerm)
            {
                return BuildDefaultEdmExpression(edmTerm.Type, edmTerm.DefaultValue);
            }
            else if (term is CsdlSemanticsTerm csdlTerm)
            {
                return BuildDefaultCsdlExpression(csdlTerm.Type, csdlTerm.DefaultValue, term);
            }
            throw Error.NotSupported();
        }
#endif
    }
}

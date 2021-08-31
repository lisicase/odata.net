//---------------------------------------------------------------------
// <copyright file="EdmTermExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;

namespace Microsoft.OData.Edm.Vocabularies
{
#if false // backup
    internal static class EdmTermExtensions
    {
        public static IEdmExpression GetDefaultExpression(this IEdmModel model, IEdmTerm term)
        {
            if (term is EdmTerm edmTerm)
            {
                return BuildDefaultValue(edmTerm.Type, edmTerm.DefaultValue);
            }

            //if (term is CsdlSemanticsTerm csdlTerm)
            //{

            //}
            
        }

        private static IEdmExpression BuildDefaultValue(IEdmTypeReference typeReference, string defaultValue)
        {
            //string defaultValue = term.DefaultValue;
            EdmTypeKind termTypeKind = typeReference.TypeKind();

            // Create constants for the corresponding types
            switch(termTypeKind)
            {
                case EdmTypeKind.Primitive:
                    return BuildPrimitiveValue(typeReference.AsPrimitive(), defaultValue);

                case EdmTypeKind.Enum:
                    // TODO: return new EdmEnumMemberExpression(....);
                case EdmTypeKind.Complex:
                    // TODO
                case EdmTypeKind.Entity:
                    // Example:
                    // {
                    //   "City":"Redmond",
                    //   "Street": "156TH, AVE..."
                    // }
                    var properties = ParseObject(defaultValue);
                    foreach (var item in properties)
                    {
                        // get the property
                        // get the property type from strucutral type
                        // build the propertyvalue
                    }

                    // build the properties expression one by one
                    return new EdmRecordExpression(....);
                case EdmTypeKind.Collection:
                    IEdmCollectionTypeReference collectionType = (IEdmCollectionTypeReference)typeReference;
                    IEdmTypeReference elementType = collectionType.ElementType();
                    // If it's in the scope:
                    // term.DefaultValue could be a JSON string
                    // [5, 6, 9]
                    // [{"city":["redmond","seattle"]},{}]
                    // Have to recursively parse the JSON array into items

                    var items = ParseCollection(defaultValue);
                    IList<IEdmExpression> itemExpressions = new List<IEdmExpression>();
                    foreach (var item in items)
                    {
                        IEdmExpression itemExpr = BuildDefaultValue(elementType, item);
                        itemExpressions.Add(itemExpr);
                    }

                    return new EdmCollectionExpression(elementType, itemExpressions);

                //case EdmTypeKind.TypeDefinition:
                //    ....
                // Note: Need to have cases for the following types:
                // DateTimeOffsetConstant, DecimalConstant, FloatingConstant, GuidConstant, IntegerConstant,
                // StringConstant, DurationConstant, Null, Record, Collection, Path, If, Cast, IsType,
                // FunctionApplication, LabeledExpressionReference, Labeled, PropertyPath, NavigationPropertyPath,
                // DateConstant, TimeOfDayConstant, EnumMember, AnnotationPath
            }
        }

        // Parse an object from a string
        private static IDictionary<string, string> ParseObject(string defaultValue)
        {
            // parse the JSON array into items
            // "[5, 6, 8]"  => return "5", "6", "8"
            // defaultValue.
            return null;
        }

        // Parse a collection from a string
        private static IEnumerable<string> ParseCollection(string defaultValue)
        {
            // parse the JSON array into items
            // "[5, 6, 8]"  => return "5", "6", "8"
            // defaultValue.
            return null;
        }

        // Return an IEdmExpression for a primitive value from a string
        private static IEdmExpression BuildPrimitiveValue(IEdmPrimitiveTypeReference reference, string defaultValue, bool isEdm)
        {
            switch (reference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    byte[] binary = new byte[0];
                    EdmValueParser.TryParseBinary(defaultValue, out binary);
                    return new EdmBinaryConstant(binary);
         
                // ...
                case EdmPrimitiveTypeKind.Decimal:
                    this.ProcessDecimalTypeReference(reference.AsDecimal());
                    break;
                case EdmPrimitiveTypeKind.String:
                    this.ProcessStringTypeReference(reference.AsString());
                    break;
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    this.ProcessTemporalTypeReference(reference.AsTemporal());
                    break;
                case EdmPrimitiveTypeKind.Boolean:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.Byte:
                    // TODO: return constant
                case EdmPrimitiveTypeKind.Double:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.Guid:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.Int16:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.Int32:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.Int64:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.SByte:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.Single:
                    // TODO: return constant
                    break;
                case EdmPrimitiveTypeKind.Date:
                    // TODO: return constant
                    break;

                    // Note: Ignoring these for now - should they be supported for expressions?
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
                    // Throw exception here for not supported TypeKind
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_PrimitiveKind(reference.PrimitiveKind().ToString()));
            }
        }

        private static CsdlExpressionBase GetDefaultExpression()
    }
#endif
}

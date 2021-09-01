//---------------------------------------------------------------------
// <copyright file="EdmVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Data.OData;
using Microsoft.OData.Edm.Csdl;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM annotation with an immediate value.
    /// </summary>
    public class EdmVocabularyAnnotation : EdmElement, IEdmVocabularyAnnotationWithDefault
    {
        private readonly IEdmVocabularyAnnotatable target;
        private readonly IEdmTerm term;
        private readonly string qualifier;
        private readonly IEdmExpression value;
        private readonly bool usesDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmVocabularyAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, IEdmExpression value)
            : this(target, term, null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmVocabularyAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, string qualifier, IEdmExpression value)
        {
            EdmUtil.CheckArgumentNull(target, "target");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(value, "value");

            this.target = target;
            this.term = term;
            this.qualifier = qualifier;
            this.value = value;
            this.usesDefault = false;
        }

        /// <summary>
        /// Initializes a new instance of the EdmVocabularyAnnotation class
        /// to the default value.
        /// </summary>
        /// <param name=“target”>Element the annotation applies to.</param>
        /// <param name=“term”>Term bound by the annotation</param>
        public EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term)
        {
            // Check arguments
            if (term.DefaultValue == null) // throw error if no default value
            {
                // Note: why is this string not okay but the one in LiteralFormatter.cs is?
                // Should I create the following in Microsoft.OData.Core.cs?
                // internal const string EdmVocabularyAnnotations_DidNotFindDefaultValue = "EdmVocabularyAnnotations_DidNotFindDefaultValue";
                //throw new ODataException("Type name should not be null or empty when serializing an Enum value for URI key.");
                EdmUtil.CheckArgumentNull(new EdmStringConstant(null), "value");
            }
            EdmUtil.CheckArgumentNull(target, "target");
            EdmUtil.CheckArgumentNull(term, "term");

            // Check arguments
            EdmUtil.CheckArgumentNull(target, "target");
            EdmUtil.CheckArgumentNull(term, "term");

            // Initialize
            this.target = target;
            this.term = term;
            this.qualifier = null;
            //this.value = BuildDefaultValue(term); // Note: Eventually this method will parse the default value
            this.value = new EdmStringConstant(term.DefaultValue); // Note: temporarily only supporting Strings
            this.usesDefault = true;
        }

        /// <summary>
        /// Parses a <paramref name="defaultValue"/> into an IEdmExpression value of the correct type.
        /// </summary>
        /// <param name=“typeReference”>The type of value.</param>
        /// <param name=“defaultValue”>Original default value.</param>
        /// <returns>An IEdmExpression of type <paramref name="typeReference".</paramref></returns>
        /*private static IEdmExpression BuildDefaultValue(IEdmTypeReference typeReference, string defaultValue)
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
        }*/

        // Parse an object from a string
        /*private static IDictionary<string, string> ParseObject(string defaultValue)
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
        }*/

        // Return an IEdmExpression for a primitive value from a string
        /*private static IEdmExpression BuildPrimitiveValue(IEdmPrimitiveTypeReference reference, string defaultValue)
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
        }*/

        /* Note: This constructor is ambiguous with the other three-param constructor...
        /// <summary>
        /// NEW: Initializes a new instance of the EdmVocabularyAnnotation class 
        /// to the default value.
        /// </summary>
        /// <param name=“target”>Element the annotation applies to.</param>
        /// <param name=“term”>Term bound by the annotation</param>
        /// <param name=“qualifier”>Qualifier used to distinguish bindings.</param>
        public EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term,
        string qualifier)
        { }*/

        /// <summary>
        /// Gets the element the annotation applies to.
        /// </summary>
        public IEdmVocabularyAnnotatable Target
        {
            get { return this.target; }
        }

        /// <summary>
        /// Gets the term bound by the annotation.
        /// </summary>
        public IEdmTerm Term
        {
            get { return this.term; }
        }

        /// <summary>
        /// Gets the qualifier used to discriminate between multiple bindings of the same property or type.
        /// </summary>
        public string Qualifier
        {
            get { return this.qualifier; }
        }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets whether the annotation uses a default value
        /// (not defined with a provided value).
        /// </summary>
        public bool UsesDefault {
            get { return this.usesDefault; }
        }
    }
}

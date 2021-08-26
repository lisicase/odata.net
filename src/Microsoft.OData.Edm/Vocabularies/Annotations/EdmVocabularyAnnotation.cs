//---------------------------------------------------------------------
// <copyright file="EdmVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM annotation with an immediate value.
    /// </summary>
    public class EdmVocabularyAnnotation : EdmElement, IEdmVocabularyAnnotation
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
        /// NEW: Initializes a new instance of the EdmVocabularyAnnotation class
        /// to the default value.
        /// </summary>
        /// <param name=“target”>Element the annotation applies to.</param>
        /// <param name=“term”>Term bound by the annotation</param>
        public EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term)
            //: this(target, term, null, new EdmStringConstant(term.DefaultValue))
        {
            // Check arguments
            if (term.DefaultValue == null) // throw error if no default value
            {
                //throw new ArgumentNullException("value");
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
            //this.value = BuildDefaultValue(term);
            this.value = new EdmStringConstant(term.DefaultValue);
            this.usesDefault = true;
        }

        /*private static IEdmExpression BuildDefaultValue(IEdmTypeReference typeReference, string defaultValue)
        {
            //string defaultValue = term.DefaultValue;
            EdmTypeKind termTypeKind = typeReference.TypeKind();

            switch(termTypeKind)
            {
                case EdmTypeKind.Primitive:
                    return BuildPrimitiveValue(typeReference.AsPrimitive(), defaultValue);

                case EdmTypeKind.Enum:

                    // EdmEnumMemberExpression
                    // do the enum express
                    return new EdmEnumMemberExpression(....);

                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                    // {
                    //   "City":"Remodn",
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
                    // Be sure is it in the scope?
                    // If ti's in the scope
                    // term.DefaultValue could be a JSON string
                    // [5, 6, 9]
                    // [{"city":"red,[]mond", },{}]
                    // Have to parse the JSON array into Items

                    var items = ParseCollection(defaultValue);
                    IList<IEdmExpression> itemExpressions = new List<IEdmExpression>();
                    foreach (var item in items)
                    {
                        IEdmExpression itemExpr = BuildDefaultValue(elementType, item);
                        itemExpressions.Add(itemExpr);
                    }

                    return new EdmCollectionExpression(elementType, itemExpressions);

                case EdmTypeKind.TypeDefinition:
                    ....


            }*/

            //return new EdmNavigationPropertyPathExpression(defaultValue);

            //IEdmExpression defaultValueExp = new EdmStringConstant(defaultValue);

            /*if (object.Equals(termTypeKind, IEdmExpression.BinaryConstant))
            {
                return new EdmBinaryConstant(System.Binary.Parse(defaultValue));
            }
            else if (object.Equals(termTypeKind, IEdmExpression.BooleanConstant))
            {
                return new EdmBooleanConstant(System.Boolean.Parse(defaultValue));
            }
            // ...
            else
            {
                return new EdmStringConstant(defaultValue);
            }


            // DateTimeOffsetConstant, DecimalConstant, FloatingConstant, GuidConstant, IntegerConstant,
            // StringConstant, DurationConstant, Null, Record, Collection, Path, If, Cast, IsType,
            // FunctionApplication, LabeledExpressionReference, Labeled, PropertyPath, NavigationPropertyPath,
            // DateConstant, TimeOfDayConstant, EnumMember, AnnotationPath

            //return defaultValueExp;
        }*/

        /* static IDictionary<string, string> ParseObject(string defaultValue)
        {
            // parse the JSON array into items
            // "[5, 6, 8]"  => return "5", "6", "8"
            // defaultValue.
            return null;
        }

        private static IEnumerable<string> ParseCollection(string defaultValue)
        {
            // parse the JSON array into items
            // "[5, 6, 8]"  => return "5", "6", "8"
            // defaultValue.
            return null;
        }*/

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
                    // add code to create bollean cont
                    break;
                case EdmPrimitiveTypeKind.Byte:
                    // add code to creat 
                case EdmPrimitiveTypeKind.Double:
                    break;
                case EdmPrimitiveTypeKind.Guid:
                    // add code to create Guid Contant
                    break;
                case EdmPrimitiveTypeKind.Int16:
                    break;
                case EdmPrimitiveTypeKind.Int32:
                    break;
                case EdmPrimitiveTypeKind.Int64:
                    break;
                case EdmPrimitiveTypeKind.SByte:
                    break;
                case EdmPrimitiveTypeKind.Single:
                    break;
                case EdmPrimitiveTypeKind.Date:
                    break;

                    // below type kinds are not supported for expression?
                case EdmPrimitiveTypeKind.Stream:
                    ///fkasdlkjfasdlfjdsal;jflaj
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
                    // Throw exception here for notsupported tyep kind
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_PrimitiveKind(reference.PrimitiveKind().ToString()));
            }
        }*/

        /* This constructor is ambiguous with the other three-param constructor...
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

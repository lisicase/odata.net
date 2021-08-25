//---------------------------------------------------------------------
// <copyright file="EdmVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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

            // Find default value
            string defaultValue = term.DefaultValue;
            IEdmExpression defaultValueExp = new EdmStringConstant(defaultValue);

            // Check arguments
            EdmUtil.CheckArgumentNull(target, "target");
            EdmUtil.CheckArgumentNull(term, "term");

            // Initialize
            this.target = target;
            this.term = term;
            this.qualifier = null;
            this.value = defaultValueExp;
            this.usesDefault = true;
        }

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
        /// Gets Whether the annotation uses a default value
        /// (not defined with a provided value).
        /// </summary>
        public bool UsesDefault {
            get { return this.usesDefault; }
        }
    }
}

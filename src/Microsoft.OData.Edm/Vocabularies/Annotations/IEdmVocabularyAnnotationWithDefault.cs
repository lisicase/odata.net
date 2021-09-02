//---------------------------------------------------------------------
// <copyright file="IEdmVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM vocabulary annotation that may use a term's default value as its value.
    /// </summary>
    public interface IEdmVocabularyAnnotationWithDefault : IEdmVocabularyAnnotation
    {
        /// <summary>
        /// Gets whether the annotation uses a default value
        /// (not defined with a provided value).
        /// </summary>
        bool UsesDefault { get; }
    }
}

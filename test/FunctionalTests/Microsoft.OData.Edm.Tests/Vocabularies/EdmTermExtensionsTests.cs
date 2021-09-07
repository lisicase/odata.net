//---------------------------------------------------------------------
// <copyright file="ValidationVocabularyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    /// <summary>
    /// Test EdmTermExtensions
    /// </summary>
    public class EdmTermExtensionsTests
    {

        [Fact]
        public void CollectionTermParsesCorrectly()
        {
            string defaultValue = "[{\"city\":[\"redmond\",\"s{eattle\"]},{\"state\":[\"wa\",\"ca\"]},\"test\"]";
            IEnumerable<string> results = EdmTermExtensions.ParseCollection(defaultValue);
            var resultsArray = results.ToArray();
            Assert.Equal(3, resultsArray.Length);
            Assert.Equal("{\"city\":[\"redmond\",\"s{eattle\"]}", resultsArray[0]);
            Assert.Equal("{\"state\":[\"wa\",\"ca\"]}", resultsArray[1]);
            Assert.Equal("\"test\"", resultsArray[2]);

            // TODO: add additional cases - empty array, empty string, null, whitespace, special chars, etc.
        }
    }
}

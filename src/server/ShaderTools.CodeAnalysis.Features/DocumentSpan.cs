// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using ShaderTools.CodeAnalysis.Text;
using ShaderTools.Utilities;

namespace ShaderTools.CodeAnalysis
{
    /// <summary>
    /// Represents a <see cref="TextSpan"/> location in a <see cref="Document"/>.
    /// </summary>
    internal struct DocumentSpan : IEquatable<DocumentSpan>
    {
        public LogicalDocument Document { get; }
        public SourceFileSpan SourceSpan { get; }

        public DocumentSpan(LogicalDocument document, SourceFileSpan sourceSpan)
        {
            Document = document;
            SourceSpan = sourceSpan;
        }

        public override bool Equals(object obj)
            => Equals((DocumentSpan) obj);

        public bool Equals(DocumentSpan obj)
            => this.Document == obj.Document && this.SourceSpan == obj.SourceSpan;

        public static bool operator ==(DocumentSpan d1, DocumentSpan d2)
            => d1.Equals(d2);

        public static bool operator !=(DocumentSpan d1, DocumentSpan d2)
            => !(d1 == d2);

        public override int GetHashCode()
            => Hash.Combine(
                this.Document,
                this.SourceSpan.GetHashCode());
    }
}
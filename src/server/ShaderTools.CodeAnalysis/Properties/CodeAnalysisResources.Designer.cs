﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShaderTools.CodeAnalysis.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CodeAnalysisResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CodeAnalysisResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ShaderTools.CodeAnalysis.Properties.CodeAnalysisResources", typeof(CodeAnalysisResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The changes must be ordered and not overlapping..
        /// </summary>
        internal static string ChangesMustBeOrderedAndNotOverlapping {
            get {
                return ResourceManager.GetString("ChangesMustBeOrderedAndNotOverlapping", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;end&apos; must not be less than &apos;start&apos;.
        /// </summary>
        internal static string EndMustNotBeLessThanStart {
            get {
                return ResourceManager.GetString("EndMustNotBeLessThanStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The span does not include the end of a line..
        /// </summary>
        internal static string SpanDoesNotIncludeEndOfLine {
            get {
                return ResourceManager.GetString("SpanDoesNotIncludeEndOfLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The span does not include the start of a line..
        /// </summary>
        internal static string SpanDoesNotIncludeStartOfLine {
            get {
                return ResourceManager.GetString("SpanDoesNotIncludeStartOfLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;start&apos; must not be negative.
        /// </summary>
        internal static string StartMustNotBeNegative {
            get {
                return ResourceManager.GetString("StartMustNotBeNegative", resourceCulture);
            }
        }
    }
}

﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace RepoLite.GeneratorEngine.Generators.CSharp.SQLServer.Pk.GenParts
{
    using System.Text;
    using System.Linq;
    using System.Xml;
    using Common;
    using Helpers;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class Interface : InterfaceBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 8 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
 var pk = generationObject.Table.PrimaryKeys.FirstOrDefault();
            
            #line default
            #line hidden
            this.Write("    public partial interface I");
            
            #line 9 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.RepositoryName));
            
            #line default
            #line hidden
            this.Write(" : IPkRepository<");
            
            #line 9 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(">\r\n    {\r\n\t\t");
            
            #line 11 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(" Get(");
            
            #line 11 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 11 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.FieldName));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 12 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
 if (generationSettings.IncludeCaching) {
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 13 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(" Get(");
            
            #line 13 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 13 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.FieldName));
            
            #line default
            #line hidden
            this.Write(", bool skipCache);\r\n");
            
            #line 14 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\tIEnumerable<");
            
            #line 15 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write("> Get(List<");
            
            #line 15 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 15 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.FieldName));
            
            #line default
            #line hidden
            this.Write("s);\r\n");
            
            #line 16 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
 if (generationSettings.IncludeCaching) {
            
            #line default
            #line hidden
            this.Write("\t\tIEnumerable<");
            
            #line 17 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write("> Get(List<");
            
            #line 17 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 17 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.FieldName));
            
            #line default
            #line hidden
            this.Write("s, bool skipCache);\r\n");
            
            #line 18 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\tIEnumerable<");
            
            #line 19 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write("> Get(params ");
            
            #line 19 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write("[] ");
            
            #line 19 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.FieldName));
            
            #line default
            #line hidden
            this.Write("s);\r\n\t\t\r\n        ");
            
            #line 21 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write(" GetMaxId();\r\n\t\tbool Delete(");
            
            #line 22 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 22 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.FieldName));
            
            #line default
            #line hidden
            this.Write(");\r\n\t\tbool Delete(IEnumerable<");
            
            #line 23 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataTypeString));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 23 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.FieldName));
            
            #line default
            #line hidden
            this.Write("s);\r\n\r\n");
            
            #line 25 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TtHelpers.AppendInheritanceLogic(generationObject,(column, rgo) =>
                $"{Helpers.Tab2}bool DeleteBy{column.DbColumnName}({column.DataTypeString} {column.FieldName});\n")));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n        IEnumerable<");
            
            #line 28 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write("> Search(\r\n");
            
            #line 29 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TtHelpers.AppendInheritanceLogic(generationObject,(column, rgo) =>
    {
        var isb = new StringBuilder();
        isb.Append(column.DataType != typeof(XmlDocument)
                ? $"{Helpers.Tab3}{column.DataTypeString}{(Helpers.IsCSharpNullable(column.DataTypeString) ? "?" : string.Empty)} {column.FieldName} = null"
                : $"{Helpers.Tab3}String {column.FieldName} = null");
        
        if (rgo.InheritedDependency != null || column != rgo.Table.Columns.Last())
                isb.AppendLine(",");
        
        return isb.ToString();
    })));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n");
            
            #line 42 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TtHelpers.AppendInheritanceLogic(generationObject, (column, rgo) =>
    {
        var isb = new StringBuilder();
        isb.AppendLine(
            column.DataType != typeof(XmlDocument)
                ? $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}({column.DataTypeString} {column.FieldName});"
                : $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}(String {column.FieldName});");

        if (generationSettings.IncludeCaching)
        {
            isb.AppendLine(
                column.DataType != typeof(XmlDocument)
                    ? $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}({column.DataTypeString} {column.FieldName}, bool skipCache);"
                    : $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}(String {column.FieldName}, bool skipCache);");
        }
        
        isb.AppendLine(
            column.DataType != typeof(XmlDocument)
                ? $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}(FindComparison comparison, {column.DataTypeString} {column.FieldName});"
                : $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}(FindComparison comparison, String {column.FieldName});");


        if (generationSettings.IncludeCaching)
        {
            isb.AppendLine(
                column.DataType != typeof(XmlDocument)
                    ? $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}(FindComparison comparison, {column.DataTypeString} {column.FieldName}, bool skipCache);"
                    : $"{Helpers.Tab2}IEnumerable<{generationObject.Table.ClassName}> FindBy{column.PropertyName}(FindComparison comparison, String {column.FieldName}, bool skipCache);");
        }
        return isb.ToString();
    })));
            
            #line default
            #line hidden
            this.Write("\r\n    }");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Pk\GenParts\Interface.tt"

private global::RepoLite.Common.Options.GenerationOptions _generationSettingsField;

/// <summary>
/// Access the generationSettings parameter of the template.
/// </summary>
private global::RepoLite.Common.Options.GenerationOptions generationSettings
{
    get
    {
        return this._generationSettingsField;
    }
}

private global::RepoLite.Common.Models.RepositoryGenerationObject _generationObjectField;

/// <summary>
/// Access the generationObject parameter of the template.
/// </summary>
private global::RepoLite.Common.Models.RepositoryGenerationObject generationObject
{
    get
    {
        return this._generationObjectField;
    }
}


/// <summary>
/// Initialize the template
/// </summary>
public virtual void Initialize()
{
    if ((this.Errors.HasErrors == false))
    {
bool generationSettingsValueAcquired = false;
if (this.Session.ContainsKey("generationSettings"))
{
    this._generationSettingsField = ((global::RepoLite.Common.Options.GenerationOptions)(this.Session["generationSettings"]));
    generationSettingsValueAcquired = true;
}
if ((generationSettingsValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("generationSettings");
    if ((data != null))
    {
        this._generationSettingsField = ((global::RepoLite.Common.Options.GenerationOptions)(data));
    }
}
bool generationObjectValueAcquired = false;
if (this.Session.ContainsKey("generationObject"))
{
    this._generationObjectField = ((global::RepoLite.Common.Models.RepositoryGenerationObject)(this.Session["generationObject"]));
    generationObjectValueAcquired = true;
}
if ((generationObjectValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("generationObject");
    if ((data != null))
    {
        this._generationObjectField = ((global::RepoLite.Common.Models.RepositoryGenerationObject)(data));
    }
}


    }
}


        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class InterfaceBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
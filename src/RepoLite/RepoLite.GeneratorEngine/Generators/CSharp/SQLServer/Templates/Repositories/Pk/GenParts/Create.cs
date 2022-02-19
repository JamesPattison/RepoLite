﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace RepoLite.GeneratorEngine.Generators.CSharp.SQLServer.Templates.Repositories.Pk.GenParts
{
    using RepoLite.Common.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RepoLite.Common.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class Create : CreateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 8 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 var pk = generationObject.Table.PrimaryKeys.FirstOrDefault();
            
            #line default
            #line hidden
            this.Write("       public override bool Create(");
            
            #line 9 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(" item)\r\n        {\r\n            //Validation\r\n            if (item == null)\r\n                return false;\r\n\r\n            var validationErrors = item.Validate();\r\n            if (validationErrors.Any())\r\n                throw new ValidationException(validationErrors);\r\n\r\n");
            
            #line 19 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 if (generationObject.Inherits) { 
            
            #line default
            #line hidden
            this.Write("            if (_");
            
            #line 20 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.InheritedTable.RepositoryName.LowerFirst()));
            
            #line default
            #line hidden
            this.Write(".Create(item))\r\n            {\r\n                var createdKeys = BaseCreate(\r\n");
            
            #line 23 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 foreach (var column in generationObject.Table.Columns) { 
            
            #line default
            #line hidden
            this.Write("                    item.");
            
            #line 24 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column.PropertyName));
            
            #line default
            #line hidden
            
            #line 24 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column == generationObject.Table.Columns.Last()? ");": ","));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 25 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("                if (createdKeys.Count != ");
            
            #line 26 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(".Columns.Count(x => x.PrimaryKey))\r\n                    return false;\r\n\r\n                item.");
            
            #line 29 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.PropertyName));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 29 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataType));
            
            #line default
            #line hidden
            this.Write(".Parse(createdKeys[nameof(");
            
            #line 29 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 29 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.PropertyName));
            
            #line default
            #line hidden
            this.Write(")].ToString());\r\n                item.ResetDirty();\r\n\r\n");
            
            #line 32 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 if (generationSettings.IncludeCaching) {
            
            #line default
            #line hidden
            this.Write("                if (CacheEnabled)\r\n                {\r\n                    SaveToCache(item);\r\n                }\r\n");
            
            #line 37 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("                return true;\r\n            }\r\n            return false;\r\n");
            
            #line 41 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("            var createdKeys = BaseCreate(\r\n");
            
            #line 43 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 foreach (var column in generationObject.Table.Columns) { 
            
            #line default
            #line hidden
            this.Write("                    item.");
            
            #line 44 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column.PropertyName));
            
            #line default
            #line hidden
            
            #line 44 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column == generationObject.Table.Columns.Last()? ");": ","));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 45 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            if (createdKeys.Count != ");
            
            #line 46 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(".Columns.Count(x => x.PrimaryKey))\r\n                return false;\r\n\r\n            item.");
            
            #line 49 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.PropertyName));
            
            #line default
            #line hidden
            this.Write(" = ");
            
            #line 49 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.DataType));
            
            #line default
            #line hidden
            this.Write(".Parse(createdKeys[nameof(");
            
            #line 49 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 49 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pk.PropertyName));
            
            #line default
            #line hidden
            this.Write(")].ToString());\r\n            item.ResetDirty();\r\n\r\n");
            
            #line 52 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 if (generationSettings.IncludeCaching) {
            
            #line default
            #line hidden
            this.Write("            if (CacheEnabled)\r\n            {\r\n                SaveToCache(item);\r\n            }\r\n");
            
            #line 57 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            return true;\r\n");
            
            #line 59 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n        public override bool BulkCreate(params ");
            
            #line 62 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write("[] items)\r\n        {\r\n            if (!items.Any())\r\n                return false;\r\n\r\n            var validationErrors = items.SelectMany(x => x.Validate()).ToList();\r\n            if (validationErrors.Any())\r\n                throw new ValidationException(validationErrors);\r\n\r\n");
            
            #line 71 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 if (generationObject.Inherits) { 
            
            #line default
            #line hidden
            this.Write("            if (_");
            
            #line 72 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.InheritedTable.RepositoryName.LowerFirst()));
            
            #line default
            #line hidden
            this.Write(".BulkCreate(items))\r\n            {\r\n                var dt = new DataTable();\r\n                foreach (var mergeColumn in ");
            
            #line 75 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(".Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))\r\n                    dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);\r\n\r\n                foreach (var item in items)\r\n                {\r\n                    dt.Rows.Add(\r\n");
            
            #line 81 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 foreach (var column in generationObject.Table.Columns) { 
            
            #line default
            #line hidden
            this.Write("                        item.");
            
            #line 82 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column.PropertyName));
            
            #line default
            #line hidden
            
            #line 82 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column == generationObject.Table.Columns.Last()? ");": ","));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 83 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("                }\r\n\r\n                if (BulkInsert(dt))\r\n                {\r\n");
            
            #line 88 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 if (generationSettings.IncludeCaching) {
            
            #line default
            #line hidden
            this.Write("                    if (CacheEnabled)\r\n                    {\r\n                        foreach (var item in items)\r\n                        {\r\n                            SaveToCache(item);\r\n                        }\r\n                    }\r\n");
            
            #line 96 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("                    return true;\r\n                }\r\n                return false;\r\n            }\r\n            return false;\r\n");
            
            #line 102 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("\t\t\tvar dt = new DataTable();\r\n\t\t\tforeach (var mergeColumn in ");
            
            #line 104 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write(".Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))\r\n\t\t\t\tdt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);\r\n\r\n\t\t\tforeach (var item in items)\r\n\t\t\t{\r\n\t\t\t\tdt.Rows.Add(\r\n");
            
            #line 110 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 foreach (var column in generationObject.Table.Columns) { 
            
            #line default
            #line hidden
            this.Write("                    item.");
            
            #line 111 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column.PropertyName));
            
            #line default
            #line hidden
            
            #line 111 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(column == generationObject.Table.Columns.Last()? ");": ","));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 112 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t}\r\n\r\n\t\t\tif (BulkInsert(dt))\r\n\t\t\t{\r\n");
            
            #line 117 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 if (generationSettings.IncludeCaching) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\tif (CacheEnabled)\r\n\t\t\t\t{\r\n\t\t\t\t\tforeach (var item in items)\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tSaveToCache(item);\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n");
            
            #line 125 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t\treturn true;\r\n\t\t\t}\r\n\t\t\treturn false;\r\n");
            
            #line 129 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
 } 
            
            #line default
            #line hidden
            this.Write("        }\r\n        public override bool BulkCreate(List<");
            
            #line 131 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(generationObject.Table.ClassName));
            
            #line default
            #line hidden
            this.Write("> items)\r\n        {\r\n            return BulkCreate(items.ToArray());\r\n        }");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "C:\Users\Jimmy\source\repos\RepoLite\src\RepoLite\RepoLite.GeneratorEngine\Generators\CSharp\SQLServer\Templates\Repositories\Pk\GenParts\Create.tt"

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
    public class CreateBase
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

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WSServer.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WSServer.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to /*
        /// * echotest.js
        /// *
        /// * Derived from Echo Test of WebSocket.org (http://www.websocket.org/echo.html).
        /// *
        /// * Copyright (c) 2012 Kaazing Corporation.
        /// */
        ///
        ///var url = &quot;ws://localhost:4649/Echo&quot;;
        /////var url = &quot;wss://localhost:5963/Echo&quot;;
        ///var output;
        ///
        ///function init () {
        ///  output = document.getElementById (&quot;output&quot;);
        ///  doWebSocket ();
        ///}
        ///
        ///function doWebSocket () {
        ///  websocket = new WebSocket (url);
        ///
        ///  websocket.onopen = function (e) {
        ///    onOpen (e);
        ///  };
        ///
        ///  websocket.onmessage = function (e) [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string echo {
            get {
                return ResourceManager.GetString("echo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///  &lt;head&gt;
        ///    &lt;title&gt;WebSocket Echo Test&lt;/title&gt;
        ///    &lt;script type=&quot;text/javascript&quot; src=&quot;/Js/echotest.js&quot;&gt;
        ///    &lt;/script&gt;
        ///  &lt;/head&gt;
        ///  &lt;body&gt;
        ///    &lt;h2&gt;WebSocket Echo Test&lt;/h2&gt;
        ///    &lt;div id=&quot;output&quot;&gt;&lt;/div&gt;
        ///  &lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string index {
            get {
                return ResourceManager.GetString("index", resourceCulture);
            }
        }
    }
}
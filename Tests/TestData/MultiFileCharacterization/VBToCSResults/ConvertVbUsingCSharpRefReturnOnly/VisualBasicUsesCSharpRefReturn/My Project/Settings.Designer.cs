﻿// ------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
// Runtime Version:4.0.30319.42000
// 
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.VisualBasic;


namespace VisualBasicUsesCSharpRefReturn.My
{

    [System.Runtime.CompilerServices.CompilerGenerated()]
    [System.CodeDom.Compiler.GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    internal sealed partial class MySettings : System.Configuration.ApplicationSettingsBase
    {

        private static MySettings defaultInstance = (MySettings)Synchronized(new MySettings());

        #region My.Settings Auto-Save Functionality
        /* TODO ERROR: Skipped IfDirectiveTrivia
        #If _MyType = "WindowsForms" Then
        *//* TODO ERROR: Skipped DisabledTextTrivia
                Private Shared addedHandler As Boolean

                Private Shared addedHandlerLockObject As New Object

                <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
                Private Shared Sub AutoSaveSettings(ByVal sender As Global.System.Object, ByVal e As Global.System.EventArgs)
                    If My.Application.SaveMySettingsOnExit Then
                        My.Settings.Save()
                    End If
                End Sub
        *//* TODO ERROR: Skipped EndIfDirectiveTrivia
        #End If
        */
        #endregion

        public static MySettings Default
        {
            get
            {

                /* TODO ERROR: Skipped IfDirectiveTrivia
                #If _MyType = "WindowsForms" Then
                *//* TODO ERROR: Skipped DisabledTextTrivia
                                   If Not addedHandler Then
                                        SyncLock addedHandlerLockObject
                                            If Not addedHandler Then
                                                AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                                                addedHandler = True
                                            End If
                                        End SyncLock
                                    End If
                *//* TODO ERROR: Skipped EndIfDirectiveTrivia
                #End If
                */
                return defaultInstance;
            }
        }
    }
}

namespace VisualBasicUsesCSharpRefReturn.My
{

    [HideModuleName()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.CompilerGenerated()]
    internal static class MySettingsProperty
    {

        [System.ComponentModel.Design.HelpKeyword("My.Settings")]
        internal static MySettings Settings
        {
            get
            {
                return MySettings.Default;
            }
        }
    }
}
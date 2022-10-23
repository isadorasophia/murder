using InstallWizard.Core.Graphics;
using InstallWizard.Data;
using InstallWizard.Util;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.CustomEditors
{
    //[CustomEditorOf(typeof(EditorSettings))]
    internal class EditorSettingsEditor : CustomEditor
    {
        public override object Target => _target;
        private EditorSettings _target = null!;


        internal override ValueTask DrawEditor()
        {

            ImGui.ShowStyleEditor();

            return default;
        }

        internal override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _target = (EditorSettings)target;
        }
    }
}

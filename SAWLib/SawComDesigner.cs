using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace SAWLib
{
     ///////////////////////////////////////////////////////////////// 
    // Designer for the FlowChart control with support for a smart  
    // tag panel. 
    // Must add reference to System.Design.dll 
    /////////////////////////////////////////////////////////////////
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class SawComDesigner : ComponentDesigner
    {
        public override void Initialize(IComponent c)
        {
            base.Initialize(c);
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            properties.Remove("AccessibleDescription");
            properties.Remove("AccessibleName");
            properties.Remove("AccessibleRole");
            properties.Remove("Anchor");
            properties.Remove("AllowDrop");
            properties.Remove("AutoScaleMode");
            properties.Remove("AutoScroll");
            properties.Remove("AutoScrollMargin");
            properties.Remove("AutoScrollMinSize");
            properties.Remove("AutoSize");
            properties.Remove("AutoSizeMode");
            properties.Remove("CausesValidation");
            properties.Remove("ContextMenuStrip");
            properties.Remove("Cursor");
            properties.Remove("ImeMode");
            properties.Remove("UseWaitCursor");
            properties.Remove("AutoValidate");
            properties.Remove("BackgroundImage");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("ApplicationSetting");
            properties.Remove("GenerateMember");
            properties.Remove("RightToLeft");
            properties.Remove("Padding");
            properties.Remove("TabIndex");
            properties.Remove("TabStop");
            properties.Remove("Margin");
            //properties.Remove("Modifiers");
            properties.Remove("MaximumSize");
            properties.Remove("MinimumSize");

            //properties.Remove("BackColor");
            properties.Remove("BorderStyle");
            properties.Remove("ForeColor");
            properties.Remove("Font");
            properties.Remove("Visible");
            properties.Remove("Locked");
            properties.Remove("Dock");
            //properties.Remove("Size");


            base.PreFilterProperties(properties);
        }

        protected override void PreFilterEvents(System.Collections.IDictionary events)
        {
            base.PreFilterEvents(events);

            events.Remove("ContextMenuClosing");
            events.Remove("ContextMenuOpening");
            events.Remove("DataContextChanged");
            events.Remove("DragEnter");
            events.Remove("DragLeave");
            events.Remove("DragOver");
            events.Remove("Drag");
            events.Remove("FocusableChanged");
            events.Remove("GiveFeedback");
            events.Remove("GotFocus");
            events.Remove("GotKeyboardFocus");
            events.Remove("GotMouseCapture");
            events.Remove("GotStylusCapture");
            events.Remove("GotTouchCapture");
            events.Remove("Initialized");
            events.Remove("IsEnabledChanged");
            events.Remove("IsHitTestVisibleChanged");
            events.Remove("IsKeyboardFocusedChanged");
            events.Remove("IsKeyboardFocusWithinChanged");
            events.Remove("IsMouseCapturedChanged");
            events.Remove("IsMouseCaptureWithinChanged");
            events.Remove("IsMouseDirectlyOverChanged");
            events.Remove("IsStylusCapturedChanged");
            events.Remove("IsStylusCaptureWithinChanged");
            events.Remove("IsStylusDirectlyOverChanged");
            events.Remove("IsVisibleChanged");
            events.Remove("KeyDown");
            events.Remove("KeyUp");
            events.Remove("LayoutUpdated");
            events.Remove("Loaded");
            events.Remove("LostFocus");
            events.Remove("LostKeyboardFocus");
            events.Remove("LostMouseCapture");
            events.Remove("LostStylusCapture");
            events.Remove("LostTouchCapture");
            events.Remove("ManipulationBoundaryFeedback");
            events.Remove("ManipulationCompleted");
            events.Remove("ManipulationDelta");
            events.Remove("ManipulationInertiaStarting");
            events.Remove("ManipulationStarted");
            events.Remove("ManipulationStarting");
            events.Remove("MouseDoubleClick");
            events.Remove("MouseDown");
            events.Remove("MouseEnter");
            events.Remove("MouseLeave");
            events.Remove("MouseLeftButtonDown");
            events.Remove("MouseLeftButtonUp");
            events.Remove("MouseMove");
            events.Remove("MouseRightButtonDown");
            events.Remove("MouseRightButtonUp");
            events.Remove("MouseHover");
            events.Remove("MouseUp");
            events.Remove("MouseWheel");
            events.Remove("PreviewDragEnter");
            events.Remove("PreviewDragLeave");
            events.Remove("PreviewDragOver");
            events.Remove("PreviewDrop");
            events.Remove("PreviewGiveFeedback");
            events.Remove("PreviewGotKeyboardFocus");
            events.Remove("PreviewKeyDown");
            events.Remove("PreviewKeyUp");
            events.Remove("PreviewLostKeyboardFocus");
            events.Remove("PreviewMouseDoubleClick");
            events.Remove("PreviewMouseDown");
            events.Remove("PreviewMouseLeftButtonDown");
            events.Remove("PreviewMouseLeftButtonUp");
            events.Remove("PreviewMouseMove");
            events.Remove("PreviewMouseRightButtonDown");
            events.Remove("PreviewMouseRightButtonUp");
            events.Remove("PreviewMouseUp");
            events.Remove("PreviewMouseWheel");
            events.Remove("PreviewQueryContinueDrag");
            events.Remove("PreviewStylusButtonDown");
            events.Remove("PreviewStylusButtonUp");
            events.Remove("PreviewStylusDown");
            events.Remove("PreviewStylusInAirMove");
            events.Remove("PreviewStylusInRange");
            events.Remove("PreviewStylusMove");
            events.Remove("PreviewStylusOutOfRange");
            events.Remove("PreviewStylusSystemGesture");
            events.Remove("PreviewStylusUp");
            events.Remove("PreviewTextInput");
            events.Remove("PreviewTouchDown");
            events.Remove("PreviewTouchMove");
            events.Remove("PreviewTouchUp");
            events.Remove("QueryContinueDrag");
            events.Remove("QueryCursor");
            events.Remove("RequestBringIntoView");
            events.Remove("SizeChanged");
            events.Remove("SourceUpdated");
            events.Remove("StylusButtonDown");
            events.Remove("StylusButtonUp");
            events.Remove("StylusDown");
            events.Remove("StylusEnter");
            events.Remove("StylusInAirMove");
            events.Remove("StylusInRange");
            events.Remove("StylusLeave");
            events.Remove("StylusMove");
            events.Remove("StylusOutOfRange");
            events.Remove("StylusSystemGesture");
            events.Remove("StylusUp");
            events.Remove("TargetUpdated");
            events.Remove("TextInput");
            events.Remove("ToolTipClosing");
            events.Remove("ToolTipOpening");
            events.Remove("TouchDown");
            events.Remove("TouchEnter");
            events.Remove("TouchLeave");
            events.Remove("TouchMove");
            events.Remove("TouchUp");
            events.Remove("Unloaded");
            events.Remove("AutoSizeChanged");
            events.Remove("BackColorChanged");
            events.Remove("BackgroundImageChanged");
            events.Remove("BackgroundImageLayoutChanged");
            events.Remove("BindingContextChanged");
            events.Remove("CausesValidationChanged");
            events.Remove("ChangeUICues");
            //events.Remove("Click");
            events.Remove("ClientSizeChanged");
            events.Remove("ContextMenuStripChanged");
            events.Remove("ControlAdded");
            events.Remove("ControlRemoved");
            events.Remove("CursorChanged");
            events.Remove("DockChanged");
            events.Remove("DoubleClick");
            events.Remove("DragDrop");
            events.Remove("EnabledChanged");
            events.Remove("Enter");
            events.Remove("FontChanged");
            events.Remove("ForeColorChanged");
            events.Remove("HelpRequested");
            events.Remove("ImeModeChanged");
            events.Remove("Layout");
            events.Remove("Leave");
            events.Remove("LocationChanged");
            events.Remove("MarginChanged");
            events.Remove("MouseCaptureChanged");
            events.Remove("MouseClick");
            events.Remove("Move");
            events.Remove("PaddingChanged");
            events.Remove("Paint");
            events.Remove("ParentChanged");
            events.Remove("QueryAccessibilityHelp");
            events.Remove("RegionChanged");
            events.Remove("Resize");
            events.Remove("RightToLeftChanged");
            events.Remove("Scroll");
            events.Remove("StyleChanged");
            events.Remove("SystemColorsChanged");
            events.Remove("TabIndexChanged");
            events.Remove("TabStopChanged");
            events.Remove("Validated");
            events.Remove("Validating");
            events.Remove("VisibleChanged");

        }

    }
}

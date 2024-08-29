using UnityEditor;
using UnityEngine;

namespace Reflex.Editor.DebuggingWindow
{
    public static class Styles
    {
        private static GUIStyle _richTextLabel;
        public static GUIStyle RichTextLabel
        {
            get
            {
                if (_richTextLabel == null)
                {
                    _richTextLabel = new GUIStyle(EditorStyles.label)
                    {
                        richText = true
                    };
                }

                return _richTextLabel;
            }
        }
        
        private static GUIStyle _stackTrace;
        public static GUIStyle StackTrace
        {
            get
            {
                if (_stackTrace == null)
                {
                    _stackTrace = new GUIStyle("CN Message");
                    _stackTrace.wordWrap = false;
                }

                return _stackTrace;
            }
        }
    
        private static GUIStyle _labelHorizontallyCentered;
        public static GUIStyle LabelHorizontallyCentered
        {
            get
            {
                if (_labelHorizontallyCentered == null)
                {
                    _labelHorizontallyCentered = new GUIStyle("Label");
                    _labelHorizontallyCentered.alignment = TextAnchor.MiddleCenter;
                }

                return _labelHorizontallyCentered;
            }
        }
    
        private static GUIStyle _appToolbar;
        public static GUIStyle AppToolbar
        {
            get
            {
                if (_appToolbar == null)
                {
                    _appToolbar = new GUIStyle("AppToolbar");
                }

                return _appToolbar;
            }
        }
    
        private static GUIStyle _statusBarIcon;
        public static GUIStyle StatusBarIcon
        {
            get
            {
                if (_statusBarIcon == null)
                {
                    _statusBarIcon = new GUIStyle("StatusBarIcon");
                }

                return _statusBarIcon;
            }
        }
    
        private static GUIStyle _hyperlink;
        public static GUIStyle Hyperlink
        {
            get
            {
                if (_hyperlink == null)
                {
                    _hyperlink = new GUIStyle(EditorStyles.linkLabel);
                    _hyperlink.wordWrap = false;
                }

                return _hyperlink;
            }
        }
    }
}
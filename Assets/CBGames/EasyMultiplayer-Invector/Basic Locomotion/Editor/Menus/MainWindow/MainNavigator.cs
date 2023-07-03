using EMI.Utils;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    public class MainNavigator : EditorWindow
    {
        #region Properties
        protected bool showProgressBar = false;
        private Texture2D _titleBar = null;
        protected Texture2D titleBar
        {
            get
            {
                return _titleBar;
            }
            set
            {
                _titleBar = value;
                if (viewTitleBackground != null)
                {
                    viewTitleBackground.normal.background = _titleBar;
                }
            }
        }
        protected Texture2D rightViewBackground = null;
        protected float titleBarSize;
        protected GUIStyle rightView, leftPanel, divider, viewTitleBackground, viewTitleLable, leftButton, labels = null;
        protected Color orgColor, clearColor;
        protected string rightViewTitle;
        #endregion

        #region Initializations
        protected virtual void OnEnable()
        {
            rightViewTitle = "Easy Multiplayer - Invector";
            SetupGUIStyle();
        }

        // Will reset the window back to its original settings
        protected virtual void ResetStyles()
        {
            Font editorFont = Resources.Load<Font>("Assassin");
            titleBarSize = 75f;
            titleBar = Resources.Load<Texture2D>("NavigatorTitleBar");
            rightViewBackground = Resources.Load<Texture2D>("RightViewBackground");

            orgColor = GUI.color;
            clearColor = GUI.color;
            clearColor.a = 0.1f;

            rightView = new GUIStyle();
            rightView.normal.background = EditorUtils.MakeTex(new Color(35 / 255F, 39 / 255F, 42 / 255F));
            rightView.font = editorFont;

            leftPanel = new GUIStyle();
            leftPanel.normal.background = EditorUtils.MakeTex(new Color(44 / 255F, 47 / 255F, 51 / 255F));
            leftPanel.font = editorFont;

            divider = new GUIStyle();
            divider.normal.background = EditorUtils.MakeTex(Color.black);

            try
            {
                viewTitleLable = new GUIStyle(EditorStyles.boldLabel);
                viewTitleLable.alignment = TextAnchor.MiddleCenter;
                viewTitleLable.fontSize = 24;
                viewTitleLable.font = editorFont;
            }
            catch { }

            try
            {
                leftButton = new GUIStyle(EditorStyles.miniButton);
                leftButton.alignment = TextAnchor.MiddleCenter;
                leftButton.fontSize = 16;
                leftButton.hover.textColor = Color.white;
                leftButton.font = editorFont;
            }
            catch { }

            try
            {
                labels = new GUIStyle(EditorStyles.label);
                labels.normal.textColor = Color.white;
                labels.onNormal.textColor = Color.white;
                labels.hover.textColor = Color.white;
                labels.onHover.textColor = Color.white;
                labels.focused.textColor = Color.white;
                labels.onFocused.textColor = Color.white;
                labels.active.textColor = Color.white;
                labels.onActive.textColor = Color.white;
                labels.richText = true;
            }
            catch { }
        }

        // Will set the color theme for everything that is used in this editor window
        protected virtual void SetupGUIStyle()
        {
            viewTitleBackground = new GUIStyle();
            ResetStyles();
        }
        #endregion

        #region Drawing GUI
        public virtual void OnInspectorUpdate()
        {
            Repaint();
        }

        // Draws all the GUI elements 
        protected virtual void OnGUI()
        {
            LeftPanel();
            Divider();
            RightView();

        }
        #endregion

        #region Formattings Functions
        // Draws everything in the left panel
        protected void LeftPanel()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width / 4, position.height), leftPanel);
            Color orgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(153 / 255F, 170 / 255F, 181 / 255F);
            LeftPanelContents();
            GUI.backgroundColor = orgColor;
            GUILayout.EndArea();
        }

        // Draws a line between the panel and the view
        protected void Divider()
        {
            GUILayout.BeginArea(new Rect((position.width / 4), 0, 2, position.height), divider);
            GUILayout.EndArea();
        }

        // Draws everything in the right view
        protected void RightView()
        {
            GUILayout.BeginArea(new Rect((position.width / 4)+2, 0, (position.width - (position.width / 4))-2, position.height), rightView);
            DrawTitle();
            DrawRightViewContents();
            GUILayout.EndArea();
        }

        // Draws the title bar
        protected virtual void DrawTitle()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width - (position.width/4)-2, titleBarSize), viewTitleBackground);
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(rightViewTitle, viewTitleLable);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }

        // Draws the active contents of the right view under the title
        protected virtual void DrawRightViewContents()
        {
            GUILayout.BeginArea(new Rect(0, titleBarSize, position.width-(position.width/4)-2, position.height - (titleBarSize+20)), rightView);
            GUI.color = clearColor;
            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), rightViewBackground, ScaleMode.StretchToFill);
            GUI.color = orgColor;
            RightViewContents();
            GUILayout.EndArea();
        }
        #endregion

        #region Override For Content Functions
        protected virtual string YouTubePlayList()
        {
            return "https://www.youtube.com/channel/UCnmqZ8pqQlB9zXlGqjTK_Ag/playlists";
        }
        protected virtual string DocumentationWebsite()
        {
            return "https://cyberbulletgames.com/easy-multiplayer-invector-docs/";
        }
        protected virtual void RightViewContents() { }
        protected virtual void LeftPanelContents() { }
        #endregion
    }
}

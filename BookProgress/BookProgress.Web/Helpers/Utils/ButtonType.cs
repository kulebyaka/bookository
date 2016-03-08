namespace BookProgress.Web.Helpers.Utils
{       
    public class ButtonType
        {
            public string Icon { get; private set; }
            public string Class { get; private set; }

            public static readonly ButtonType Add = new ButtonType("plus", "success");

            public static readonly ButtonType Default = new ButtonType(null, "primary");

            public static readonly ButtonType Deactivate = new ButtonType(null, "danger");

            public static readonly ButtonType Ok = new ButtonType("ok", "success");

            public static readonly ButtonType SearchField = new ButtonType("search", "default");

            public static readonly ButtonType Search = new ButtonType("search", "success");

            public static readonly ButtonType Remove = new ButtonType("remove", "default");

            private ButtonType(string glyphiconGlyphiconPlus, string btnBtnSuccess)
            {
                Icon = glyphiconGlyphiconPlus;
                Class = btnBtnSuccess;
            }
        }
}
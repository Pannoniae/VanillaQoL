using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

#pragma warning disable // tml shitcode anyway

namespace VanillaQoL.API;

public class FloatInputElement : ConfigElement {
    public List<float> floatList { get; set; }

    public float Min { get; set; }

    public float Max { get; set; } = 1;

    public float Increment { get; set; } = 0.01f;

    public override void OnBind() {
        base.OnBind();
        floatList = (List<float>)List;
        if (floatList != null)
            TextDisplayFunction = () => {
                float num = Index + 1;
                string str1 = num.ToString();
                num = floatList[Index];
                string str2 = num.ToString();
                return str1 + ": " + str2;
            };
        if (RangeAttribute != null && RangeAttribute.Min is float && RangeAttribute.Max is float) {
            Min = (float)RangeAttribute.Min;
            Max = (float)RangeAttribute.Max;
        }

        if (IncrementAttribute != null && IncrementAttribute.Increment is float)
            Increment = (float)IncrementAttribute.Increment;
        UIPanel element1 = new UIPanel();
        element1.SetPadding(0.0f);
        UIFocusInputTextField uIInputTextField = new UIFocusInputTextField("Type here");
        element1.Top.Set(0.0f, 0.0f);
        element1.Left.Set(-190f, 1f);
        element1.Width.Set(180f, 0.0f);
        element1.Height.Set(30f, 0.0f);
        Append(element1);
        uIInputTextField.SetText(GetValue().ToString());
        uIInputTextField.Top.Set(5f, 0.0f);
        uIInputTextField.Left.Set(10f, 0.0f);
        uIInputTextField.Width.Set(-42f, 1f);
        uIInputTextField.Height.Set(20f, 0.0f);
        uIInputTextField.OnTextChange += (a, b) => {
            float result;
            if (!float.TryParse(uIInputTextField.CurrentString, out result))
                return;
            SetValue(result);
        };
        uIInputTextField.OnUnfocus +=
            (a, b) => uIInputTextField.SetText(GetValue().ToString());
        element1.Append(uIInputTextField);
        UIModConfigHoverImageSplit element2 =
            new UIModConfigHoverImageSplit(UpDownTexture, "+" + Increment, "-" + Increment);
        element2.Recalculate();
        element2.Top.Set(4f, 0.0f);
        element2.Left.Set(-30f, 1f);
        element2.OnLeftClick += (MouseEvent)((a, b) => {
            Rectangle rectangle = b.GetDimensions().ToRectangle();
            if ((double)a.MousePosition.Y < rectangle.Y + rectangle.Height / 2)
                SetValue(Terraria.Utils.Clamp(GetValue() + Increment, Min, Max));
            else
                SetValue(Terraria.Utils.Clamp(GetValue() - Increment, Min, Max));
            uIInputTextField.SetText(GetValue().ToString("0.###", CultureInfo.CurrentCulture));
        });
        element1.Append(element2);
        Recalculate();
    }

    protected virtual float GetValue() => (float)GetObject();

    protected virtual void SetValue(float value) {
        SetObject(Terraria.Utils.Clamp(value, Min, Max));
    }
}

public class UIFocusInputTextField : UIElement {
    internal bool Focused;
    internal string CurrentString = "";
    private readonly string _hintText;
    private int _textBlinkerCount;
    private int _textBlinkerState;

    public bool UnfocusOnTab { get; internal set; }

    public event EventHandler OnTextChange;

    public event EventHandler OnUnfocus;

    public event EventHandler OnTab;

    public UIFocusInputTextField(string hintText) => _hintText = hintText;

    public void SetText(string text) {
        if (text == null)
            text = "";
        if (!(CurrentString != text))
            return;
        CurrentString = text;
        EventHandler onTextChange = OnTextChange;
        if (onTextChange == null)
            return;
        onTextChange(this, new EventArgs());
    }

    public override void LeftClick(UIMouseEvent evt) {
        Main.clrInput();
        Focused = true;
    }

    public override void Update(GameTime gameTime) {
        if (!ContainsPoint(new Vector2(Main.mouseX, Main.mouseY)) && Main.mouseLeft) {
            Focused = false;
            EventHandler onUnfocus = OnUnfocus;
            if (onUnfocus != null)
                onUnfocus(this, new EventArgs());
        }

        base.Update(gameTime);
    }

    private static bool JustPressed(Keys key) {
        return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        if (Focused) {
            PlayerInput.WritingText = true;
            Main.instance.HandleIME();
            string inputText = Main.GetInputText(CurrentString);
            if (!inputText.Equals(CurrentString)) {
                CurrentString = inputText;
                EventHandler onTextChange = OnTextChange;
                if (onTextChange != null)
                    onTextChange(this, new EventArgs());
            }
            else
                CurrentString = inputText;

            if (JustPressed(Keys.Tab)) {
                if (UnfocusOnTab) {
                    Focused = false;
                    EventHandler onUnfocus = OnUnfocus;
                    if (onUnfocus != null)
                        onUnfocus(this, new EventArgs());
                }

                EventHandler onTab = OnTab;
                if (onTab != null)
                    onTab(this, new EventArgs());
            }

            if (++_textBlinkerCount >= 20) {
                _textBlinkerState = (_textBlinkerState + 1) % 2;
                _textBlinkerCount = 0;
            }
        }

        string currentString = CurrentString;
        if (_textBlinkerState == 1 && Focused)
            currentString += "|";
        CalculatedStyle dimensions = GetDimensions();
        if (CurrentString.Length == 0 && !Focused)
            Terraria.Utils.DrawBorderString(spriteBatch, _hintText, new Vector2(dimensions.X, dimensions.Y),
                Color.Gray);
        else
            Terraria.Utils.DrawBorderString(spriteBatch, currentString, new Vector2(dimensions.X, dimensions.Y),
                Color.White);
    }

    public delegate void EventHandler(object sender, EventArgs e);
}

public class UIModConfigHoverImageSplit : UIImage {
    public string HoverTextUp;
    public string HoverTextDown;

    public UIModConfigHoverImageSplit(
        Asset<Texture2D> texture,
        string hoverTextUp,
        string hoverTextDown)
        : base(texture) {
        HoverTextUp = hoverTextUp;
        HoverTextDown = hoverTextDown;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        base.DrawSelf(spriteBatch);
        Rectangle rectangle = GetDimensions().ToRectangle();
        if (!IsMouseHovering)
            return;
        var uiModConfig = typeof(UIImage).Assembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig");
        if (Main.mouseY < rectangle.Y + rectangle.Height / 2) {
            uiModConfig.GetProperty("Tooltip", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .SetValue(null, HoverTextUp);
        }
        else {
            uiModConfig.GetProperty("Tooltip", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .SetValue(null, HoverTextDown);
        }
    }
}
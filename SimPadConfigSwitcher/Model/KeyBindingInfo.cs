using SimPadController.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimPadConfigSwitcher.Model
{
    public class KeyBindingInfo
    {
        public ModifierKeys Modifiers
        {
            get
            {
                ModifierKeys ret = ModifierKeys.None;
                foreach(var i in SModifiers)
                {
                    ret |= i;
                }

                return ret;
            }
            set
            {
                List<ModifierKeys> result = new List<ModifierKeys>();

                foreach(ModifierKeys i in Enum.GetValues(typeof(ModifierKeys)))
                {
                    if (i == ModifierKeys.None) continue;

                    if((value & i) == i)
                    {
                        result.Add(i);
                    }
                }

                SModifiers = result.ToArray();
            }
        }

        private ModifierKeys[] SModifiers;

        public Key NormalKey;

        public SimPadController.Model.KeySetting SimPadKeySetting = new SimPadController.Model.KeySetting();

        public override string ToString()
        {
            if(SModifiers == null && NormalKey == Key.None)
            {
                List<SimPadKeySpecial> specials = new List<SimPadKeySpecial>();
                foreach(SimPadKeySpecial i in Enum.GetValues(typeof(SimPadKeySpecial)))
                {
                    if (i == SimPadKeySpecial.None) continue;

                    if((i & this.SimPadKeySetting.Special) == i)
                    {
                        specials.Add(i);
                    }
                }

                string r = String.Join(" + ", specials);
                
                if(this.SimPadKeySetting.Normal != SimPadKeyNormal.None)
                {
                    r += this.SimPadKeySetting.Normal;
                }

                return r;
            }

            string ret = String.Join(" + ", SModifiers.Select(i => SpecialKeyToSimPadKey(i)));

            if(ret != String.Empty && NormalKey != Key.None)
            {
                ret += " + ";
            }

            if(NormalKey != Key.None)
            {
                ret += KeyToSimPadKey(NormalKey).ToString();
            }

            return ret;
        }

        public void Apply()
        {
            SimPadKeySpecial special = SimPadKeySpecial.None;

            if (SModifiers != null) return;

            foreach(var i in SModifiers)
            {
                special |= SpecialKeyToSimPadKey(i);
            }

            SimPadKeyNormal normal = KeyToSimPadKey(NormalKey);

            this.SimPadKeySetting.Normal = normal;
            this.SimPadKeySetting.Special = special;

            this.SModifiers = null;
            this.NormalKey = Key.None;
        }

        private SimPadKeySpecial SpecialKeyToSimPadKey(ModifierKeys k)
        {
            switch(k)
            {
                case ModifierKeys.Control:
                    return SimPadKeySpecial.LeftCtrl;
                case ModifierKeys.Alt:
                    return SimPadKeySpecial.LeftAlt;
                case ModifierKeys.Windows:
                    return SimPadKeySpecial.LeftWin;
                case ModifierKeys.Shift:
                    return SimPadKeySpecial.LeftShift;
            }

            return SimPadKeySpecial.None;
        }

        private SimPadKeyNormal KeyToSimPadKey(Key k)
        {
            if(k >= Key.A && k <= Key.Z)
            {
                return (k - Key.A + SimPadKeyNormal.A);
            }

            if(k >= Key.D1 && k <= Key.D9)
            {
                return (k - Key.D1 + SimPadKeyNormal.Num1);
            }

            if(k >= Key.NumPad1 && k <= Key.NumPad9)
            {
                return (k - Key.NumPad1 + SimPadKeyNormal.NumPad1);
            }

            if(k >= Key.F1 && k <= Key.F12)
            {
                return (k - Key.F1 + SimPadKeyNormal.F1);
            }

            switch(k)
            {
                case Key.NumPad0:
                    return SimPadKeyNormal.NumPad0;
                case Key.D0:
                    return SimPadKeyNormal.Num0;
                case Key.Escape:
                    return SimPadKeyNormal.Esc;
                case Key.PrintScreen:
                    return SimPadKeyNormal.PrtSc;
                case Key.Scroll:
                    return SimPadKeyNormal.ScrLk;
                case Key.Pause:
                    return SimPadKeyNormal.PauseBreak;
                case Key.OemTilde:
                    return SimPadKeyNormal.WaveLine;
                case Key.OemMinus:
                    return SimPadKeyNormal.Bar;
                case Key.OemPlus:
                    return SimPadKeyNormal.Equal;
                case Key.Back:
                    return SimPadKeyNormal.Bsp;
                case Key.Insert:
                    return SimPadKeyNormal.Ins;
                case Key.Home:
                    return SimPadKeyNormal.Home;
                case Key.PageUp:
                    return SimPadKeyNormal.PageUp;
                case Key.NumLock:
                    return SimPadKeyNormal.NumPadNumLk;
                case Key.Divide:
                    return SimPadKeyNormal.NumPadSlash;
                case Key.Multiply:
                    return SimPadKeyNormal.NumPadAsterisk;
                case Key.Subtract:
                    return SimPadKeyNormal.NumPadBar;
                case Key.Delete:
                    return SimPadKeyNormal.Del;
                case Key.End:
                    return SimPadKeyNormal.End;
                case Key.PageDown:
                    return SimPadKeyNormal.PgDn;
                case Key.Tab:
                    return SimPadKeyNormal.Tab;
                case Key.OemOpenBrackets:
                    return SimPadKeyNormal.LeftParenthesis;
                case Key.OemCloseBrackets:
                    return SimPadKeyNormal.RightParenthesis;
                case Key.OemPipe:
                    return SimPadKeyNormal.BackSlant;
                case Key.CapsLock:
                    return SimPadKeyNormal.CapsLock;
                case Key.OemSemicolon:
                    return SimPadKeyNormal.Semicolon;
                case Key.OemQuotes:
                    return SimPadKeyNormal.QuotationMark;
                case Key.Enter:
                    return SimPadKeyNormal.Enter;
                case Key.OemComma:
                    return SimPadKeyNormal.Comma;
                case Key.OemPeriod:
                    return SimPadKeyNormal.Period;
                case Key.OemQuestion:
                    return SimPadKeyNormal.QuestionMark;
                case Key.Space:
                    return SimPadKeyNormal.Space;
                case Key.Up:
                    return SimPadKeyNormal.UpArrow;
                case Key.Down:
                    return SimPadKeyNormal.DownArrow;
                case Key.Left:
                    return SimPadKeyNormal.LeftArrow;
                case Key.Right:
                    return SimPadKeyNormal.RightArrow;
                case Key.Apps:
                    return SimPadKeyNormal.Menu;
                case Key.Add:
                    return SimPadKeyNormal.NumPadAdd;
                case Key.Decimal:
                    return SimPadKeyNormal.NumPadPeriod;

            }

            return SimPadKeyNormal.None;
        }
    }
}

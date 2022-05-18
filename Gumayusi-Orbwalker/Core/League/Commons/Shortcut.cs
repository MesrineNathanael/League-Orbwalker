using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Gumayusi_Orbwalker.Core.League.Commons
{
    public class Shortcut
    {
        public List<Key> Keys;

        public string Text1 = "";

        public string Text2 = "";

        public string Var1 = "";

        public string Var2 = "";

        public bool LastKeyNeedToBeUp = true;

        /// <summary>
        /// Return a the result in format "{Text1} {Var1} {Text2} {Var2}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Text1 != "") stringBuilder.Append(Text1);
            if (Var1 != "") stringBuilder.Append(" " + Var1);
            if (Text2 != "") stringBuilder.Append(" " + Text2);
            if (Var2 != "") stringBuilder.Append(" " + Var2);
            return stringBuilder.ToString();
        }
    }
}

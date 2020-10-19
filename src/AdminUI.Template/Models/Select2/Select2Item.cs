using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.AdminUI.Template.Models.Select2
{
    public class Select2ItemBase<TKey>
    {
        public TKey Id { get; set; }
        public string Text { get; set; }

        public Select2ItemBase(TKey id, string text)
        {
            Id = id;
            Text = text;
        }
    }

    public class Select2ItemBase : Select2ItemBase<string>
    {
        public Select2ItemBase(string id, string text) : base(id, text)
        {
        }
    }

    public class Select2Item<TKey> : Select2ItemBase<TKey>
    {
        public bool Selected { get; set; }
        public bool Disabled { get; set; }

        public Select2Item(TKey id, string text, bool selected, bool disabled) : base(id, text)
        {
            Selected = selected;
            Disabled = disabled;
        }
    }

    public class Select2Item : Select2Item<string>
    {
        public Select2Item(string id, string text, bool selected, bool disabled) : base(id, text, selected, disabled)
        {
        }

        public Select2Item(string id, string text) : base(id, text, false, false)
        {
        }
    }
}

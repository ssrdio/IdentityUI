namespace SSRD.AdminUI.Template.Models
{
    public class DragAndDropItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool Disabled { get; set; }

        public DragAndDropItem()
        {
        }

        public DragAndDropItem(string text)
        {
            Id = text;
            Text = text;
            Disabled = false;
        }

        public DragAndDropItem(string id, string text)
        {
            Id = id;
            Text = text;
            Disabled = false;
        }

        public DragAndDropItem(string text, bool disabled)
        {
            Id = text;
            Text = text;
            Disabled = disabled;
        }

        public DragAndDropItem(string id, string text, bool disabled)
        {
            Id = id;
            Text = text;
            Disabled = disabled;
        }
    }
}

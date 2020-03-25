namespace Zergatul.FileFormat.Pdf
{
    public class PageActions
    {
        public object OnOpen { get; }
        public object OnClose { get; }

        public PageActions(object onOpen, object onClose)
        {
            OnOpen = onOpen;
            OnClose = onClose;
        }
    }
}
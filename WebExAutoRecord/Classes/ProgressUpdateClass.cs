namespace PoliDLGUI.Forms
{
    public class ProgressUpdateClass
    {
        public Enums.ProgressUpdate progressUpdate;
        public bool retry;

        public ProgressUpdateClass(Enums.ProgressUpdate progressUpdate, bool retry)
        {
            this.progressUpdate = progressUpdate;
            this.retry = retry;
        }
    }
}
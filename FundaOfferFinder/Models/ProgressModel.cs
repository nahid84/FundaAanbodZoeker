namespace Models
{
    public class ProgressModel
    {
        public int Total { get; set; }
        public int Current { get; set; }
        public int Reminder
        {
            get
            {
                return Total - Current;
            }
        }
    }
}

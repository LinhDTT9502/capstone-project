namespace _2Sport_BE.ViewModels
{
    public class SportDTO
    {
        public string Name { get; set; }
    }
    public class SportVM : SportDTO
    {
        public int Id { get; set; }
        public bool Status { get; set; }
    }

    public class SportCM : SportDTO
    {
    }

    public class SportUM : SportDTO
    {
    }
}

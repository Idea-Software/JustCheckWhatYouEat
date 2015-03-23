namespace JustCheckWhatYouEat.Api.Models
{

    public class FoodPicture
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int Relevance { get; set; }
        public bool IsUserUploaded { get; set; }
    }
}
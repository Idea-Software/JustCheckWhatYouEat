using System.Collections.Generic;

namespace JustCheckWhatYouEat.Api.Models
{

    public class FoodInfo
    {
        public string Category { get; set; }
        public string Food { get; set; }
        public string Description { get; set; }
        public List<FoodPicture> Pictures { get; set; }
    }

}
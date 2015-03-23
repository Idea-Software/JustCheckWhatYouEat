using Microsoft.WindowsAzure.Storage.Table;

namespace JustCheckWhatYouEat.Api.Models
{

    public class FoodEntity : TableEntity
    {
        public FoodEntity(string category, string food)
        {
            this.PartitionKey = category;
            this.RowKey = food;

            this.Category = category;
            this.Food = food;
        }

        public FoodEntity() { }

        public string Category { get; set; }
        public string Food { get; set; }
        public string Data { get; set; }
    }
}
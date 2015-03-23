using System;
using System.Configuration;
using System.Linq;
using JustCheckWhatYouEat.Api.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace JustCheckWhatYouEat.Api.DataAccess
{

    public interface IFoodRepo
    {
        FoodInfo Get(string category, string food);
        void Store(FoodInfo food);
        void Update(FoodInfo food);
        bool TryVote(string category, string food, int pictureId, bool voteUp);
    }

    public class FoodRepo : IFoodRepo
    {
        public FoodInfo Get(string category, string food)
        {
            var table = GetTable();

            var entity = table.Execute(TableOperation.Retrieve<FoodEntity>(category, food)).Result as FoodEntity;
            if (entity == null)
                return null;
            
            return JsonConvert.DeserializeObject<FoodInfo>(entity.Data);
        }
        /// <summary>
        /// Store new record. Throws if conflicting record already exists.
        /// </summary>
        /// <param name="foodInfo"></param>
        public void Store(FoodInfo foodInfo)
        {
            var table = GetTable();

            var entity = new FoodEntity(foodInfo.Category, foodInfo.Food)
            {
                Data = JsonConvert.SerializeObject(foodInfo)
            };
            table.Execute(TableOperation.Insert(entity));
        }

        
        public void Update(FoodInfo foodInfo)
        {
            var table = GetTable();

            var entity = table.Execute(TableOperation.Retrieve<FoodEntity>(foodInfo.Category, foodInfo.Food)).Result as FoodEntity;
            
            // ReSharper disable once PossibleNullReferenceException
            entity.Data = JsonConvert.SerializeObject(foodInfo);

            table.Execute(TableOperation.Replace(entity));
        }

        public bool TryVote(string category, string food, int pictureId, bool voteUp)
        {
            var table = GetTable();

            var entity = table.Execute(TableOperation.Retrieve<FoodEntity>(category, food)).Result as FoodEntity;

            // ReSharper disable once PossibleNullReferenceException
            var foodInfo = JsonConvert.DeserializeObject<FoodInfo>(entity.Data);

            foodInfo.Pictures.Single(p => p.Id == pictureId).Relevance += voteUp ? 1 : -1;

            entity.Data = JsonConvert.SerializeObject(foodInfo);
            try
            {
                table.Execute(TableOperation.Replace(entity));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        private CloudTable GetTable()
        {
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("food");
            return table;
        }

        ////Only uncomment for maintenance
        //public static void DELETE_ALL_DATA_IN_TABLE()
        //{
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
        //    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        //    CloudTable table = tableClient.GetTableReference("food");
        //    table.DeleteIfExists();
        //}
    }
}
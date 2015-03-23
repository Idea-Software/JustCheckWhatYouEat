using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using JustCheckWhatYouEat.Api.DataAccess;
using JustCheckWhatYouEat.Api.Models;


namespace JustCheckWhatYouEat.Api.Controllers
{

    [EnableCors("https://www.just-eat.co.uk, http://www.just-eat.co.uk", "*", "*")]
    public class FoodController : ApiController
    {
        public IFoodRepo FoodRepo { get; set; }
        public IBingFacade BingFacade { get; set; }

        public FoodController(IFoodRepo foodRepo, IBingFacade bingFacade)
        {
            FoodRepo = foodRepo;
            BingFacade = bingFacade;
        }


        [HttpGet]
        [Route("category/{category}/food/{food}")]
        public FoodInfo FindFoodInfo(string category, string food)
        {
            category = category.ToLower();
            food = food.ToLower();

            var foodInfo = FoodRepo.Get(category, food);

            if (foodInfo == null)
            {
                return CreateNewFood(category, food);
            }

            foodInfo.Pictures = foodInfo.Pictures.OrderByDescending(p => p.Relevance).ToList();

            return foodInfo;
        }


        
        private FoodInfo CreateNewFood(string category, string food)
        {
            var pictureUrls = BingFacade.FindImages(String.Format("{0} - {1}", category, food));


            var pictureId = 0;
            var pictures = new List<FoodPicture>();
            foreach(var url in pictureUrls)
            {
                pictures.Add(new FoodPicture
                {
                    Id = pictureId,
                    Url = url,
                    Relevance = 0,
                    IsUserUploaded = false
                });
                pictureId++;
            }

            var newFood = new FoodInfo
            {
                Category = category,
                Food = food,
                Pictures = pictures
            };

            FoodRepo.Store(newFood);
            return newFood;
        }


        ////Only uncomment for maintenance
        //[HttpDelete]
        //[HeaderBasedRoute("kill_all_data", "Confirm-Action", "Delete")]
        //public void Delete()
        //{
        //    DataAccess.FoodRepo.DELETE_ALL_DATA_IN_TABLE();
        //}
    }
}

using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using IdeaSoftware.Utils.WebApi.Routing;
using JustCheckWhatYouEat.Api.DataAccess;

namespace JustCheckWhatYouEat.Api.Controllers
{
    [EnableCors("https://www.just-eat.co.uk, http://www.just-eat.co.uk", "*", "*", "Idea.JCWYE.Vote")]
    public class PictureController : ApiController
    {
        public IFoodRepo FoodRepo { get; set; }

        public PictureController(IFoodRepo foodRepo)
        {
            FoodRepo = foodRepo;
        }


        [HttpPatch]
        [HeaderBasedRoute("category/{category}/food/{food}/picture/{id}", "Idea.JCWYE.Vote", "Up")]
        [HeaderBasedRoute("category/{category}/food/{food}/picture/{id}", "Idea.JCWYE.Vote", "Down")]
        public IHttpActionResult Vote(string category, string food, int id)
        {
            var vote = Request.Headers.GetValues("Idea.JCWYE.Vote").First().ToLower() == "up";

            var timeToWait =
                TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["JCWYE:MaxTimeToWaitForVoteInSeconds"]));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!FoodRepo.TryVote(category, food, id, vote))
            {
                if (sw.Elapsed > timeToWait)
                    return InternalServerError();
            }

            return Ok();
        }

    }
}
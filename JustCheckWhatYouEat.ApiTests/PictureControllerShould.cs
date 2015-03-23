using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Common;
using JustCheckWhatYouEat.Api.Controllers;
using JustCheckWhatYouEat.Api.DataAccess;
using JustCheckWhatYouEat.Api.Models;
using Moq;
using NUnit.Framework;

namespace JustCheckWhatYouEat.ApiTests
{
    [TestFixture]
    public class PictureControllerShould
    {
        private Mock<IFoodRepo> _foodRepo;
        private PictureController _controller;

        private const string Category = "drinks";
        private const string Food = "beer";
        private const int Id = 3;

        private FoodInfo _foodInfo;
        private readonly List<string> _urls = new List<string>
        {
            "http://idea-software.net/jcwye/images/0.png",
            "http://idea-software.net/jcwye/images/1.png",
            "http://idea-software.net/jcwye/images/2.png",
            "http://idea-software.net/jcwye/images/3.png"
        };

        [SetUp]
        public void TestSetUp()
        {
            _foodRepo = new Mock<IFoodRepo>();
            _controller = new PictureController(_foodRepo.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Request.Headers.Add("Idea.JCWYE.Vote", "Up");
            ConfigurationManager.AppSettings["JCWYE:MaxTimeToWaitForVoteInSeconds"] = "1";

            _foodInfo = new FoodInfo
            {
                Category = Category,
                Food = Food,
                Description = String.Empty,
                Pictures = _urls.Select((item, i) => new FoodPicture { Id = i, Url = item }).ToList()
            };
        }

        [Test]
        public void CallTryVoteAtLeastOnce_OnVote()
        {
            _foodRepo.Setup(m => m.TryVote(Category, Food, Id, true)).Returns(true);

            _controller.Vote(Category, Food, Id);

            _foodRepo.Verify(m => m.TryVote(Category, Food, Id, true), Times.Once);
        }

        [Test]
        public void ReturnOkWhenTryVoteReturnsTrue_OnVote()
        {
            _foodRepo.Setup(m => m.TryVote(Category, Food, Id, true)).Returns(true);

            var result = _controller.Vote(Category, Food, Id).ExecuteAsync(default(CancellationToken)).Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ReturnErrorWhenTryVoteFailsForTooLong_OnVote()
        {
            _foodRepo.Setup(m => m.TryVote(Category, Food, Id, true)).Callback<string,string,int,bool>((c, f, i, v) =>
            {
                Thread.Sleep(300);
            }).Returns(false);

            var result = _controller.Vote(Category, Food, Id).ExecuteAsync(default(CancellationToken)).Result;

            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using JustCheckWhatYouEat.Api.Controllers;
using JustCheckWhatYouEat.Api.DataAccess;
using JustCheckWhatYouEat.Api.Models;
using Moq;
using NUnit.Framework;

namespace JustCheckWhatYouEat.ApiTests
{
    [TestFixture]
    public class FoodControllerShould
    {
        private Mock<IFoodRepo> _foodRepo;
        private Mock<IBingFacade> _bingFacade;
        private FoodController _controller;

        private const string Category = "drinks";
        private const string Food = "beer";

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
            _bingFacade = new Mock<IBingFacade>();
            _controller = new FoodController(_foodRepo.Object, _bingFacade.Object);

            _foodInfo = new FoodInfo
            {
                Category = Category,
                Food = Food,
                Description = String.Empty,
                Pictures =  _urls.Select((item, i) => new FoodPicture{Id = i, Url = item}).ToList()
            };
        }

        [Test]
        public void RetrieveFoodInfoFromAzureTable_OnFindFoodInfo()
        {
            _foodRepo.Setup(m => m.Get(Category, Food)).Returns(_foodInfo);

            var info = _controller.FindFoodInfo(Category, Food);

            info.Should().Be(_foodInfo);
            _foodRepo.Verify(m => m.Get(Category, Food), Times.Once);
        }

        [Test]
        public void CreateNewFood_WhenNoDataInAzureTable_OnFindFoodInfo()
        {
            _foodRepo.Setup(m => m.Get(Category, Food)).Returns<FoodInfo>(null);
            _bingFacade.Setup(m => m.FindImages(It.IsAny<string>())).Returns(_urls);


            var info = _controller.FindFoodInfo(Category, Food);

            info.Pictures.Should().HaveCount(_urls.Count);
            _foodRepo.Verify(m => m.Get(Category, Food), Times.Once);
            _foodRepo.Verify(m => m.Store(info), Times.Once);
            _bingFacade.Verify(m => m.FindImages(It.IsAny<string>()), Times.Once);
        }
    }
}

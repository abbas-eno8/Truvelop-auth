using System;
using AuthoritySTS.Controllers;
using AuthoritySTS.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthoritySTS.Tests.Controllers
{
    public class HomeControllerTests
    {
        private HomeController _homeController;
        public HomeControllerTests()
        {
            _homeController = new HomeController();
        }

        [Fact]
        public void IndexAction_Should_Return_View()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        //[Fact]
        //public void ErrorAction_Should_Return_View()
        //{
        //    // Arrange            
        //    var model = new ErrorViewModel()
        //    {
        //        RequestId = "Test"
        //    };

        //    var controller = new HomeController();
            

        //    // Act
        //    var result = controller.Error();

        //    // Assert
        //    Assert.IsType<ViewResult>(result);
        //}        
    }
}

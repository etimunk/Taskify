//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Taskify.Controllers;
//using Taskify.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace TaskifyTest
//{
//    internal class ProjectTest
//    {
//        private FakeContext fakeContext = new FakeContext();

//        [Fact]
//        public void Get_ReturnList()
//        {
//            //AAA
//            //arrange

//            //act
//            var controller = new ProjectController(fakeContext);
//            var result = controller.Get();

//            //assert
//            Assert.IsType<List<Project>>(result);
//        }

//        [Fact]
//        public void GetById_ReturnOK()
//        {
//            //AAA
//            //arrange
//            var id = 1;
//            //act
//            var controller = new ProjectController(fakeContext);
//            var result = controller.Get();

//            //assert
//            Assert.IsType<OkObjectResult>(result);
//        }



//        [Fact]
//        public void GetById_ReturnNotFount()
//        {
//            //AAA
//            //arrange - בחלק זה נרשום את הנתונים שנצרכים להפעלת הפונקציה
//            var id = 2;
//            //act - בחלק זה נפעיל את הפונקציה
//            var controller = new ProjectController(fakeContext);
//            var result = controller.Get(id);

//            //assert - בחלק זה נכריז על התוצאה שאנחנו מצפות לקבל
//            Assert.IsType<NotFoundResult>(result);
//        }

//        [Fact]
//        public void Add_Project_ReturnOk()
//        {
//            //AAA
//            //arrange - בחלק זה נרשום את הנתונים שנצרכים להפעלת הפונקציה
//            var pro = new Project { Id = 1, Description = "BHGBHJK", DueDate = "NKNKL", ManagerId = 1, Name = "KNJKK", StartDate = "HIH", Status = "NJJNJ" };
//            //act - בחלק זה נפעיל את הפונקציה
//            var controller = new ProjectController(fakeContext);
//            var result = controller.Post(pro);

//            //assert - בחלק זה נכריז על התוצאה שאנחנו מצפות לקבל
//            Assert.IsType<OkObjectResult>(result);
//        }
//        [Fact]
//        public void Update_ReturnOk()
//        {
//            //AAA
//            //arrange - בחלק זה נרשום את הנתונים שנצרכים להפעלת הפונקציה
//            var pro = new Project { Id = 1, Description = "BHGBHJK", DueDate = "NKNKL", ManagerId = 1, Name = "KNJKK", StartDate = "HIH", Status = "NJJNJ" };
//            //act - בחלק זה נפעיל את הפונקציה
//            var controller = new ProjectController(fakeContext);
//            var result = controller.Put(1, pro);

//            //assert - בחלק זה נכריז על התוצאה שאנחנו מצפות לקבל
//            Assert.IsType<OkObjectResult>(result);
//        }
//        [Fact]
//        public void Delete_ReturnOk()
//        {
//            //AAA
//            //arrange - בחלק זה נרשום את הנתונים שנצרכים להפעלת הפונקציה

//            //act - בחלק זה נפעיל את הפונקציה
//            var controller = new ProjectController();
//            var result = controller.Delete(1);

//            //assert - בחלק זה נכריז על התוצאה שאנחנו מצפות לקבל
//            Assert.IsType<NoContentResult>(result);
//        }
//    }
//}

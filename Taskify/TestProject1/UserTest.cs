//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Taskify.Controllers;

//namespace TaskifyTest
//{
//    internal class UserTest
//    {
//        //private FakeContext fakeContext = new FakeContext();

//        [Fact]
//        public void Get_ReturnList()
//        {
//            //AAA
//            //arrange

//            //act
//            var controller = new UserController();
//            var result = controller.Get();

//            //assert
//            Assert.IsType<List<User>>(result);
//        }

//        [Fact]
//        public void Get_ReturnCount()
//        {
//            //AAA
//            //arrange

//            //act
//            var controller = new UserController(fakeContext);
//            var result = controller.Get();

//            //assert
//            Assert.Equal(1, result.Count());
//        }


//        [Fact]
//        public void GetById_ReturnOk()
//        {
//            //AAA
//            //arrange - בחלק זה נרשום את הנתונים שנצרכים להפעלת הפונקציה
//            var id = 58;
//            //act - בחלק זה נפעיל את הפונקציה
//            var controller = new StudentsController(fakeContext);
//            var result = controller.Get(id);

//            //assert - בחלק זה נכריז על התוצאה שאנחנו מצפות לקבל
//            Assert.IsType<OkObjectResult>(result);
//        }

//        [Fact]
//        public void GetById_ReturnNotFount()
//        {
//            //AAA
//            //arrange - בחלק זה נרשום את הנתונים שנצרכים להפעלת הפונקציה
//            var id = 2;
//            //act - בחלק זה נפעיל את הפונקציה
//            var controller = new StudentsController(fakeContext);
//            var result = controller.Get(id);

//            //assert - בחלק זה נכריז על התוצאה שאנחנו מצפות לקבל
//            Assert.IsType<NotFoundResult>(result);
//        }

//        [Fact]
//        public void Add_ReturnOk()
//        {
//            //AAA
//            //arrange - בחלק זה נרשום את הנתונים שנצרכים להפעלת הפונקציה
//            var stu = new Students { FirstName = "Chana", LastName = "Levi", Grade = eGrade.b, Id = 5 };
//            //act - בחלק זה נפעיל את הפונקציה
//            var controller = new StudentsController(fakeContext);
//            var result = controller.Post(stu);

//            //assert - בחלק זה נכריז על התוצאה שאנחנו מצפות לקבל
//            Assert.IsType<OkObjectResult>(result);

//        }
//    }
//}

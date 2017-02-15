using NUnit.Framework;
using System.Collections.Generic;
using UnityCastleAutofacDiff;

namespace UnitTests
{
    [TestFixtureSource("Args")]
    public class UserControllerTest
    {
        private List<string> dt = null;
        private UserController _uc = null;

        static object[] Args = {new object[] {new List<string>() { "Hermine_0", "Gor_1", "Shaqeh_2" }},  //case 1
                                //new object[] {...} //case 2
                                };

        public UserControllerTest(List<string> data)
        {
            dt = data;
        }

        [SetUp]
        public void SetUp()
        {
            _uc = new UserController(dt);            
        }

        [TearDown]
        public void Clear()
        {
            _uc = null;
        }

        [Test]
        public void GetUsersTest()
        {
            Assert.That(_uc.GetUsers().Count, Is.EqualTo(3));
            //CollectionAssert.IsNotEmpty(_uc.GetUsers());
        }

        [Test]
        public void GetUserByIdTest([Range(0, 2, 1)] int id)
        {
            StringAssert.EndsWith("_" + id, _uc.GetUserById(id));
        }

        [Test]
        public void SaveUserTest([Values("Embo", "Rembo")] string name)
        {
            _uc.SaveUser(name).Wait();
            CollectionAssert.Contains(_uc.GetUsers(), name);
        }

        [Ignore("I need to ignore this old test")]
        public void SomeIgnoredTest()
        { }

        [Test]        
        public void GetUserByIdOutOfBoundTest()
        {
            Assert.That(() => _uc.GetUserById(-1), Throws.Nothing);
            Assert.That(_uc.GetUserById(-1), Is.Empty);

            Assert.That(() => _uc.GetUserById(_uc.GetUsers().Count), Throws.Nothing);
            Assert.That(_uc.GetUserById(_uc.GetUsers().Count), Is.Empty);
        }
    }
}

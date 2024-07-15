using AutoFixture;
using SimpleChat.Library.Clients;
using SimpleChat.Library.Models;
using SimpleChat.Library.Requests.Users;

namespace SimpleChat.Api.Tests
{
    public class UsersClientTests
    {
        private readonly Fixture _fixture = new();
        private UsersClinet _systemUnderTest;

        public UsersClientTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public void Setup() => _systemUnderTest = new UsersClinet(new HttpClient(), "http://localhost:5008");

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_add_user_THEN_user_is_added_to_database()
        {
            //Arrange
            var expected = _fixture.Build<User>()
                .Without(u => u.ChatsCreated)
                .Without(u => u.Chats)
                .Without(u => u.Messages)
                .Create();

            //Act & Assert
            var addResponse = await _systemUnderTest.Add(expected);
            Assert.That(addResponse.IsSuccessfull, Is.True);

            var getOneResponse = await _systemUnderTest.GetOne(addResponse.Payload);
            Assert.That(getOneResponse.IsSuccessfull, Is.True);
            var actual = getOneResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            var removeResponse = await _systemUnderTest.Remove(actual.Id);
            Assert.That(removeResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_update_user_THEN_user_is_being_update_in_database()
        {
            //Arrange
            var expected = _fixture.Build<User>()
                .Without(u => u.ChatsCreated)
                .Without(u => u.Chats)
                .Without(u => u.Messages)
                .Create();

            //Act & Assert
            var addResponse = await _systemUnderTest.Add(expected);
            Assert.That(addResponse.IsSuccessfull, Is.True);

            expected.Name = _fixture.Create<string>();
            var updateRequest = new UpdateUserRequest() { Id = expected.Id, Name = expected.Name };
            var updateResponse = await _systemUnderTest.Update(updateRequest);
            Assert.That(updateResponse.IsSuccessfull, Is.True);

            var actual = updateResponse.Payload;
            AssertObjectsAreEqual(expected, actual);

            var removeResponse = await _systemUnderTest.Remove(actual.Id);
            Assert.That(removeResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_search_users_by_name_THEN_users_with_similar_name_returned()
        {
            //Arrange
            var expected = _fixture.Build<User>()
                .Without(u => u.ChatsCreated)
                .Without(u => u.Chats)
                .Without(u => u.Messages)
                .Create();

            //Act & Assert
            var addResponse = await _systemUnderTest.Add(expected);
            Assert.That(addResponse.IsSuccessfull, Is.True);

            var getAllResponse = await _systemUnderTest.GetAll(expected.Name);
            Assert.That(getAllResponse.IsSuccessfull, Is.True);

            var actualUsers = getAllResponse.Payload;
            Assert.That(actualUsers.Count(), Is.GreaterThan(0));

            var actual = getAllResponse.Payload.Single(u => u.Id == expected.Id);
            AssertObjectsAreEqual(expected, actual);

            var removeResponse = await _systemUnderTest.Remove(actual.Id);
            Assert.That(removeResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_get_all_users_THEN_they_are_returned()
        {
            //Arrange
            var expected = _fixture.Build<User>()
                .Without(u => u.ChatsCreated)
                .Without(u => u.Chats)
                .Without(u => u.Messages)
                .CreateMany(3);

            //Act & Assert
            foreach (var user in expected)
            {
                var addResponse = await _systemUnderTest.Add(user);
                Assert.That(addResponse.IsSuccessfull, Is.True);
            }

            var getAllResponse = await _systemUnderTest.GetAll();
            Assert.That(getAllResponse.IsSuccessfull, Is.True);

            var actualUsers = getAllResponse.Payload;
            Assert.That(actualUsers.Count(), Is.EqualTo(expected.Count()));

            foreach (var expectedUser in expected)
            {
                var actual = actualUsers.Single(u => u.Id == expectedUser.Id);
                AssertObjectsAreEqual(expectedUser, actual);
            }

            foreach (var user in expected)
            {
                var removeResponse = await _systemUnderTest.Remove(user.Id);
                Assert.That(removeResponse.IsSuccessfull, Is.True);
            }
        }

        private void AssertObjectsAreEqual(User expected, User actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Name, Is.EqualTo(expected.Name));
            });
        }
    }
}

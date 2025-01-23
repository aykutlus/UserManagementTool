using ManagementTool.Functions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementTool.Functions.Tests.Domain
{
    public class DomainUserTests
    {
        [Fact]
        public void GivenNewRequest_WhenCreateWithIdAndCredits_ThenShouldSetPropertiesCorrectly()
        {           
            //arrange
            var id = Guid.NewGuid().ToString();
            var name = "Test User";
            var email = "test@example.com";
            var credits = 100;

            //act
            var user = DomainUser.CreateWithIdAndCredits(id, name, email, credits);

            //assert
            Assert.Equal(id, user.Id);
            Assert.Equal(credits, user.Credits);
        }

        [Fact]
        public void GivenIdIsNull_WhenCreateWithIdAndCredits_ShouldThrowArgumentException()
        {
            //arrange
            string id = null;
            var name = "Test User";
            var email = "test@example.com";
            var credits = 100;

            //act & assert
            Assert.Throws<ArgumentException>(() =>
                DomainUser.CreateWithIdAndCredits(id, name, email, credits));
        }

        [Fact]
        public void WhenUpdatingDetails_WhenUpdateDetails_ShouldUpdateNameAndEmail()
        {
            //arrange
            var user = new DomainUser("Initial Name", "initial@example.com");
            var newName = "Updated Name";
            var newEmail = "updated@example.com";

            //act
            user.UpdateDetails(newName, newEmail);

            //assert
            Assert.Equal(newName, user.Name);
            Assert.Equal(newEmail, user.Email);
        }

        [Fact]
        public void GivenNewNameOrEmailIsNull_WHenUpdateDetails_ThenThrowException()
        {
            //arrange
            var user = new DomainUser("Initial Name", "initial@example.com");

            //act & assert
            Assert.Throws<ArgumentException>(() => user.UpdateDetails(null, "updated@example.com"));
            Assert.Throws<ArgumentException>(() => user.UpdateDetails("Updated Name", null));
            Assert.Throws<ArgumentException>(() => user.UpdateDetails("   ", "updated@example.com"));
        }

        [Fact]
        public void GivenCreditIsValid_WhenAddCredits_ThenIncreaseCredits()
        {
            //arrange
            var user = DomainUser.CreateWithIdAndCredits(Guid.NewGuid().ToString(), "Test User", "test@example.com", 50);
            var creditsToAdd = 100;

            //act
            user.AddCredits(creditsToAdd);

            //assert
            Assert.Equal(150, user.Credits);
        }

        [Fact]
        public void GivenInvalidCredit_WHenAddCredits_ThenThrowsException()
        {
            //arrange
            var user = DomainUser.CreateWithIdAndCredits(Guid.NewGuid().ToString(), "Test User", "test@example.com", 50);

            //act & assert
            Assert.Throws<ArgumentException>(() => user.AddCredits(0));
            Assert.Throws<ArgumentException>(() => user.AddCredits(-10));
        }

        [Fact]
        public void GivenValidHistoryEnttry_WHenAddHistoryEntry_ThenAddsToHistory()
        {
            //arrange
            var user = DomainUser.CreateWithIdAndCredits(Guid.NewGuid().ToString(), "Test User", "test@example.com", 50);
            var oldValue = 50;
            var newValue = 100;

            //act
            user.AddHistoryEntry(ChangeType.ModifiedUserCredits, oldValue, newValue);

            //assert
            var historyEntry = user.History.GetEnumerator();
            historyEntry.MoveNext();
            Assert.Equal(ChangeType.ModifiedUserCredits, historyEntry.Current.ChangeType);
        }
    }
}

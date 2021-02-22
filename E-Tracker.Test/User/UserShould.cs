using E_Tracker.CreateDto;
using FluentValidation.TestHelper;
using Xunit;

namespace E_Tracker.Test.User
{
    public class UserShould
    {
        private UserValidation _userValidation;

        public UserShould ()
        {
            this._userValidation = new UserValidation();
        }

         [Fact]
        public void HaveValidEmail()
        {
            //ValidEmailTest
            _userValidation.ShouldHaveValidationErrorFor(user => user.Email, null as string);
        }

        [Fact]
        public void HaveSurname()
        {
            //NotEmptySurNameTest
            _userValidation.ShouldHaveValidationErrorFor(user => user.Surname, null as string);
        }

        [Fact]
        public void HaveOtherName()
        {
            //NotEmptyOtherNameTest
            _userValidation.ShouldHaveValidationErrorFor(user => user.OtherNames, null as string);
        }

        [Fact]
        public void HaveDepartment()
        {
            //MustBeAssignedToDepartmet
            _userValidation.ShouldHaveValidationErrorFor(user => user.DepartmentId, null as string);
        }

        [Fact]
        public void HavePassword()
        {
            //MustInputAPassword
            _userValidation.ShouldHaveValidationErrorFor(user => user.Password, null as string);
        }

        [Fact]
        public void ConfirmPassword()
        {
            //MustConfirmAbovePassword
            _userValidation.ShouldHaveValidationErrorFor(user => user.ConfirmPassword, null as string);
        }
    }
}

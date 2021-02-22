

using E_Tracker.Dto;
using FluentValidation.TestHelper;
using Xunit;

namespace E_Tracker.Test.Role
{
    public class RoleShould
    {
        private RoleValidation _roleValidation;

        public RoleShould()
        {
           this._roleValidation = new RoleValidation();
        }

        [Fact]
        public void HaveValidName()
        {
            //ValidEmailTest
            _roleValidation.ShouldHaveValidationErrorFor(user => user.Name, null as string);
        }
    }
}

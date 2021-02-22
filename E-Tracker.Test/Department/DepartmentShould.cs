using E_Tracker.CreateDto;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace E_Tracker.Test.Department
{

    public class DepartmentShould
    {
        private DepartmentValidation validations;
        public DepartmentShould()
        {
            this.validations = new DepartmentValidation();
        }

        [Fact]
        public void HaveValidName()
        {
         
            // test name 
            validations.ShouldHaveValidationErrorFor(name => name.Name, null as string);
        }
    } 
}

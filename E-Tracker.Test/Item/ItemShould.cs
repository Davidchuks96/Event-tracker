using E_Tracker.CreateDto;
using E_Tracker.Dto;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace E_Tracker.Test.Item
{
    public class ItemShould
    {
        private CreateDto.ItemValidation _validations;
        public ItemShould()
        {
            _validations = new CreateDto.ItemValidation();
        }
        [Fact]
        public void HaveValidName()
        {
            // test name 
            _validations.ShouldHaveValidationErrorFor(name => name.Name, null as string);
        }

        [Fact]
        public void HaveName()
        {
            // test name 
            _validations.ShouldHaveValidationErrorFor(name => name.Name, null as string);
        }
    }
}

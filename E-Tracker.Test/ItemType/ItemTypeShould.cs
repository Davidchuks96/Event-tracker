using System;
using System.Collections.Generic;
using System.Text;
using E_Tracker.CreateDto;
using E_Tracker.Dto;
using FluentValidation.TestHelper;
using Xunit;
using ItemTypeValidation = E_Tracker.CreateDto.ItemTypeValidation;

namespace E_Tracker.Test.ItemType
{
    public class ItemTypeShould
    {
       private ItemTypeValidation _validations;
       public ItemTypeShould()
        {
           _validations = new ItemTypeValidation();
        }
        [Fact]
        public void HaveValidName()
        {
             //test name 
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

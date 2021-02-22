using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static E_Tracker.CreateDto.CreateServiceDto;

namespace E_Tracker.Test.Service
{
    public class ServiceShould
    {
        private ServiceValidation _validations;
        public ServiceShould()
        {
            _validations = new ServiceValidation();
        }
        [Fact]
        public void HaveValidName()
        {
            // test name 
            _validations.ShouldHaveValidationErrorFor(name => name.ItemId, null as string);
        }

    }
}

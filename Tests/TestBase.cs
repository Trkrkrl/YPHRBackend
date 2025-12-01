using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class TestBase : IClassFixture<BusinessTestFixture>
    {
        protected readonly BusinessTestFixture Fixture;

        public TestBase(BusinessTestFixture fixture)
        {
            Fixture = fixture;
        }
    }
}

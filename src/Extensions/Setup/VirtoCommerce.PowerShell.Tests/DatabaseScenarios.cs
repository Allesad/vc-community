﻿using FunctionalTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.PowerShell.DatabaseSetup;
using VirtoCommerce.PowerShell.DatabaseSetup.Cmdlet;
using Xunit;

namespace VirtoCommerce.PowerShell.Tests
{
    public class DatabaseScenarios : DbTestBase
    {
        private string _PowerShellRootFolder = @"e:\Projects\VCF\main\Community\src\Extensions\Setup\VirtoCommerce.PowerShell";

        private string PowerShellRootFolder
        {
            get
            {
                return _PowerShellRootFolder;
            }
        }

        [Fact]
        public void Can_setup_customer_database()
        {
            DropDatabase();
            var publisher = new PublishCustomerDatabase();
            publisher.Publish(TestDatabase.ConnectionString, PowerShellRootFolder, true);
            Assert.True(TableExists("Contract"));
            DropDatabase();
        }

        [Fact]
        public void Can_setup_security_database()
        {
            DropDatabase();
            var publisher = new PublishSecurityDatabase();
            publisher.Publish(TestDatabase.ConnectionString, PowerShellRootFolder, true);
            Assert.True(TableExists("Account"));
            DropDatabase();
        }

        [Fact]
        public void Can_setup_order_database()
        {
            DropDatabase();
			
            var publisher = new PublishOrderDatabase();
            publisher.Publish(TestDatabase.ConnectionString, PowerShellRootFolder, true);
            Assert.True(TableExists("Gateway"));
            DropDatabase();
        }
    }
}

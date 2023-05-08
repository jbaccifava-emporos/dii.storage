﻿using dii.storage.Exceptions;
using dii.storage.tests.Attributes;
using dii.storage.tests.Models;
using dii.storage.tests.OptimizerTests.Data;
using dii.storage.tests.Orderer;
using dii.storage.tests.Utilities;
using System;
using Xunit;

namespace dii.storage.tests.OptimizerTests
{
    [Collection(nameof(ConfigureTypesTests))]
    [TestCollectionPriorityOrder(202)]
    [TestCaseOrderer(TestPriorityOrderer.FullName, TestPriorityOrderer.AssemblyName)]
    public class ConfigureTypesTests
    {
        [Fact, TestPriorityOrder(100)]
        public void ConfigureTypes_Prep()
        {
            _ = Optimizer.Init(typeof(FakeEntity));

            TestHelpers.AssertOptimizerIsInitialized();
        }

        [Theory, TestPriorityOrder(101), ClassData(typeof(ConfigureTypesNoOpData))]
        public void ConfigureTypes_NoOp(Type[] type)
        {
            Optimizer optimizer;
            try
            {
                optimizer = Optimizer.Get();
            }
            catch
            {
                ConfigureTypes_Prep();
                optimizer = Optimizer.Get();
            }

            Assert.Single(optimizer.Tables);

            var tablesInitialized = optimizer.Tables;
            var tableMappingsInitialized = optimizer.TableMappings;

            optimizer.ConfigureTypes(type);

            Assert.Single(optimizer.Tables);
            Assert.Equal(tablesInitialized.Count, optimizer.Tables.Count);
            Assert.Equal(tablesInitialized[0].TableName, optimizer.Tables[0].TableName);
            Assert.Equal(tablesInitialized[0].ClassName, optimizer.Tables[0].ClassName);

            Assert.Single(optimizer.TableMappings);
            Assert.Equal(tableMappingsInitialized.Count, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].TableName, optimizer.TableMappings[typeof(FakeEntity)].TableName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].ClassName, optimizer.TableMappings[typeof(FakeEntity)].ClassName);
        }

        [Theory, TestPriorityOrder(102), ClassData(typeof(ConfigureTypesInvalidSearchableKeyExceptionData))]
        public void ConfigureTypes_AddTypeWithInvalidSearchableKey(Type type, string key, string propertyName, string typeName)
        {
            Optimizer optimizer;
            try
            {
                optimizer = Optimizer.Get();
            }
            catch
            {
                ConfigureTypes_Prep();
                optimizer = Optimizer.Get();
            }

            Assert.Single(optimizer.Tables);

            var tablesInitialized = optimizer.Tables;
            var tableMappingsInitialized = optimizer.TableMappings;

            var exception = Assert.Throws<DiiReservedSearchableKeyException>(() => { optimizer.ConfigureTypes(type); });

            Assert.NotNull(exception);
            Assert.Equal(new DiiReservedSearchableKeyException(key, propertyName, typeName).Message, exception.Message);

            Assert.Single(optimizer.Tables);
            Assert.Equal(tablesInitialized.Count, optimizer.Tables.Count);
            Assert.Equal(tablesInitialized[0].TableName, optimizer.Tables[0].TableName);
            Assert.Equal(tablesInitialized[0].ClassName, optimizer.Tables[0].ClassName);

            Assert.Single(optimizer.TableMappings);
            Assert.Equal(tableMappingsInitialized.Count, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].TableName, optimizer.TableMappings[typeof(FakeEntity)].TableName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].ClassName, optimizer.TableMappings[typeof(FakeEntity)].ClassName);
        }

        [Theory, TestPriorityOrder(103), ClassData(typeof(ConfigureTypesInvalidPartitionKeyOrderExceptionData))]
        public void ConfigureTypes_AddTypeWithInvalidPartitionKeyOrder(Type type, string propertyName, string duplicatePropertyName, int order)
        {
            Optimizer optimizer;
            try
            {
                optimizer = Optimizer.Get();
            }
            catch
            {
                ConfigureTypes_Prep();
                optimizer = Optimizer.Get();
            }

            Assert.Single(optimizer.Tables);

            var tablesInitialized = optimizer.Tables;
            var tableMappingsInitialized = optimizer.TableMappings;

            var exception = Assert.Throws<DiiPartitionKeyDuplicateOrderException>(() => { optimizer.ConfigureTypes(type); });

            Assert.NotNull(exception);
            Assert.Equal(new DiiPartitionKeyDuplicateOrderException(propertyName, duplicatePropertyName, order).Message, exception.Message);

            Assert.Single(optimizer.Tables);
            Assert.Equal(tablesInitialized.Count, optimizer.Tables.Count);
            Assert.Equal(tablesInitialized[0].TableName, optimizer.Tables[0].TableName);
            Assert.Equal(tablesInitialized[0].ClassName, optimizer.Tables[0].ClassName);

            Assert.Single(optimizer.TableMappings);
            Assert.Equal(tableMappingsInitialized.Count, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].TableName, optimizer.TableMappings[typeof(FakeEntity)].TableName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].ClassName, optimizer.TableMappings[typeof(FakeEntity)].ClassName);
        }

        [Theory, TestPriorityOrder(104), ClassData(typeof(ConfigureTypesInvalidIdOrderExceptionData))]
        public void ConfigureTypes_AddTypeWithInvalidIdOrder(Type type, string propertyName, string duplicatePropertyName, int order)
        {
            Optimizer optimizer;
            try
            {
                optimizer = Optimizer.Get();
            }
            catch
            {
                ConfigureTypes_Prep();
                optimizer = Optimizer.Get();
            }

            Assert.Single(optimizer.Tables);

            var tablesInitialized = optimizer.Tables;
            var tableMappingsInitialized = optimizer.TableMappings;

            var exception = Assert.Throws<DiiIdDuplicateOrderException>(() => { optimizer.ConfigureTypes(type); });

            Assert.NotNull(exception);
            Assert.Equal(new DiiIdDuplicateOrderException(propertyName, duplicatePropertyName, order).Message, exception.Message);

            Assert.Single(optimizer.Tables);
            Assert.Equal(tablesInitialized.Count, optimizer.Tables.Count);
            Assert.Equal(tablesInitialized[0].TableName, optimizer.Tables[0].TableName);
            Assert.Equal(tablesInitialized[0].ClassName, optimizer.Tables[0].ClassName);

            Assert.Single(optimizer.TableMappings);
            Assert.Equal(tableMappingsInitialized.Count, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].TableName, optimizer.TableMappings[typeof(FakeEntity)].TableName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].ClassName, optimizer.TableMappings[typeof(FakeEntity)].ClassName);
        }

        [Fact, TestPriorityOrder(105)]
        public void ConfigureTypes_AddNewType()
        {
            Optimizer optimizer;
            try
            {
                optimizer = Optimizer.Get();
            }
            catch
            {
                ConfigureTypes_Prep();
                optimizer = Optimizer.Get();
            }

            Assert.Single(optimizer.Tables);

            var tablesInitialized = optimizer.Tables;
            var tableMappingsInitialized = optimizer.TableMappings;

            optimizer.ConfigureTypes(typeof(FakeEntityTwo));

            Assert.Equal(2, optimizer.Tables.Count);
            Assert.Equal(tablesInitialized[0].TableName, optimizer.Tables[0].TableName);
            Assert.Equal(tablesInitialized[0].ClassName, optimizer.Tables[0].ClassName);
            Assert.Equal(nameof(FakeEntityTwo), optimizer.Tables[1].TableName);
            Assert.Equal(nameof(FakeEntityTwo), optimizer.Tables[1].ClassName);

            Assert.Equal(2, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized.Count, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].TableName, optimizer.TableMappings[typeof(FakeEntity)].TableName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].ClassName, optimizer.TableMappings[typeof(FakeEntity)].ClassName);
            Assert.Equal(nameof(FakeEntityTwo), optimizer.TableMappings[typeof(FakeEntityTwo)].TableName);
            Assert.Equal(nameof(FakeEntityTwo), optimizer.TableMappings[typeof(FakeEntityTwo)].ClassName);
        }

        [Fact, TestPriorityOrder(106)]
        public void ConfigureTypes_AddNewTypeWithSameIdAndPKProperty()
        {
            Optimizer optimizer;
            try
            {
                optimizer = Optimizer.Get();
            }
            catch
            {
                ConfigureTypes_Prep();
                optimizer = Optimizer.Get();
            }

            Assert.Equal(2, optimizer.Tables.Count);

            var tablesInitialized = optimizer.Tables;
            var tableMappingsInitialized = optimizer.TableMappings;

            optimizer.ConfigureTypes(typeof(FakeEntityFive));

            Assert.Equal(3, optimizer.Tables.Count);
            Assert.Equal(tablesInitialized[0].TableName, optimizer.Tables[0].TableName);
            Assert.Equal(tablesInitialized[0].ClassName, optimizer.Tables[0].ClassName);
            Assert.Equal(tablesInitialized[1].TableName, optimizer.Tables[1].TableName);
            Assert.Equal(tablesInitialized[1].ClassName, optimizer.Tables[1].ClassName);
            Assert.Equal(nameof(FakeEntityFive), optimizer.Tables[2].TableName);
            Assert.Equal(nameof(FakeEntityFive), optimizer.Tables[2].ClassName);

            Assert.Equal(3, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized.Count, optimizer.TableMappings.Count);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].TableName, optimizer.TableMappings[typeof(FakeEntity)].TableName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntity)].ClassName, optimizer.TableMappings[typeof(FakeEntity)].ClassName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntityTwo)].TableName, optimizer.TableMappings[typeof(FakeEntityTwo)].TableName);
            Assert.Equal(tableMappingsInitialized[typeof(FakeEntityTwo)].ClassName, optimizer.TableMappings[typeof(FakeEntityTwo)].ClassName);
            Assert.Equal(nameof(FakeEntityFive), optimizer.TableMappings[typeof(FakeEntityFive)].TableName);
            Assert.Equal(nameof(FakeEntityFive), optimizer.TableMappings[typeof(FakeEntityFive)].ClassName);
        }

        #region Teardown
        [Fact, TestPriorityOrder(int.MaxValue)]
        public void Teardown()
        {
            TestHelpers.ResetOptimizerInstance();
        }
        #endregion
    }
}
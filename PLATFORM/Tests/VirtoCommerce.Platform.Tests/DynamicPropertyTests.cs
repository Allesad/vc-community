﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Data.DynamicProperties;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Tests
{
    public class WithDynamicProperties : IHasDynamicProperties
    {
        public ICollection<DynamicPropertyObjectValue> DynamicPropertyValues { get; set; }
    }

    [TestClass]
    public class DynamicPropertyTests
    {
        [TestMethod]
        public void GetObjectTypes()
        {
            var service = GetDynamicPropertyService();
            var typeNames = service.GetObjectTypes();
        }

        [TestMethod]
        public void SaveLoadPropertiesAndValues()
        {
            var service = GetDynamicPropertyService();

            // Delete existing properties
            var existingTypeProperties = service.GetProperties("TestObjectType");
            var propertyIds = existingTypeProperties.Select(p => p.Id).ToArray();
            service.DeleteProperties(propertyIds);

            // Test properties
            var typeProperties = new[]
            {
                new DynamicProperty
                {
                    ObjectType = "TestObjectType",
                    Name = "Color",
                    ValueType = DynamicPropertyValueType.ShortText,
                    DisplayNames = new[]
                    {
                        new DynamicPropertyName
                        {
                            Locale = "en-US", Name = "Color"
                        },
                        new DynamicPropertyName
                        {
                            Locale = "ru-RU", Name = "Цвет"
                        },
                    },
                    IsDictionary = true,
                    IsArray = true,
                },
                new DynamicProperty
                {
                    ObjectType = "TestObjectType",
                    Name = "SingleValueProperty",
                    ValueType = DynamicPropertyValueType.ShortText,
                    DisplayNames = new[]
                    {
                        new DynamicPropertyName
                        {
                            Locale = "en-US", Name = "Single Value Property"
                        },
                        new DynamicPropertyName
                        {
                            Locale = "ru-RU", Name = "Свойство с одним значением"
                        },
                    },
                },
                new DynamicProperty
                {
                    ObjectType = "TestObjectType",
                    Name = "Array",
                    ValueType = DynamicPropertyValueType.ShortText,
                    IsArray = true,
                    DisplayNames = new[]
                    {
                        new DynamicPropertyName
                        {
                            Locale = "en-US", Name = "Array"
                        },
                        new DynamicPropertyName
                        {
                            Locale = "ru-RU", Name = "Массив"
                        },
                    },
                },
            };

            service.SaveProperties(typeProperties);

            existingTypeProperties = service.GetProperties("TestObjectType");

            // Rename property
            var renamedProperties = new[] { existingTypeProperties[0] };
            var originalName = renamedProperties[0].Name;
            renamedProperties[0].Name = "NewName";
            service.SaveProperties(renamedProperties);

            // Return original name
            renamedProperties[0].Name = originalName;
            service.SaveProperties(renamedProperties);

            var arrayProperty = existingTypeProperties.First(p => p.Name == "Array");
            var singleValueProperty = existingTypeProperties.First(p => p.Name == "SingleValueProperty");
            var colorProperty = existingTypeProperties.First(p => p.Name == "Color");

            // Test dictionary items
            var newItems = new[]
            {
                new DynamicPropertyDictionaryItem
                {
                    Name = "Red",
                    DictionaryValues = new[]
                    {
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "en-US", Value = "Red"
                        },
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "ru-RU", Value = "Красный"
                        },
                    },
                },
                new DynamicPropertyDictionaryItem
                {
                    Name = "Green",
                    DictionaryValues = new[]
                    {
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "en-US", Value = "Green"
                        },
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "ru-RU", Value = "Зелёный"
                        },
                    },
                },
                new DynamicPropertyDictionaryItem
                {
                    Name = "Blue",
                    DictionaryValues = new[]
                    {
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "en-US", Value = "Blue"
                        },
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "ru-RU", Value = "Синий"
                        },
                    },
                },
                new DynamicPropertyDictionaryItem
                {
                    Name = "Yellow",
                    DictionaryValues = new[]
                    {
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "en-US", Value = "Yellow"
                        },
                        new DynamicPropertyDictionaryValue
                        {
                            Locale = "ru-RU", Value = "Жёлтый"
                        },
                    },
                },
            };

            service.SaveDictionaryItems(colorProperty.Id, newItems);

            var dictionaryItems = service.GetDictionaryItems(colorProperty.Id);

            var redColor = dictionaryItems.First(i => i.Name == "Red");
            var greenColor = dictionaryItems.First(i => i.Name == "Green");
            var blueColor = dictionaryItems.First(i => i.Name == "Blue");
            var yellowColor = dictionaryItems.First(i => i.Name == "Yellow");

            yellowColor.Name = "Pink";
            yellowColor.DictionaryValues[0].Value = "Pink";
            service.SaveDictionaryItems(colorProperty.Id, dictionaryItems);

            service.DeleteDictionaryItems(new[] { yellowColor.Id });

            // Test object values
            var objectProperties = new[]
            {
                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = colorProperty.Id },
                    ObjectId = "111",
                    Values = new[] { redColor.Id },
                },
                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = colorProperty.Id },
                    ObjectId = "222",
                    Values = new[] { greenColor.Id, blueColor.Id },
                },

                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = singleValueProperty.Id },
                    ObjectId = "111",
                    Locale = "en-US",
                    Values = new[] { "Fork" },
                },
                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = singleValueProperty.Id },
                    ObjectId = "111",
                    Locale = "ru-RU",
                    Values = new[] { "Вилка" },
                },
                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = singleValueProperty.Id },
                    ObjectId = "222",
                    Locale = "en-US",
                    Values = new[] { "Spoon" },
                },
                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = singleValueProperty.Id },
                    ObjectId = "222",
                    Locale = "ru-RU",
                    Values = new[] { "Ложка" },
                },

                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = arrayProperty.Id },
                    ObjectId = "222",
                    Locale = "en-US",
                    Values = new[] { "flower", "tree" },
                },
                new DynamicPropertyObjectValue
                {
                    Property = new DynamicProperty { Id = arrayProperty.Id },
                    ObjectId = "222",
                    Locale = "ru-RU",
                    Values = new[] { "цветок", "дерево" },
                },
            };

            service.SaveObjectValues(objectProperties);

            var objectProperties1 = service.GetObjectValues("TestObjectType", "111");
            var objectProperties2 = service.GetObjectValues("TestObjectType", "222");
        }

        private IDynamicPropertyService GetDynamicPropertyService()
        {
            return new DynamicPropertyService(() => new PlatformRepository("VirtoCommerce", new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor()));
        }
    }
}

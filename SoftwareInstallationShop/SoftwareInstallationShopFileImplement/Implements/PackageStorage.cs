﻿using SoftwareInstallationShopContracts.BindingModels;
using SoftwareInstallationShopContracts.StoragesContracts;
using SoftwareInstallationShopContracts.ViewModels;
using SoftwareInstallationShopFileImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareInstallationShopFileImplement.Implements
{
    class PackageStorage : IPackageStorage
    {
        private readonly FileDataListSingleton source;
        public PackageStorage()
        {
            source = FileDataListSingleton.GetInstance();
        }
        public List<PackageViewModel> GetFullList()
        {
            return source.Packages
            .Select(CreateModel)
            .ToList();
        }
        public List<PackageViewModel> GetFilteredList(PackageBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            return source.Packages
            .Where(rec => rec.PackageName.Contains(model.PackageName))
            .Select(CreateModel)
            .ToList();
        }
        public PackageViewModel GetElement(PackageBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            var package = source.Packages
            .FirstOrDefault(rec => rec.PackageName == model.PackageName || rec.Id
           == model.Id);
            return package != null ? CreateModel(package) : null;
        }
        public void Insert(PackageBindingModel model)
        {
            int maxId = source.Packages.Count > 0 ? source.Components.Max(rec => rec.Id): 0;
            var element = new Package
            {
                Id = maxId + 1,
                PackageComponents = new
           Dictionary<int, int>()
            };
            source.Packages.Add(CreateModel(model, element));
        }
        public void Update(PackageBindingModel model)
        {
            var element = source.Packages.FirstOrDefault(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            CreateModel(model, element);
        }
        public void Delete(PackageBindingModel model)
        {
            Package element = source.Packages.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                source.Packages.Remove(element);
            }
            else
            {
                throw new Exception("Элемент не найден");
            }
        }
        private static Package CreateModel(PackageBindingModel model, Package package)
        {
            package.PackageName = model.PackageName;
            package.Price = model.Price;
            // удаляем убранные
            foreach (var key in package.PackageComponents.Keys.ToList())
            {
                if (!model.PackageComponents.ContainsKey(key))
                {
                    product.PackageComponents.Remove(key);
                }
            }
            // обновляем существуюущие и добавляем новые
            foreach (var component in model.PackageComponents)
            {
                if (product.PackageComponents.ContainsKey(component.Key))
                {
                    product.PackageComponents[component.Key] =
                   model.PackageComponents[component.Key].Item2;
                }
                else
                {
                    product.PackageComponents.Add(component.Key,
                   model.PackageComponents[component.Key].Item2);
                }
            }
            return product;
        }
        private PackageViewModel CreateModel(Package package)
        {
            return new PackageViewModel
            {
                Id = package.Id,
                PackageName = package.ProductName,
                Price = package.Price,
                PackageComponents = package.ProductComponents.ToDictionary(recPC => recPC.Key, recPC =>(source.Components.FirstOrDefault(recC => recC.Id == 
                recPC.Key)?.ComponentName, recPC.Value))
            };
        }
    }
}

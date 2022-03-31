﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftwareInstallationShopContracts.StoragesContracts;
using SoftwareInstallationShopContracts.ViewModels;
using SoftwareInstallationShopContracts.BindingModels;
using SoftwareInstallationShopDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;

namespace SoftwareInstallationShopDatabaseImplement.Implements
{
    public class PackageStorage : IPackageStorage
    {
        public List<PackageViewModel> GetFullList()
        {
            using var context = new SoftwareInstallationShopDatabase();
            return context.Packages
            .Include(rec => rec.PackageComponents)
            .ThenInclude(rec => rec.Component)
            .ToList()
            .Select(CreateModel)
            .ToList();
        }
        public List<PackageViewModel> GetFilteredList(PackageBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new SoftwareInstallationShopDatabase();
            return context.Packages
            .Include(rec => rec.PackageComponents)
            .ThenInclude(rec => rec.Component)
            .Where(rec => rec.PackageName.Contains(model.PackageName))
            .ToList()
            .Select(CreateModel)
            .ToList();
        }
        public PackageViewModel GetElement(PackageBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new SoftwareInstallationShopDatabase();
            var package = context.Packages
            .Include(rec => rec.PackageComponents)
            .ThenInclude(rec => rec.Component)
            .FirstOrDefault(rec => rec.PackageName == model.PackageName || rec.Id == model.Id);
            return package != null ? CreateModel(package) : null;
        }
        public void Insert(PackageBindingModel model)
        {
            using var context = new SoftwareInstallationShopDatabase();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                Package package = new Package()
                {
                    PackageName = model.PackageName,
                    Price = model.Price
                };
                context.Packages.Add(package);
                context.SaveChanges();
                CreateModel(model, package, context);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void Update(PackageBindingModel model)
        {
            using var context = new SoftwareInstallationShopDatabase();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var element = context.Packages.FirstOrDefault(rec => rec.Id == model.Id);
                if (element == null)
                {
                    throw new Exception("Элемент не найден");
                }
                CreateModel(model, element, context);
                context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void Delete(PackageBindingModel model)
        {
            using var context = new SoftwareInstallationShopDatabase();
            Package element = context.Packages.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Packages.Remove(element);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Элемент не найден");
            }
        }
        private static Package CreateModel(PackageBindingModel model, Package package, SoftwareInstallationShopDatabase context)
        {
            package.PackageName = model.PackageName;
            package.Price = model.Price;
            if (model.Id.HasValue)
            {
                var packageComponents = context.PackageComponents.Where(rec => rec.PackageId == model.Id.Value).ToList();
                // удалили те, которых нет в модели
                context.PackageComponents.RemoveRange(packageComponents.Where(rec => !model.PackageComponents.ContainsKey(rec.ComponentId)).ToList());
                context.SaveChanges();
                // обновили количество у существующих записей
                foreach (var updateComponent in packageComponents)
                {
                    updateComponent.Count = model.PackageComponents[updateComponent.ComponentId].Item2;
                    model.PackageComponents.Remove(updateComponent.ComponentId);
                }
                context.SaveChanges();
            }
            // добавили новые
            foreach (var fc in model.PackageComponents)
            {
                context.PackageComponents.Add(new PackageComponent
                {
                    PackageId = package.Id,
                    ComponentId = fc.Key,
                    Count = fc.Value.Item2
                });
                context.SaveChanges();
            }
            return package;
        }
        private static PackageViewModel CreateModel(Package package)
        {
            return new PackageViewModel
            {
                Id = package.Id,
                PackageName = package.PackageName,
                Price = package.Price,
                PackageComponents = package.PackageComponents
                .ToDictionary(recFC => recFC.ComponentId, recFC => (recFC.Component?.ComponentName, recFC.Count))
            };
        }
    }
}
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
    public class OrderStorage : IOrderStorage
    {
        public List<OrderViewModel> GetFullList()
        {
            using var context = new SoftwareInstallationShopDatabase();
            return context.Orders
                .Select(CreateModel)
                .ToList();
        }
        public List<OrderViewModel> GetFilteredList(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new SoftwareInstallationShopDatabase();
            return context.Orders
            .Where(rec => rec.Id.Equals(model.Id))
            .Select(CreateModel)
            .ToList();
        }
        public OrderViewModel GetElement(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new SoftwareInstallationShopDatabase();
            var order = context.Orders
            .FirstOrDefault(rec => rec.Id == model.Id);
            return order != null ? CreateModel(order) : null;
        }
        public void Insert(OrderBindingModel model)
        {
            using var context = new SoftwareInstallationShopDatabase();
            context.Orders.Add(CreateModel(model, new Order()));
            context.SaveChanges();
        }
        public void Update(OrderBindingModel model)
        {
            using var context = new SoftwareInstallationShopDatabase();
            var element = context.Orders.FirstOrDefault(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            CreateModel(model, element);
            context.SaveChanges();
        }
        public void Delete(OrderBindingModel model)
        {
            using var context = new SoftwareInstallationShopDatabase();
            Order element = context.Orders.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Orders.Remove(element);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Элемент не найден");
            }
        }
        private static Order CreateModel(OrderBindingModel model, Order order)
        {
            order.PackageId = model.PackageId;
            order.Count = model.Count;
            order.Sum = model.Sum;
            order.Status = model.Status;
            order.DateCreate = model.DateCreate;
            order.DateImplement = model.DateImplement;
            return order;
        }
        private static OrderViewModel CreateModel(Order order)
        {
            using var context = new SoftwareInstallationShopDatabase();
            return new OrderViewModel
            {
                Id = order.Id,
                PackageId = order.PackageId,
                PackageName = context.Packages.FirstOrDefault(packageName => packageName.Id == order.PackageId)?.PackageName,
                Count = order.Count,
                Sum = order.Sum,
                Status = Enum.GetName(order.Status),
                DateCreate = order.DateCreate,
                DateImplement = order.DateImplement
            };
        }
    }
}
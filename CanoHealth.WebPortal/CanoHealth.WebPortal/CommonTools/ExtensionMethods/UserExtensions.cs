﻿using AutoMapper;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Persistance;
using CanoHealth.WebPortal.ViewModels;
using CanoHealth.WebPortal.ViewModels.Admin;
using IdentitySample.Models;
using System.Collections.Generic;
using System.Linq;

namespace CanoHealth.WebPortal.CommonTools.ExtensionMethods
{
    public static class UserExtensions
    {
        public static IEnumerable<UserFormViewModel> Convert(this IEnumerable<ApplicationUser> users)
        {
            return (from user in users
                    let data = GetCorporationAndRolesOfUser(user)

                    select new UserFormViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Active = user.Active,
                        Password = user.PasswordHash,
                        ConfirmPassword = user.PasswordHash,
                        Roles = data.Roles,//user.Roles.Select(d => new RoleViewModel { Id = d.RoleId }),
                        Corporations = data.Corporations
                    }).ToList();
        }

        private static IEnumerable<CorporationViewModel> GetCorporationAccessByUser(string userId)
        {
            var corporations = new List<CorporationViewModel>();
            using (var unitOfWork = new UnitOfWork(new ApplicationDbContext()))
            {
                corporations = unitOfWork.UserCorporationAccessRepository
                    .GetCorporationAccessByUser(userId)
                    .Select(Mapper.Map<Corporation, CorporationViewModel>)
                    .ToList();
            }
            return corporations;
        }

        private static IEnumerable<RoleViewModel> GetUserRoles(List<string> role)
        {
            var roles = new List<RoleViewModel>();
            using (var unitOfWork = new UnitOfWork(new ApplicationDbContext()))
            {
                roles = unitOfWork.RoleRepository.EnumarableGetAll(r => role.Contains(r.Id))
                    .Select(Mapper.Map<ApplicationRole, RoleViewModel>)
                    .ToList();
            }
            return roles;
        }

        private static CorporationRolesViewModel GetCorporationAndRolesOfUser(ApplicationUser user)
        {
            using (var unitOfWork = new UnitOfWork(new ApplicationDbContext()))
            {
                var corporations = unitOfWork.UserCorporationAccessRepository
                    .GetCorporationAccessByUser(user.Id)
                    .Select(Mapper.Map<Corporation, CorporationViewModel>)
                    .ToList();

                var rolesId = user.Roles.Select(x => x.RoleId).ToList();

                var roles = unitOfWork.RoleRepository.EnumarableGetAll(r => rolesId.Contains(r.Id))
                   .Select(Mapper.Map<ApplicationRole, RoleViewModel>)
                   .ToList();

                return new CorporationRolesViewModel
                {
                    Corporations = corporations,
                    Roles = roles
                };
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Services.AuditLogs;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StructureMap;
using System.Data.Entity;

namespace CanoHealth.WebPortal.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        #region Constructors and Destructors

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });
            //For<IExample>().Use<Example>(); 
            For<ILogs<DoctorSchedule>>().Use<Logs<DoctorSchedule>>();
            //For<IUserStore<ApplicationUser>>().Use<UserStore<ApplicationUser>>();
            //For<DbContext>().Use<ApplicationDbContext>();

            //this.For<IUserStore<ApplicationUser>>().Use(ctx => ctx.GetInstance<UserStore<ApplicationUser>>());
        }


        #endregion
    }
}
﻿using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Settings;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SettingsController : MrCMSAdminController
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISession _session;

        public SettingsController(IConfigurationProvider configurationProvider, ISession session)
        {
            _configurationProvider = configurationProvider;
            _session = session;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(SiteSettingsACL),SiteSettingsACL.View)]
        public ViewResult Index()
        {
            var settings = _configurationProvider.GetAllSiteSettings().FindAll(arg => arg.RenderInSettings);
            settings.ForEach(@base => @base.SetViewData(_session,ViewData));
            return View(settings);
        }

        [HttpPost]
        [ActionName("Index")]
        [MrCMSACLRule(typeof(SiteSettingsACL), SiteSettingsACL.Save)]
        public RedirectToRouteResult Index_Post([ModelBinder(typeof(SiteSettingsModelBinder))]List<SiteSettingsBase> settings)
        {
            settings.ForEach(s => _configurationProvider.SaveSettings(s));
            TempData["settings-saved"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult FileSystem()
        {
            return View(_configurationProvider.GetSiteSettings<FileSystemSettings>());
        }

        [HttpPost]
        public RedirectToRouteResult FileSystem([ModelBinder(typeof(FileSystemSettingsModelBinder))]FileSystemSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("FileSystem");
        }
    }
}
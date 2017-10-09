using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AAD._2tiers.Web.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net;

namespace AAD._2tiers.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.User = User.Identity.Name;

            //use https://jwt.io/ to visualize them
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            string idToken = await HttpContext.GetTokenAsync("id_token");
            ViewBag.AccessToken = accessToken;
            ViewBag.IdToken = idToken;

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {accessToken}");
                ViewBag.Message = client.DownloadString("http://localhost:10000/api/Hello");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error accessing APi layer " + ex.Message;
            }

            return View();
        }
    }
}

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

            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {accessToken}");

            try
            {
                ViewBag.Message = client.DownloadString("http://localhost:10000/Hello");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }

            try
            {
                ViewBag.MessageScopeA = client.DownloadString("http://localhost:10000/Hello/ScopeA");
            }
            catch (Exception ex)
            {
                ViewBag.MessageScopeA = "Error: " + ex.Message;
            }

            try
            {
                ViewBag.MessageScopeB = client.DownloadString("http://localhost:10000/Hello/ScopeB");
            }
            catch (Exception ex)
            {
                ViewBag.MessageScopeB = "Error: " + ex.Message;
            }

            
            

            return View();
        }
    }
}

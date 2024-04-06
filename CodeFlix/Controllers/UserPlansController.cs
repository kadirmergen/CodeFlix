using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeFlix.Data;
using CodeFlix.Models;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;

namespace CodeFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPlansController : ControllerBase
    {
        private readonly CodeFlixContext _context;

        public UserPlansController(CodeFlixContext context)
        {
            _context = context;
        }

        // GET: api/UserPlans
        [HttpGet]
        [Authorize]
        public ActionResult<List<UserPlan>> GetUserPlans()
        {
            return _context.UserPlans.ToList();
        }

        // GET: api/UserPlans/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<UserPlan> GetUserPlan(long id)
        {
            UserPlan? userPlan = _context.UserPlans.FindAsync(id).Result;
             
            if (userPlan == null)
            {
                return NotFound();
            }

            return userPlan;
        }
        
        // POST: api/UserPlans
        [HttpPost]
        [Authorize]
        public void PostUserPlan(string eMail, short planId)
        {
            Plan plan = _context.Plans.Find(planId)!;
            //get payment for  plan.Price
            //if(payment successful)
            UserPlan userPlan = new UserPlan();

            //userPlan.UserId= FindLocalPackagesResource from UserManager with EMail
            userPlan.PlanId = planId;
            userPlan.StartDate = DateTime.Today;
            userPlan.EndDate = userPlan.StartDate.AddMonths(1);
            _context.UserPlans.Add(userPlan);
            _context.SaveChanges();
        }
    }
}
